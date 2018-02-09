using NHibernate;

namespace DAL.NH.Repository
{
    public interface IRepository
    {
    }

    public interface IRepository<TEntity> : IRepository where TEntity : class
    {
        object Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }

    public interface INhReposityry<TEntity> : IRepository<TEntity> where TEntity : class
    {
        TEntity Get(object id);
        TEntity Load(object id);
        IQueryOver<TEntity, TEntity> Specify();
        void Evict(TEntity entity);
        TEntity Merge(TEntity entity);
        void Flush();
    }

    public class NhRepository<TEntity> : INhReposityry<TEntity> where TEntity : class
    {
        protected readonly ISession Session;

        public NhRepository(ISession session)
        {
            Session = session;
        }

        public virtual object Insert(TEntity entity)
        {
            return Session.Save(entity);
        }
        public virtual void Update(TEntity entity)
        {
            var newEntity = Session.Merge(entity);
            Session.Update(newEntity);
        }
        public virtual void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        public TEntity Get(object id)
        {
            return Session.Get<TEntity>(id);
        }
        public TEntity Load(object id)
        {
            return Session.Load<TEntity>(id);
        }
        public IQueryOver<TEntity, TEntity> Specify()
        {
            return Session.QueryOver<TEntity>();
        }
        public void Evict(TEntity entity)
        {
            Session.Evict(entity);
        }
        public TEntity Merge(TEntity entity)
        {
            return Session.Merge(entity);
        }
        public void Flush()
        {
            Session.Flush();
        }
    }
}
