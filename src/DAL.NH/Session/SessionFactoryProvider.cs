using System;
using NHibernate;

namespace DAL.NH.Session
{
    public interface ISessionFactoryProvider : IDisposable
    {
        ISessionFactory GetSessionFactory();
        void Reset();
    }

    public class SessionFactoryProvider : ISessionFactoryProvider
    {
        private static ISessionFactory _sessionFactory;

        public ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory != null)
            {
                return _sessionFactory;
            }

            _sessionFactory = new NhConfigurator().GetNewConfiguration().BuildSessionFactory();
            return _sessionFactory;
        }

        public void Reset()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
                _sessionFactory = null;
            }
        }
    }
}
