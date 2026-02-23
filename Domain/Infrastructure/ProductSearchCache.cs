using Core.Cache;
using Core.Models;
using System.Collections.Concurrent;

namespace Domain.Infrastructure;
public class ProductSearchCache : ICacheProvider
{
    private readonly ConcurrentDictionary<string, List<Product>> _cache = new();

    public List<Product> GetOrAdd(string key, Func<List<Product>> factory)
    {
        return _cache.GetOrAdd(key, _ => factory());
    }

    public void Clear() => _cache.Clear();
}