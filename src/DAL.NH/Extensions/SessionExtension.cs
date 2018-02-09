using System;
using System.Collections;
using DAL.NH.StoredProcedure;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace DAL.NH.Extensions
{
    public static class SessionExtension
    {
        public static IQuery ExecuteSp(this ISession session, IStoredProcedure storedProcedure)
        {
            IQuery namedQuery = session.GetNamedQuery(storedProcedure.Name);

            if (namedQuery == null)
            {
                throw new InvalidOperationException($"The stored procedure '{storedProcedure.Name}' not found");
            }

            storedProcedure.GetPropertiesMarkAttribute(
                (StoredParameterAttribute attribute, object value, Type propertyType) =>
                {
                    if (value != null)
                    {
                        if (propertyType.IsGenericCollection())
                        {
                            namedQuery.SetParameterList(attribute.ParameterName, (IEnumerable)value);
                        }
                        else
                        {
                            namedQuery.SetParameter(attribute.ParameterName, value);
                        }
                    }
                    else
                    {
                        namedQuery.SetParameter(attribute.ParameterName, null,
                            TypeFactory.GetSerializableType(propertyType));
                    }
                });

            return namedQuery;
        }
    }
}