using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company company);
}