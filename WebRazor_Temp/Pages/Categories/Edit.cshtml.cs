using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor_Temp.Data;
using WebRazor_Temp.Models;

namespace WebRazor_Temp.Pages.Categories;
[BindProperties]
public class Edit : PageModel
{
    private readonly ApplicationDbContext _db;
    public Category Category { get; set; }
    
    public Edit(ApplicationDbContext db)
    {
        _db = db;
    }
    public void OnGet(int? id)
    {
        if (id is null or 0)
        {
            NotFound();
        }
        else
        {
            Category = _db.Categories.Find(id);
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        _db.Categories.Update(Category);
        _db.SaveChanges();
        TempData["Success"] = "The category has been updated successfully";
        return RedirectToPage("Index");
    }
}