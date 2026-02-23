namespace Data.Mapping;

public interface IMappable<TDomain>
{
    static abstract TDomain ToDomain(object entity);
}

