using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;
using ShowWeb.Utility;
using Stripe;
using Stripe.Checkout;
using Product = ShowWeb.Models.Product;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderVM OrderVM { get; set; }
    
    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        OrderVM = new OrderVM()
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == id,
                includeProperties: nameof(ApplicationUser)),
            OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == id,
                includeProperties: nameof(Product))
        };

        return View(OrderVM);
    }
    
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
        // updating fields
        orderHeader.Name = OrderVM.OrderHeader.Name;
        orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        orderHeader.City = OrderVM.OrderHeader.City;
        orderHeader.State = OrderVM.OrderHeader.State;
        orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
        if(!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.TrackingNumber))
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        if (!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.Carrier))
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        
        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order details updated successfully";
        
        return RedirectToAction(nameof(Details), new {id = orderHeader.Id});
    }

    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    [HttpPost]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order status updated successfully";
        
        return RedirectToAction(nameof(Details), new {id = OrderVM.OrderHeader.Id});
    }
    
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    [HttpPost]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
        orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        orderHeader.ShippingDate = DateTime.Now;
        orderHeader.OrderStatus = SD.StatusShipped;
        if(orderHeader.PaymentStatus == SD.StatusPending)
            orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        
        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order shipped successfully";
        
        return RedirectToAction(nameof(Details), new {id = OrderVM.OrderHeader.Id});
    }
    
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    [HttpPost]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions()
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            var refund = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id,SD.StatusCancelled,SD.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id,SD.StatusCancelled, SD.StatusCancelled);
        }
        _unitOfWork.Save();
        TempData["Success"] = "Order cancelled successfully";
        
        return RedirectToAction(nameof(Details), new {id = OrderVM.OrderHeader.Id});
        
    }

    [ActionName("Details")]
    [HttpPost]
    public IActionResult DetailsPayNow()
    {
        OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id,
            includeProperties: nameof(ApplicationUser));
        OrderVM.OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == OrderVM.OrderHeader.Id,
            includeProperties: nameof(Product));
        
        const string domain = "https://localhost:7228/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + "Admin/Order/PaymentConfirmation?id=" + OrderVM.OrderHeader.Id,
            CancelUrl = domain + "Admin/Order/Details?id=" + OrderVM.OrderHeader.Id,
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
        };
        foreach (var item in OrderVM.OrderDetails)
        {
            var sessionLineItemOptions = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "kzt",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = item.Product.Title,
                    },
                },
                Quantity = item.Count,
            };
            options.LineItems.Add(sessionLineItemOptions);
        }
            
        var service = new SessionService();
            
        var optionsForPaymentIntent = new PaymentIntentCreateOptions
        {
            Amount = (long)(OrderVM.OrderHeader.OrderTotal * 100),
            Currency = "kzt",
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
        };

        var paymentService = new PaymentIntentService();
        var paymentIntent = paymentService.Create(optionsForPaymentIntent);
        var session = service.Create(options);
        _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, paymentIntent.Id);
        _unitOfWork.Save();
            
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }
    
    public IActionResult PaymentConfirmation(int id)
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == id);
        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            var sessionService = new SessionService();
            var session = sessionService.Get(orderHeader.SessionId);
            
            var paymentService = new PaymentIntentService();
            var paymentIntent = paymentService.Get(orderHeader.PaymentIntentId);

            if (session.Status.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id,
                    session.Id, paymentIntent.Id);
                _unitOfWork.OrderHeader.UpdateStatus(id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            else
            {
                orderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
        }

        return View(id);
    }
    
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders ;
        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: nameof(ApplicationUser)).ToList();
        else
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            orderHeaders = _unitOfWork.OrderHeader.GetAll(o => o.ApplicationUserId == claim.Value,
                includeProperties: nameof(ApplicationUser)).ToList();
        }
        
        switch (status)
        {
            case "pending":
                orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                break;
            case "completed":
                orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                break;
            case "approved":
                orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                break;
        }
        return Json(new { data = orderHeaders });
    }
    
    #endregion
}