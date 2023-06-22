using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;

namespace ShowWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new ShoppingCartVM()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId,
                includeProperties: nameof(Product)),
        };

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartVM);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }

        if (shoppingCart.Count <= 100)
        {
            return shoppingCart.Product.Price50;
        }

        return shoppingCart.Product.Price100;
    }

    public IActionResult Plus(int id)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == id);
        if (cartFromDb != null)
        {
            cartFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int id)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == id);
        if (cartFromDb != null)
        {
            if (cartFromDb.Count <= 1)
            {
                // remove
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int id)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == id);
        if (cartFromDb != null)
        {
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        return View();
    }
}