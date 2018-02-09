using NHibernate;

namespace DAL.NH.Repository
{
    public interface IRepositoryFactory
    {
        INhReposityry<TEntity> Create<TEntity>() where TEntity : class;
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ISession _session;

        public RepositoryFactory(ISession session)
        {
            _session = session;
        }

        public INhReposityry<TEntity> Create<TEntity>() where TEntity : class
        {
            return new NhRepository<TEntity>(_session);
        }
    }
}