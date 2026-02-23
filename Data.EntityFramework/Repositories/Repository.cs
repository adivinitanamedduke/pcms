using Core.Repository;
using Data.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.EntityFramework.Repositories;

public class Repository<T, TKey>(InMemoryDbContext context) : IRepository<T, TKey> where T : class
{
    protected readonly InMemoryDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<TDomain?> GetDomainByIdAsync<TEntity, TDomain>(TKey id) where TEntity : class, IMappable<TDomain>
    {
        var entity = await GetByIdAsync(id);
        return entity != null ? TEntity.ToDomain(entity) : default;
    }

    public async Task<T?> GetByIdAsync(TKey id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

    #region Write Operations
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null) _dbSet.Remove(entity);

    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    #endregion Write operations
}

