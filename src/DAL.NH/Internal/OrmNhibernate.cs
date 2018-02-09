using System;
using System.Data;
using NHibernate;

namespace DAL.NH.Internal
{
    public interface IOrm : IDisposable
    {
        ISession Session { get; }

        void BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
    }

    internal class OrmNhibernate : IOrm
    {
        private ISession _session;
        private ITransaction _transaction;

        public OrmNhibernate(ISession session)
        {
            _session = session;
        }

        public ISession Session
        {
            get { return _session; }
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _transaction = _session.BeginTransaction(isolationLevel);
        }
        public void CommitTransaction()
        {
            CheckTransactionIsOpened();
            _transaction.Commit();
        }
        public void RollbackTransaction()
        {
            CheckTransactionIsOpened();

            if (_transaction.IsActive)
            {
                _transaction.Rollback();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                if (!_transaction.WasCommitted && !_transaction.WasRolledBack)
                {
                    RollbackTransaction();
                }

                _transaction.Dispose();
                _transaction = null;
            }

            _session.Dispose();
            _session = null;
        }

        private void CheckTransactionIsOpened()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("The transaction was not opened.");
            }
        }
    }
}