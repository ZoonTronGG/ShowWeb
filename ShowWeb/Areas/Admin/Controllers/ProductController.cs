using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        var products = _unitOfWork.Product.GetAll().ToList();
        return View(products);
    }
    
    public IActionResult Upsert(int? id)
    {
        IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        });
        // ViewBag.CategoryList = categoryList;
        // ViewData["CategoryList"] = categoryList;
        var productVM = new ProductVM
        {
            Product = new Product(),
            CategoryList = categoryList
        };
        if (id is null or 0)
        {
            // create
            return View(productVM);
        }
        // update
        productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
        return View(productVM);

    }
    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Add(productVM.Product);
            _unitOfWork.Save();
            TempData["Success"] = "The product has been created successfully";
            return RedirectToAction("Index");
        }
        // ViewBag.CategoryList = categoryList;
        // ViewData["CategoryList"] = categoryList;
        productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        });
        return View(productVM);

    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _unitOfWork.Product.Get(u => u.Id == id);;
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var product = _unitOfWork.Product.Get(u => u.Id == id);
        if (product == null) return NotFound();
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["Success"] = "The product has been deleted successfully";
        return RedirectToAction("Index");
    }
}