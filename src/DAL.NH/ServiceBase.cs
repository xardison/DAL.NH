using System.Collections.Generic;
using DAL.NH.Internal;
using DAL.NH.Repository;

namespace DAL.NH
{
    public interface IDalService
    {
    }

    public abstract class ServiceBase : IDalService
    {
        protected ServiceBase(IOrm orm, IRepositoryFactory repositoryFactory)
        {
            Orm = orm;
            RepositoryFactory = repositoryFactory;
        }

        protected IOrm Orm { get; private set; }
        protected IRepositoryFactory RepositoryFactory { get; set; }

        protected virtual IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return RepositoryFactory.Create<TEntity>().Specify().List();
        }

        protected virtual TEntity GetEntity<TEntity>(object id) where TEntity : class
        {
            return RepositoryFactory.Create<TEntity>().Get(id);
        }

        protected virtual TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var repository = RepositoryFactory.Create<TEntity>();
            var id = repository.Insert(entity);
            return repository.Get(id);
        }

        protected virtual void UpdateEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var repository = RepositoryFactory.Create<TEntity>();
            repository.Update(entity);
        }

        protected virtual void RemoveEntity<TEntity>(TEntity entity) where TEntity : class, IHasId
        {
            var repository = RepositoryFactory.Create<TEntity>();
            var sourceEntity = repository.Get(entity.Id);
            repository.Delete(sourceEntity);
        }

        protected virtual TEntity AddAndGetEntity<TEntity>(TEntity entity) where TEntity : class, IHasId
        {
            var repository = RepositoryFactory.Create<TEntity>();
            repository.Insert(entity);
            repository.Evict(entity);
            return repository.Get(entity.Id);
        }

        protected virtual TEntity UpdateAndGetEntity<TEntity>(TEntity entity) where TEntity : class, IHasId
        {
            var repository = RepositoryFactory.Create<TEntity>();
            repository.Update(entity);
            repository.Evict(entity);
            return repository.Get(entity.Id);
        }

        protected virtual TEntity UpsertEntity<TEntity>(TEntity entity) where TEntity : class, IHasId
        {
            var id = entity.Id as long?;
            return id != null && id > 0 ? UpdateAndGetEntity(entity) : AddAndGetEntity(entity);
        }
    }
}