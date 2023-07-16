using System.Linq.Expressions;
using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository;

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _db;
    public ProductImageRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ProductImage obj)
    {
        _db.ProductImages.Update(obj);
    }
}