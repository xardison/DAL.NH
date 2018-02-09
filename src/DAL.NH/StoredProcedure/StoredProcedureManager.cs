using System;
using System.Collections;
using System.Collections.Generic;
using DAL.NH.Extensions;
using NHibernate;
using NHibernate.Transform;

namespace DAL.NH.StoredProcedure
{
    public interface IStoredProcedureManager
    {
        void Execute(IStoredProcedure storedProcedure);
        TRet Execute<TRet>(IStoredProcedure storedProcedure);
        TRet ExecuteResultTransformer<TRet>(IStoredProcedure storedProcedure);
    }

    public class StoredProcedureManager : IStoredProcedureManager
    {
        private readonly ISession _session;

        public StoredProcedureManager(ISession session)
        {
            _session = session;
        }

        public void Execute(IStoredProcedure storedProcedure)
        {
            NhibernateExecuteSp(storedProcedure).UniqueResult();
        }
        public TRet Execute<TRet>(IStoredProcedure storedProcedure)
        {
            return ExecuteBase<TRet>(storedProcedure, false);
        }
        public TRet ExecuteResultTransformer<TRet>(IStoredProcedure storedProcedure)
        {
            return ExecuteBase<TRet>(storedProcedure, true);
        }

        private TRet ExecuteBase<TRet>(IStoredProcedure storedProcedure, bool needTransform)
        {
            if (IsSubclassOrRawGeneric(typeof(IList<>), typeof(TRet)))
            {
                Type itemType = typeof(TRet).GetProperty("Item").PropertyType;
                var query = NhibernateExecuteSp(storedProcedure);
                if (needTransform)
                {
                    query = query.SetResultTransformer(Transformers.AliasToBean(itemType));
                }
                var result = query.List();
                var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                foreach (var item in result)
                {
                    collection.Add(item);
                }

                return (TRet)collection;
            }

            return NhibernateExecuteSp(storedProcedure).UniqueResult<TRet>();
        }
        private bool IsSubclassOrRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var currentType = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (currentType == generic)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }

            return false;
        }
        private IQuery NhibernateExecuteSp(IStoredProcedure storedProcedure)
        {
            return _session.ExecuteSp(storedProcedure);
        }
    }
}