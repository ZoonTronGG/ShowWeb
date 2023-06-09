using Microsoft.AspNetCore.Mvc;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        var categories = _unitOfWork.Category.GetAll().ToList();
        return View(categories);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Category obj)
    {
        // if (obj.Name == obj.DisplayOrder.ToString())
        // {
        //     ModelState.AddModelError("Name", "Name and Display Order cannot be the same");
        // }
        if (!ModelState.IsValid) return View(obj);
        _unitOfWork.Category.Add(obj);
        _unitOfWork.Save();
        TempData["Success"] = "The category has been added successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _unitOfWork.Category.Get(u => u.Id == id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View(obj);
        _unitOfWork.Category.Update(obj);
        _unitOfWork.Save();
        TempData["Success"] = "The category has been updated successfully";
        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _unitOfWork.Category.Get(u => u.Id == id);;
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _unitOfWork.Category.Get(u => u.Id == id);
        if (category == null) return NotFound();
        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();
        TempData["Success"] = "The category has been deleted successfully";
        return RedirectToAction("Index");
    }
}