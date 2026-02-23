using Core.Repository;
using Data.Entities;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace YourNamespace.Data.Repositories;

public class InMemoryCategoryRepository : IRepository<Category, int>
{
    // Mocking the database with a thread-safe dictionary
    private static readonly ConcurrentDictionary<int, Category> _storage = new();
    private static int _currentId = 0;

    public Task<Category?> GetByIdAsync(int id)
    {
        _storage.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Category>> GetAllAsync() =>
        Task.FromResult(_storage.Values.AsEnumerable());

    public Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate) =>
        Task.FromResult(_storage.Values.AsQueryable().Where(predicate).AsEnumerable());

    public Task AddAsync(Category entity)
    {
        entity.Id = Interlocked.Increment(ref _currentId);
        _storage.TryAdd(entity.Id, entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Category entity)
    {
        if (_storage.ContainsKey(entity.Id))
            _storage[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(int id)
    {
        _storage.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync() => Task.FromResult(1); // No-op for in-memory
}
