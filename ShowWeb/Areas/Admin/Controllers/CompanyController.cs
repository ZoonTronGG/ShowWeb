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
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        var companies = _unitOfWork.Company.GetAll().ToList();
        return View(companies);
    }
    
    public IActionResult Upsert(int? id)
    {
        if (id is null or 0)
        {
            // create
            return View(new Company());
        }
        // update
        Company company = _unitOfWork.Company.Get(u => u.Id == id);
        return View(company);

    }
    [HttpPost]
    public IActionResult Upsert(Company company, IFormFile? file)
    {
        if (!ModelState.IsValid) return View(company);
        if(company.Id == 0)
        {
            _unitOfWork.Company.Add(company);
            TempData["Success"] = "The Company has been created successfully";
        }
        else
        {
            _unitOfWork.Company.Update(company);
            TempData["Success"] = "The Company has been updated successfully";
        }
            
        _unitOfWork.Save();
            
        return RedirectToAction("Index");
        // ViewBag.CategoryList = categoryList;
        // ViewData["CategoryList"] = categoryList;

    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var company = _unitOfWork.Company.Get(u => u.Id == id);
        if (company == null) return NotFound();
        _unitOfWork.Company.Remove(company);
        _unitOfWork.Save();
        TempData["Success"] = "The Company has been deleted successfully";
        return RedirectToAction("Index");
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.Company.GetAll();
        return Json(new { data = allObj });
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var objFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        if (objFromDb == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        _unitOfWork.Company.Remove(objFromDb);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
    #endregion
}