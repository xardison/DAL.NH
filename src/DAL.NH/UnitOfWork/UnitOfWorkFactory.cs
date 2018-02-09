using DAL.NH.Session;
using NHibernate;

namespace DAL.NH.UnitOfWork
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(bool openTransaction = false);
        IUnitOfWork Create(string database, bool openTransaction = false);
    }

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISessionProvider _sessionProvider;

        static UnitOfWorkFactory()
        {
            DalParams.Init();
        }

        public UnitOfWorkFactory()
        {
            _sessionProvider = new SessionProvider(new SessionFactoryProvider());
        }

        public IUnitOfWork Create(bool openTransaction = false)
        {
            var session = GetSession();
            return new UnitOfWork(session, openTransaction);
        }

        public IUnitOfWork Create(string database, bool openTransaction = false)
        {
            ConnectionStringManager.SetAuthData(database);
            new SessionFactoryProvider().Reset();
            return Create(openTransaction);
        }

        private ISession GetSession()
        {
            return _sessionProvider.CurrentSession ?? _sessionProvider.OpenSession();
        }
    }
}