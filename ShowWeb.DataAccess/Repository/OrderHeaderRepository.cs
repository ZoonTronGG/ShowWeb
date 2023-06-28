using System.Linq.Expressions;
using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;
using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _db;
    public OrderHeaderRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderHeader orderHeader)
    {
        _db.OrderHeaders.Update(orderHeader);
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
        if (orderFromDb == null) return;
        orderFromDb.OrderStatus = orderStatus;
        if (!string.IsNullOrWhiteSpace(paymentStatus))
        {
            orderFromDb.PaymentStatus = paymentStatus;
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
        if (orderFromDb == null) return;
        if (string.IsNullOrWhiteSpace(sessionId)) return;
        orderFromDb.SessionId = sessionId;
        if (string.IsNullOrWhiteSpace(paymentIntentId)) return;
        orderFromDb.PaymentIntentId = paymentIntentId;
        orderFromDb.PaymentDate = DateTime.Now;
    }
}