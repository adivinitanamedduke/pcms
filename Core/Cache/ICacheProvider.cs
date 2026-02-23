using Core.Models;

namespace Core.Cache;
public interface ICacheProvider
{
    List<Product> GetOrAdd(string key, Func<List<Product>> factory);
    void Clear();
}
