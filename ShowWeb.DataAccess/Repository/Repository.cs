using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShowWeb.DataAccess.Data;
using ShowWeb.DataAccess.Repository.IRepository;

namespace ShowWeb.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>(); // this is the same as db.Categories == dbSet
    }

    public IEnumerable<T> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (string.IsNullOrEmpty(includeProperties)) return query.ToList();
        foreach (var includeProp in includeProperties
                     .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProp);
        }
        return query.ToList();
    }
    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        if (string.IsNullOrEmpty(includeProperties)) return query.FirstOrDefault();
        foreach (var includeProp in includeProperties
                     .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProp);
        }
        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        dbSet.RemoveRange(entity);
    }
}