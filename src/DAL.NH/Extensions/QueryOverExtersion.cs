using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.NH.Filter;
using NHibernate;
using NHibernate.Criterion;
using Expression = System.Linq.Expressions.Expression;

namespace DAL.NH.Extensions
{
    public static class QueryOverExtersion
    {
        public static IQueryOver<TEntity, TEntity> Paging<TEntity>(this IQueryOver<TEntity, TEntity> query, IPager pager)
        {
            if (pager == null || pager.PageSize <= 0)
            {
                return query;
            }

            return query.SkipTake((pager.PageCurrent - 1) * pager.PageSize, pager.PageSize);
        }

        public static IQueryOver<TEntity, TEntity> FindByRn<TEntity>(this IQueryOver<TEntity, TEntity> query, object id)
            where TEntity : IHasId
        {
            if (id is long && (long)id <= 0)
            {
                return query;
            }

            if (id is int && (int)id <= 0)
            {
                return query;
            }

            if (id is string && string.IsNullOrWhiteSpace((string)id))
            {
                return query;
            }

            if (id is ICollection<long>)
            {
                return query.IsIn(x => x.Id, (id as ICollection<long>).ToArray());
            }

            return query.Where(x => x.Id == id);
        }

        public static IQueryOver<TEntity, TEntity> RowCount<TEntity>(this IQueryOver<TEntity, TEntity> query, out int total)
        {
            total = query.RowCount();
            return query;
        }

        public static IQueryOver<TEntity, TEntity> SkipTake<TEntity>(this IQueryOver<TEntity, TEntity> query, int skip, int take)
        {
            if (skip > 0)
            {
                query.Skip(skip);
            }

            if (take > 0)
            {
                query.Take(take);
            }

            return query;
        }

        public static IQueryOver<TEntity, TEntity> FetchEager<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property)
        {
            return query.Fetch(property).Eager;
        }

        public static IQueryOver<TEntity, TEntity> Like<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return query;
            }

            return Like(query, property, value, MatchMode.Start);
        }
        public static IQueryOver<TEntity, TEntity> Like<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value, MatchMode matchMode)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return query;
            }

            return query.Where(Restrictions.Like(Projections.Property(property), value, matchMode));
        }
        public static IQueryOver<TEntity, TEntity> LikeLower<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value, MatchMode matchMode)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return query;
            }

            return query.Where(
                Restrictions.Like(
                    Projections.SqlFunction("lower", NHibernateUtil.String, Projections.Property(property)),
                    value.ToLower(),
                    matchMode));
        }
        public static IQueryOver<TEntity, TEntity> LikeAny<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value)
        {
            return query.Like(property, value.ReplaceStr("", "%"), MatchMode.Anywhere);
        }
        public static IQueryOver<TEntity, TEntity> LikeAnyLower<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value)
        {
            return query.Like(property, value.ReplaceStr("", "%"), MatchMode.Anywhere);
        }

        public static IQueryOver<TEntity, TEntity> IsBetween<TEntity, T>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, T>> property, T from, T to)
        {
            if (from == null && to == null)
            {
                return query;
            }

            ParameterExpression paramExpr = Expression.Parameter(typeof(TEntity), "x");
            var fromConst = Expression.Constant(from, typeof(T));
            var toConst = Expression.Constant(to, typeof(T));
            Expression<Func<TEntity, bool>> fromLambda =
                Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(property.Body, fromConst),
                    paramExpr);
            Expression<Func<TEntity, bool>> toLambda =
                Expression.Lambda<Func<TEntity, bool>>(Expression.LessThanOrEqual(property.Body, toConst), paramExpr);

            if (from != null && to != null)
            {
                return query.Where(fromLambda).And(toLambda);
            }

            if (from != null)
            {
                return query.Where(fromLambda);
            }

            return query.Where(toLambda);
        }

        public static IQueryOver<TEntity, TEntity> IsIn<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, object[] parameters)
        {
            if (parameters != null && parameters.Any())
            {
                return query.WhereRestrictionOn(property).IsIn(parameters);
            }

            return query;
        }
        public static IQueryOver<TEntity, TEntity> IsIn<TEntity, T>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, ICollection<T> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                return query.WhereRestrictionOn(property).IsIn(parameters.ToArray());
            }

            return query;
        }
        public static IQueryOver<TEntity, TEntity> IsInStr<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return query;
            }

            var separator = ";";

            var str = value
                .Replace(Environment.NewLine, separator).Replace("", separator)
                .Replace(";", separator).Replace("; ", separator)
                .Replace(",", separator).Replace(", ", separator);

            var arr = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            return query.IsIn(property, arr);
        }

        public static IQueryOver<TEntity, TEntity> UserEqual<TEntity>(this IQueryOver<TEntity, TEntity> query, Expression<Func<TEntity, object>> property)
        {
            return query.Where(
                Restrictions.EqProperty(
                    Projections.SqlFunction("User", NHibernateUtil.String),
                    Projections.Property(property)));
        }
    }
}