using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

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
    
    public IActionResult Create()
    {
        IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        });
        ViewBag.CategoryList = categoryList;
        // ViewData["CategoryList"] = categoryList;
        return View();
    }
    [HttpPost]
    public IActionResult Create(Product obj)
    {
        // if (obj.Name == obj.DisplayOrder.ToString())
        // {
        //     ModelState.AddModelError("Name", "Name and Display Order cannot be the same");
        // }
        if (!ModelState.IsValid) return View(obj);
        _unitOfWork.Product.Add(obj);
        _unitOfWork.Save();
        TempData["Success"] = "The product has been added successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _unitOfWork.Product.Get(u => u.Id == id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost]
    public IActionResult Edit(Product obj)
    {
        if (!ModelState.IsValid) return View(obj);
        _unitOfWork.Product.Update(obj);
        _unitOfWork.Save();
        TempData["Success"] = "The product has been updated successfully";
        return RedirectToAction("Index");
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