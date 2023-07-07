using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShowWeb.DataAccess.Data;
using ShowWeb.Models;
using ShowWeb.Utility;

namespace ShowWeb.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;


    public DbInitializer(UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task Initialize()
    {
        // migrate migrations if not already migrated
        try
        {
            if ((await _db.Database.GetPendingMigrationsAsync()).Any())
            {
                await _db.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        // create roles if not already created
        if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
        {
            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
            
            // create admin
            await _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "admin@mail.ru",
                Email = "admin@mail.ru",
                Name = "Artyom Titorenko",
                PhoneNumber = "123456789",
                StreetAddress = "Almaty",
                City = "Almaty",
                PostalCode = "050000",
                State = "Kazakhstan",
            }, "Admin123*");

            var identityUser = await _db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == "admin@mail.ru");
        
            await _userManager.AddToRoleAsync(identityUser, SD.Role_Admin);
        }
       
    }
}