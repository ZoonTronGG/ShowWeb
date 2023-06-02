using Microsoft.AspNetCore.Mvc;
using ShowWeb.Data;
using ShowWeb.Models;

namespace ShowWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }
    public IActionResult Index()
    {
        var categories = _db.Categories.ToList();
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
        _db.Categories.Add(obj);
        _db.SaveChanges();
        TempData["Success"] = "The category has been added successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _db.Categories.Find(id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View(obj);
        _db.Categories.Update(obj);
        _db.SaveChanges();
        TempData["Success"] = "The category has been updated successfully";
        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        var obj = _db.Categories.Find(id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _db.Categories.Find(id);
        if (category == null) return NotFound();
        _db.Categories.Remove(category);
        _db.SaveChanges();
        TempData["Success"] = "The category has been deleted successfully";
        return RedirectToAction("Index");
    }
}