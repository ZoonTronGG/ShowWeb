using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}