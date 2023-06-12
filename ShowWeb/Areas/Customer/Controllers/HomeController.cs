using System.Diagnostics;
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
        return View(productFromDb);
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