using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShowWeb.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public string Manufacturer { get; set; }
    [Required]
    public string Model { get; set; }
    [Required]
    public string SKU { get; set; } // Stock Keeping Unit(inventory control)
    [Required]
    [Display(Name = "List Price")]
    [Range(1000, 1_000_000_000)]
    public double ListPrice { get; set; }
    
    [Required]
    [Display(Name = "Price for 1-50")]
    [Range(1000, 1_000_000_000)]
    public double Price { get; set; }
    
    [Required]
    [Display(Name = "Price for 50+")]
    [Range(1000, 1_000_000_000)]
    public double Price50 { get; set; }
    
    [Required]
    [Display(Name = "Price for 100+")]
    [Range(1000, 1_000_000_000)]
    public double Price100 { get; set; }
    
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category Category { get; set; }
    [ValidateNever]
    public string ImageUrl { get; set; }
}