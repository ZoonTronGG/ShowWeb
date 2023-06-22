using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: nameof(Product.Category));
        return View(productList);
    }
    public IActionResult Details(int id)
    {
        var productFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: nameof(Product.Category));
        var shoppingCart = new ShoppingCart()
        {
            Product = productFromDb,
            ProductId = id,
            Count = 1
        };
        return View(shoppingCart);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;

        var shoppingCartFromDb = _unitOfWork.ShoppingCart
            .Get(s => s.ApplicationUserId == userId
                      && s.ProductId == shoppingCart.ProductId);
        if (shoppingCartFromDb != null)
        {
            // shopping cart item already exists
            shoppingCartFromDb.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
        }
        else
        {
            // shopping cart item does not exist
            // I don't know why but Id is equal to ProductId so I set it to 0 to create a new record
            if (shoppingCart.Id == shoppingCart.ProductId)
            {
                shoppingCart.Id = 0;
            }
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        TempData["Success"] = "Cart updated successfully";
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}