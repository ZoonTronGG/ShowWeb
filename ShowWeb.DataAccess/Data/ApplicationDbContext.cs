using Microsoft.EntityFrameworkCore;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

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
        modelBuilder.Entity<Product>().HasData(
            new Product()
            {
                Id = 1, Title = "Dell XPS 13", Description = "Dell XPS 13", 
                Manufacturer = "Dell", Model = "XPS 13", SKU = "SKU 1", 
                ListPrice = 100000, Price = 95000, Price50 = 90000, Price100 = 85000,
                CategoryId = 1,
                ImageUrl = "https://petapixel.com/assets/uploads/2022/01/Dell-Launches-the-Ultra-Modern-XPS-13-Plus-with-OLED-and-Intels-Latest-800x420.jpg"
            },
            new Product()
            {
                Id = 2, Title = "Dell XPS 15", Description = "Dell XPS 15", 
                Manufacturer = "Dell", Model = "XPS 15", SKU = "SKU 2", 
                ListPrice = 120000, Price = 110000, Price50 = 100000, Price100 = 95000,
                CategoryId = 1,
                ImageUrl = "https://avatars.mds.yandex.net/get-mpic/5288781/img_id2302440974157776602.jpeg/orig"
            },
            new Product()
            { 
                Id = 3, Title = "Dell XPS 17", Description = "Dell XPS 17", 
                Manufacturer = "Dell", Model = "XPS 17", SKU = "SKU 3", 
                ListPrice = 150000, Price = 140000, Price50 = 135000, Price100 = 130000,
                CategoryId = 2,
                ImageUrl = "https://nout.kz/upload/resize_cache/webp/iblock/d16/Screenshot_1.webp"
            }
        );
    }
}