using System.Linq.Expressions;
using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext _db;
    public ShoppingCartRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ShoppingCart shoppingCart)
    {
        _db.ShoppingCarts.Update(shoppingCart);
    }
}