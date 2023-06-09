using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}