using System;
using DAL.NH.Internal;
using NHibernate;
using NHibernate.Context;

namespace DAL.NH.Session
{
    public interface ISessionProvider
    {
        ISession CurrentSession { get; }

        ISession OpenSession();
    }

    public class SessionProvider : ISessionProvider
    {
        private readonly ISessionFactory _sessionFactory;

        public SessionProvider(ISessionFactoryProvider sessionFactoryProvider)
        {
            _sessionFactory = sessionFactoryProvider.GetSessionFactory();
        }

        public ISession CurrentSession
        {
            get
            {
                if (CurrentSessionContext.HasBind(_sessionFactory))
                {
                    return _sessionFactory.GetCurrentSession();
                }

                return null;

                throw new InvalidOperationException(
                    "Database access logic cannot be used, if session not opened. Implicitly session usage not allowed now. Please open session explicitly through UnitOfWorkFactory.Create method.");
            }
        }

        public ISession OpenSession()
        {
            var session = _sessionFactory.OpenSession();
            DbHelper.UpdateConnectionInfo(session.Connection);
            return session;
        }
    }
}