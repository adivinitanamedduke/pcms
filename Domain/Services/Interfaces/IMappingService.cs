using Core.Mapping;

namespace Domain.Services;
public interface IMappingService
{
    TDomain MapToDomain<TEntity, TDomain>(TEntity entity)
        where TEntity : IMappable<TDomain>;
}

