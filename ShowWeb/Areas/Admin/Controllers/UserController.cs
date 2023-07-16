using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;
using ShowWeb.Utility;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserController(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> RoleManagement(string id)
    {
        var RoleVM = new RoleManagementVM()
        {
            ApplicationUser = _unitOfWork.ApplicationUser
                .Get(u => u.Id == id, includeProperties: nameof(Company)),
            RolesList = await _roleManager.Roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name,
            }).ToListAsync(),
            CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList()
        };

        if (RoleVM.ApplicationUser == null) return View(RoleVM);
        var user = _unitOfWork.ApplicationUser
            .Get(u => u.Id == id);
        RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
        return View(RoleVM);
    }

    [HttpPost]
    public async Task<IActionResult> RoleManagement(RoleManagementVM roleManagmentVM)
    {
        var oldRole = _userManager
            .GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
            .Result
            .FirstOrDefault();

        var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


        if (roleManagmentVM.ApplicationUser.Role != oldRole) {

            if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company) {
                applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
            }
            if (oldRole == SD.Role_Company) {
                applicationUser.CompanyId = null;
            }
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();

            await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
            await _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role);

        }
        else
        {
            if (oldRole != SD.Role_Company || applicationUser.CompanyId == roleManagmentVM.ApplicationUser.CompanyId)
                return RedirectToAction("Index");
            applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();
        }

        return RedirectToAction("Index");
    }

    #region API CALLS

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allObj = _unitOfWork.ApplicationUser
            .GetAll(includeProperties: nameof(Company)).ToList();
        foreach (var user in allObj)
        {
            user.Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            user.Company ??= new Company()
            {
                Name = ""
            };
        }

        return Json(new { data = allObj });
    }

    [HttpPost]
    public async Task<IActionResult> LockUnlock([FromBody] string id)
    {
        var objFromDb = _unitOfWork.ApplicationUser
            .Get(u => u.Id == id);
        if (objFromDb == null)
        {
            return Json(new { success = false, message = "Error while Locking/Unlocking" });
        }

        if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
        {
            // user is currently locked, we will unlock them
            objFromDb.LockoutEnd = DateTime.Now;
        }
        else
        {
            objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
        }
        
        _unitOfWork.ApplicationUser.Update(objFromDb);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Operation successful" });
    }

    #endregion
}