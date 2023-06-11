using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Product product)
    {
        var objFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
        if (objFromDb == null) return;
        objFromDb.Title = product.Title;
        objFromDb.Description = product.Description;
        objFromDb.Manufacturer = product.Manufacturer;
        objFromDb.Model = product.Model;
        objFromDb.SKU = product.SKU;
        objFromDb.ListPrice = product.ListPrice;
        objFromDb.Price = product.Price;
        objFromDb.Price50 = product.Price50;
        objFromDb.Price100 = product.Price100;
        objFromDb.CategoryId = product.CategoryId;
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            objFromDb.ImageUrl = product.ImageUrl;
        }
    }
}