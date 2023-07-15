using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShowWeb.DataAccess.Data;
using ShowWeb.Models;
using ShowWeb.Models.ViewModels;
using ShowWeb.Utility;

namespace ShowWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> RoleManagement(string id)
    {
        var role = await _db.UserRoles
            .FirstOrDefaultAsync(u => u.UserId == id);

        var RoleVM = new RoleManagementVM()
        {
            ApplicationUser = (await _db.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id))!,
            RolesList = await _db.Roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name,
            }).ToListAsync(),
            CompanyList = await _db.Companies.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToListAsync()
        };

        if (RoleVM.ApplicationUser != null)
            RoleVM.ApplicationUser.Role = await _db.Roles
                .Where(u => u.Id == role.RoleId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();

        return View(RoleVM);
    }

    [HttpPost]
    public async Task<IActionResult> RoleManagement(RoleManagementVM roleManagementVM)
    {
        var role = await _db.UserRoles
            .FirstOrDefaultAsync(u => u.UserId == roleManagementVM.ApplicationUser.Id);
        var oldRole = await _db.Roles
            .FirstOrDefaultAsync(u => role != null && u.Id == role.RoleId);
        
        if (roleManagementVM.ApplicationUser.Role == oldRole?.Name) return RedirectToAction(nameof(Index));
        
        var applicationUser = await _db.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == roleManagementVM.ApplicationUser.Id);
        if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
        {
            if (applicationUser != null) applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
        }

        if (oldRole?.Name == SD.Role_Company)
        {
            if (applicationUser != null) applicationUser.CompanyId = null;
        }

        await _db.SaveChangesAsync();

        if (applicationUser == null) return RedirectToAction(nameof(Index));
        
        await _userManager.RemoveFromRoleAsync(applicationUser, oldRole?.Name);
        await _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role);

        return RedirectToAction(nameof(Index));
    }

    #region API CALLS

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allObj = await _db.ApplicationUsers
            .Include(u => u.Company)
            .ToListAsync();

        var userRoles = await _db.UserRoles
            .ToListAsync();
        var roles = await _db.Roles
            .ToListAsync();

        foreach (var user in allObj)
        {
            var roleId = userRoles
                .FirstOrDefault(u => u.UserId == user.Id)?.RoleId;

            user.Role = roles
                .FirstOrDefault(u => u.Id == roleId)?.Name;
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
        var objFromDb = await _db.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == id);
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

        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Operation successful" });
    }

    #endregion
}