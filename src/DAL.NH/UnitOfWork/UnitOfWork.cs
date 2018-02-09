using System;
using System.Data;
using DAL.NH.Internal;
using DAL.NH.Repository;
using NHibernate;
using NHibernate.Context;

namespace DAL.NH.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        T CreateService<T>() where T : IDalService;

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
    }

    internal class UnitOfWork : IUnitOfWork
    {
        #region private fields
        private readonly IOrm _orm;
        private ISession _session;
        #endregion

        public UnitOfWork(ISession session, bool isAutoStartTransaction = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            CurrentSessionContext.Bind(session);
            _session = session;
            _orm = new OrmNhibernate(_session);

            if (isAutoStartTransaction)
            {
                BeginTransaction(isolationLevel);
            }
        }

        public T CreateService<T>() where T : IDalService
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { _orm, new RepositoryFactory(_session) });
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _orm.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _orm.CommitTransaction();
        }

        public void Rollback()
        {
            _orm.RollbackTransaction();
        }

        public void Dispose()
        {
            CurrentSessionContext.Unbind(_session.SessionFactory);
            _session.Dispose();
            _session = null;
            _orm.Dispose();
        }
    }
}
