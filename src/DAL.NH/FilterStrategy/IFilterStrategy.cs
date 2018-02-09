using NHibernate;
using IFilter = DAL.NH.Filter.IFilter;

namespace DAL.NH.FilterStrategy
{
    public interface IFilterStrategy
    {
    }

    public interface IFilterStrategy<TEntity, in TFilter> : IFilterStrategy
        where TEntity : class where TFilter : IFilter
    {
        IQueryOver<TEntity, TEntity> Filtrate(IQueryOver<TEntity, TEntity> query, TFilter filter);
    }
}