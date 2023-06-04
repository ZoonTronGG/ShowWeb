using Microsoft.EntityFrameworkCore;
using WebRazor_Temp.Models;

namespace WebRazor_Temp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Laptops",
                DisplayOrder = 1
            },
            new Category
            {
                Id = 2,
                Name = "PCs",
                DisplayOrder = 2
            },
            new Category
            {
                Id = 3,
                Name = "Mobiles",
                DisplayOrder = 3
            });
    }
}