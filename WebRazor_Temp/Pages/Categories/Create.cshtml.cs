using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor_Temp.Data;
using WebRazor_Temp.Models;

namespace WebRazor_Temp.Pages.Categories;
// [BindProperties]
public class Create : PageModel
{
    private readonly ApplicationDbContext _db;
    [BindProperty]
    public Category Category { get; set; }
    
    public Create(ApplicationDbContext db)
    {
        _db = db;
    }
    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Categories.Add(Category);
        _db.SaveChanges();
        TempData["Success"] = "The category has been added successfully";
        return RedirectToPage("Index");
    }
}