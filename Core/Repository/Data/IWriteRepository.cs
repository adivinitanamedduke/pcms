namespace Core.Repository.Data
{
    public interface IWriteRepository<T, TKey> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(TKey id);
        Task<int> SaveChangesAsync();
    }
}
