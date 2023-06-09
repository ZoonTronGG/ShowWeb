using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor_Temp.Data;
using WebRazor_Temp.Models;

namespace WebRazor_Temp.Pages.Categories;
[BindProperties]
public class Delete : PageModel
{
    private readonly ApplicationDbContext _db;
    public Category Category { get; set; }
    
    public Delete(ApplicationDbContext db)
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
         Category? category = _db.Categories.Find(Category.Id);
         if (category is null)
         {
              return NotFound();
         }
         _db.Categories.Remove(category);
         _db.SaveChanges();
         TempData["Success"] = "The category has been deleted successfully";
         return RedirectToPage("Index");
    }
}