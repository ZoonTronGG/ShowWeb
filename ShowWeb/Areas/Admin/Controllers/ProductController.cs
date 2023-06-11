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
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        var products = _unitOfWork.Product.GetAll(includeProperties: nameof(Product.Category)).ToList();
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
            string webRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(webRootPath, @"images\product");

                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    // delete old image
                    var oldFile = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                using var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create);
                file.CopyTo(fileStream);
                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }
            if(productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
                TempData["Success"] = "The product has been created successfully";
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
                TempData["Success"] = "The product has been updated successfully";
            }
            
            _unitOfWork.Save();
            
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

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.Product.GetAll(includeProperties: nameof(Product.Category));
        return Json(new { data = allObj });
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var objFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        if (objFromDb == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        var oldFile = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldFile))
        {
            System.IO.File.Delete(oldFile);
        }
        _unitOfWork.Product.Remove(objFromDb);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
    #endregion
}