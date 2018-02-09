using System;
using System.Linq;
using System.Reflection;
using NHibernate.Util;

namespace DAL.NH.Extensions
{
    internal static class ReflectExtension
    {
        public static void GetPropertiesMarkAttribute<TAttribute>(this object @object,
            Action<TAttribute, object, Type> action)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            PropertyInfo[] pInfos = @object.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in pInfos)
            {
                var attributes = propertyInfo.GetCustomAttributes(true)
                    .Where(x => x.GetType() == typeof(TAttribute));
                if (attributes.Any())
                {
                    var attribute =
                        (TAttribute)propertyInfo.GetCustomAttributes(typeof(TAttribute), false).First();
                    action(attribute, propertyInfo.GetValue(@object, null), propertyInfo.PropertyType);
                }
            }
        }

        public static TRet InvokeGenericMethod<TRet>(this object evokedObject, Type genericType, string method,
            object[] parameters = null)
        {
            object returnType = evokedObject.GetType().GetMethod(method).MakeGenericMethod(genericType)
                .Invoke(evokedObject, parameters);
            return (TRet)returnType;
        }
    }
}