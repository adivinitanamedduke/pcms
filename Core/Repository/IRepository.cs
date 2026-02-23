using Core.Repository.Data;

namespace Core.Repository
{
    public interface IRepository<T, TKey> : IReadRepository<T, TKey>, IWriteRepository<T, TKey> where T : class
    {
    }
}

