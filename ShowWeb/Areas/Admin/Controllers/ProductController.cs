using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;
using ShowWeb.Utility;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
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
        productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: nameof(Product.ProductImages));
        return View(productVM);

    }
    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
    {
        if (ModelState.IsValid)
        {
            if(productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
            }
            
            _unitOfWork.Save();
            var webRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var productPath = @"images\products\product-" + productVM.Product.Id;
                    var finalPath = Path.Combine(webRootPath, productPath);
                    
                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }
                    
                    using var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create);
                    file.CopyTo(fileStream);

                    var productImage = new ProductImage()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = productVM.Product.Id
                    };

                    if (productVM.Product.ProductImages == null)
                    {
                        productVM.Product.ProductImages = new List<ProductImage>();
                    }
                    
                    productVM.Product.ProductImages.Add(productImage);
                    
                    _unitOfWork.ProductImage.Add(productImage);
                }
                
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                
            }
            TempData["Success"] = "The product has been saved successfully";
            return RedirectToAction("Index");
        }

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

    public async Task<IActionResult> DeleteImage(int id)
    {
        var imageToDelete = _unitOfWork.ProductImage.Get(u => u.Id == id);
        if (imageToDelete == null) return NotFound();
        if (!string.IsNullOrWhiteSpace(imageToDelete.ImageUrl))
        {
            var oldFile = Path.Combine(_webHostEnvironment.WebRootPath, 
                imageToDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            
            _unitOfWork.ProductImage.Remove(imageToDelete);
            _unitOfWork.Save();
            
            TempData["Success"] = "The image has been deleted successfully";
            
        }
        return RedirectToAction("Upsert", new {id = imageToDelete.ProductId});
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
        var productPath = @"images\products\product-" + id;
        var finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
                    
        if (Directory.Exists(finalPath))
        {
            var filePaths = Directory.GetFiles(finalPath);
            foreach (var filePath in filePaths)
            {
                System.IO.File.Delete(filePath);
            }
            Directory.Delete(finalPath);
        }
        _unitOfWork.Product.Remove(objFromDb);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
    #endregion
}