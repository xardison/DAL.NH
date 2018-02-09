using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAL.NH.Filter;
using NHibernate.Util;

namespace DAL.NH.FilterStrategy
{
    public interface IFilterStrategyFactory
    {
        IFilterStrategy<TEntity, TFilter> Create<TEntity, TFilter>() where TEntity : class where TFilter : IFilter;
    }

    public class FilterStrategyFactory : IFilterStrategyFactory
    {
        #region private fields
        private const string FilterStrategyInterfaceName = "IFilterStrategy`2";
        private static readonly Dictionary<string, Type> Strategies = new Dictionary<string, Type>();
        #endregion

        public static void Add<TFilteringStrategy>() where TFilteringStrategy : IFilterStrategy
        {
            Add(typeof(TFilteringStrategy));
        }

        private static void Add(Type type)
        {
            var pr = type.GetInterface(FilterStrategyInterfaceName);
            var args = pr.GenericTypeArguments;

            var entityTypeName = args[0].FullName;
            /* var filter = args[1].FullName; // тип фильтра */

            Strategies.Add(entityTypeName, type);
        }

        public static void Add(Assembly assembly)
        {
            var strategys =
                assembly.ExportedTypes.Where(t => t.IsClass && t.GetInterface(FilterStrategyInterfaceName) != null);

            strategys.ForEach(Add);
        }

        public IFilterStrategy<TEntity, TFilter> Create<TEntity, TFilter>() where TEntity : class where TFilter : IFilter
        {
            var entityTypeName = typeof(TEntity).FullName;
            var strategy = ProvideStrategy(entityTypeName);

            if (strategy == null)
            {
                throw new InvalidOperationException(
                    $"Could`t find strategy for entity = {typeof(TEntity)} and filter = {typeof(TFilter)}. Use 'FilterStrategyFactory.Add<TFilteringStrategy>()' method.");
            }

            var str = Activator.CreateInstance(strategy);
            return (IFilterStrategy<TEntity, TFilter>)str;
        }

        private Type ProvideStrategy(string entityTypeName)
        {
            return Strategies.FirstOrDefault(x => x.Key == entityTypeName).Value;
        }
    }
}