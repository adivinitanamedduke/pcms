using Core.Mapping;
using Domain.Services;

public class MappingService : IMappingService
{
    public TDomain MapToDomain<TEntity, TDomain>(TEntity entity)
        where TEntity : IMappable<TDomain>
    {
        return TEntity.ToDomain(entity);
    }
}