using System.Linq.Expressions;
using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _db;
    public OrderDetailRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderDetail orderDetail)
    {
        _db.OrderDetails.Update(orderDetail);
    }
}