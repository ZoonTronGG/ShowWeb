﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
            },
            new Product()
            {
                Id = 2, Title = "Dell XPS 15", Description = "Dell XPS 15", 
                Manufacturer = "Dell", Model = "XPS 15", SKU = "SKU 2", 
                ListPrice = 120000, Price = 110000, Price50 = 100000, Price100 = 95000,
                CategoryId = 1,
            },
            new Product()
            { 
                Id = 3, Title = "Dell XPS 17", Description = "Dell XPS 17", 
                Manufacturer = "Dell", Model = "XPS 17", SKU = "SKU 3", 
                ListPrice = 150000, Price = 140000, Price50 = 135000, Price100 = 130000,
                CategoryId = 2,
            }
        );
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Tech Solution",
                Address = "132 Main St",
                City = "New York",
                State = "NY",
                PostalCode = "10001",
                PhoneNumber = "1234567890",
            },
            new Company
            {
                Id = 2,
                Name = "Super Tech Solution",
                Address = "132 Main St",
                City = "New York",
                State = "NY",
                PostalCode = "10001",
                PhoneNumber = "1234567890",
            },
            new Company
            {
                Id = 3,
                Name = "Mega Tech Solution",
                Address = "132 Main St",
                City = "New York",
                State = "NY",
                PostalCode = "10001",
                PhoneNumber = "1234567890",
            });
    }
}