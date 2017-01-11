using System;
using System.Linq.Expressions;

namespace Dongbo.OrmLite
{
    /// <summary>
    /// sql表达式
    /// </summary>
    public class SqlExp<T>
    {
        private ServiceStack.OrmLite.SqlExpression<T> SqlExpression;

        public SqlExp(ServiceStack.OrmLite.SqlExpression<T> sqlexp)
        {
            SqlExpression = sqlexp;
        }

        public ServiceStack.OrmLite.SqlExpression<T> GetSqlExpression()
        {
            return SqlExpression;
        }
        public SqlExp<T> Select(string expString = "*")
        {
            SqlExpression.Select(expString);
            return this;
        }
        public SqlExp<T> Select<T1, T2>(Expression<Func<T1, T2, object>> fileds = null)
        {
            SqlExpression.Select<T1, T2>(fileds);
            return this;
        }
        public SqlExp<T> And(Expression<Func<T, bool>> predicate)
        {
            SqlExpression.And(predicate);
            return this;
        }

        public SqlExp<T> And<Target>(Expression<Func<Target, bool>> predicate)
        {
            SqlExpression.And(predicate);
            return this;
        }

        public SqlExp<T> And<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
        {
            SqlExpression.And(predicate);
            return this;
        }

        public SqlExp<T> ClearLimits()
        {
            SqlExpression.ClearLimits();
            return this;
        }

        public SqlExp<T> CrossJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            SqlExpression.CrossJoin<Target>(joinExpr);
            return this;
        }

        public SqlExp<T> CrossJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            SqlExpression.CrossJoin(joinExpr);
            return this;
        }

        public SqlExp<T> CustomJoin(string joinString)
        {
            SqlExpression.CustomJoin(joinString);
            return this;
        }

        public SqlExp<T> FullJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            SqlExpression.FullJoin<Target>(joinExpr);
            return this;
        }

        public SqlExp<T> FullJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            SqlExpression.FullJoin(joinExpr);
            return this;
        }

        public SqlExp<T> GroupBy()
        {
            SqlExpression.GroupBy();
            return this;
        }

        public SqlExp<T> GroupBy(string groupBy)
        {
            SqlExpression.GroupBy(groupBy);
            return this;
        }

        public SqlExp<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            SqlExpression.GroupBy(keySelector);
            return this;
        }

        public SqlExp<T> Having()
        {
            SqlExpression.Having();
            return this;
        }

        public SqlExp<T> Having(Expression<Func<T, bool>> predicate)
        {
            SqlExpression.Having(predicate);
            return this;
        }

        public SqlExp<T> Having(string sqlFilter, params object[] filterParams)
        {
            SqlExpression.Having(sqlFilter, filterParams);
            return this;
        }

        public SqlExp<T> Join(Type sourceType, Type targetType, Expression joinExpr = null)
        {
            SqlExpression.Join(sourceType, targetType, joinExpr = null);
            return this;
        }

        public SqlExp<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            SqlExpression.Join<Target>(joinExpr);
            return this;
        }

        public SqlExp<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            SqlExpression.Join(joinExpr);
            return this;
        }

        public SqlExp<T> LeftJoin(Type sourceType, Type targetType, Expression joinExpr = null)
        {
            SqlExpression.LeftJoin(sourceType, targetType, joinExpr);
            return this;
        }

        public SqlExp<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            SqlExpression.LeftJoin<Target>(joinExpr);
            return this;
        }

        public SqlExp<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            SqlExpression.LeftJoin(joinExpr);
            return this;
        }

        public SqlExp<T> Limit()
        {
            SqlExpression.Limit();
            return this;
        }

        public SqlExp<T> Limit(int rows)
        {
            SqlExpression.Limit(rows);
            return this;
        }

        public SqlExp<T> Limit(int? skip, int? rows)
        {
            SqlExpression.Limit(skip, rows);
            return this;
        }

        public SqlExp<T> Limit(int skip, int rows)
        {
            SqlExpression.Limit(skip, rows);
            return this;
        }

        public SqlExp<T> Or(Expression<Func<T, bool>> predicate)
        {
            SqlExpression.Or(predicate);
            return this;
        }

        public SqlExp<T> Or<Target>(Expression<Func<Target, bool>> predicate)
        {
            SqlExpression.Or(predicate);
            return this;
        }

        public SqlExp<T> Or<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
        {
            SqlExpression.Or(predicate);
            return this;
        }

        public SqlExp<T> OrderBy()
        {
            SqlExpression.OrderBy();
            return this;
        }

        public SqlExp<T> OrderBy(Expression<Func<T, object>> keySelector)
        {
            SqlExpression.OrderBy(keySelector);
            return this;
        }

        public SqlExp<T> OrderBy(string orderBy)
        {
            SqlExpression.OrderBy(orderBy);
            return this;
        }

        public SqlExp<T> OrderBy<Table>(Expression<Func<Table, object>> keySelector)
        {
            SqlExpression.OrderBy(keySelector);
            return this;
        }

        public SqlExp<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            SqlExpression.OrderByDescending(keySelector);
            return this;
        }

        public SqlExp<T> OrderByDescending<Table>(Expression<Func<Table, object>> keySelector)
        {
            SqlExpression.OrderByDescending(keySelector);
            return this;
        }

        public SqlExp<T> OrderByFields(params string[] fieldNames)
        {
            SqlExpression.OrderByFields(fieldNames);
            return this;
        }

        public SqlExp<T> OrderByFieldsDescending(params string[] fieldNames)
        {
            SqlExpression.OrderByFieldsDescending(fieldNames);
            return this;
        }

        public SqlExp<T> OrderByRandom()
        {
            SqlExpression.OrderByRandom();
            return this;
        }

        public SqlExp<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            SqlExpression.RightJoin<Target>(joinExpr);
            return this;
        }

        public SqlExp<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            SqlExpression.RightJoin(joinExpr);
            return this;
        }

        public SqlExp<T> Skip(int? skip = default(int?))
        {
            SqlExpression.Skip(skip);
            return this;
        }

        public SqlExp<T> Take(int? take = default(int?))
        {
            SqlExpression.Take(take);
            return this;
        }

        public SqlExp<T> ThenBy(Expression<Func<T, object>> keySelector)
        {
            SqlExpression.ThenBy(keySelector);
            return this;
        }

        public SqlExp<T> ThenBy(string orderBy)
        {
            SqlExpression.ThenBy(orderBy);
            return this;
        }

        public SqlExp<T> ThenBy<Table>(Expression<Func<Table, object>> keySelector)
        {
            SqlExpression.ThenBy(keySelector);
            return this;
        }

        public SqlExp<T> ThenByDescending(Expression<Func<T, object>> keySelector)
        {
            SqlExpression.ThenByDescending(keySelector);
            return this;
        }

        public SqlExp<T> ThenByDescending(string orderBy)
        {
            SqlExpression.ThenByDescending(orderBy);
            return this;
        }

        public SqlExp<T> ThenByDescending<Table>(Expression<Func<Table, object>> keySelector)
        {
            SqlExpression.ThenByDescending(keySelector);
            return this;
        }

        public SqlExp<T> Where()
        {
            SqlExpression.Where();
            return this;
        }

        public SqlExp<T> Where(Expression<Func<T, bool>> predicate)
        {
            SqlExpression.Where(predicate);
            return this;
        }

        public SqlExp<T> Where<Target>(Expression<Func<Target, bool>> predicate)
        {
            SqlExpression.Where(predicate);
            return this;
        }

        public SqlExp<T> Where<Source, Target>(Expression<Func<Source, Target, bool>> predicate)
        {
            SqlExpression.Where(predicate);
            return this;
        }


    }
}