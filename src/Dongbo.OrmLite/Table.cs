using Dongbo.Configuration;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using ServiceStack.OrmLite.Dapper;
namespace Dongbo.OrmLite
{
    /// <summary>
    /// 表操作基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Table<T> : View<T> where T : class
    {
        private string _database;
        /// <summary>
        /// 表操作基类
        /// </summary>
        /// <param name="database">表所属数据库</param>
        public Table(string database) : base(database) { }

        /// <summary>
        /// 异步插入
        /// </summary>
        public async Task<long> InsertAsync(T model)
        {
            using (var conn = GetConn())
            {
                return await conn.InsertAsync(model);
            }
        }

        /// <summary>
        /// 异步插入
        /// </summary>
        public async Task InsertAsync(IEnumerable<T> items)
        {
            List<Task> tasks = new List<Task>();
            foreach (var item in items)
            {
                tasks.Add(InsertAsync(item));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// 异步删除
        /// </summary>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> where)
        {
            using (var conn = GetConn())
            {
                return await conn.DeleteAsync(where);
            }
        }
        /// <summary>
        /// 增量更新单个字段
        /// </summary>
        public async Task<int> IncrementAsync(object item, Expression<Func<T, bool>> where)
        {

            var props = item.GetType().GetProperties();
            var fields = ModelDefinition<T>
                .Definition
                .FieldDefinitions
                .Where(f => props.Any(p => p.Name == f.PropertyInfo.Name));
            if (!fields.Any())
            {
                return 0;
            }
            OrmLiteConfig.DialectProvider = MySqlDialect.Provider;
            var whereStr = new MySqlExpressionVisitor<T>().Where(where).WhereExpression;
            string sqlCmd = string.Format("UPDATE `{0}` SET {1} {2}",
                ModelDefinition<T>.Definition.ModelName,
                string.Join(",", fields.Select(f => string.Format("`{0}`=`{0}`+@{1}", f.FieldName, f.Name))),
                whereStr);//GetMySqlExp().Where(where).GetSqlExpression().WhereExpression
            using (var conn = GetConn())
            {
                return await conn.ExecuteAsync(sqlCmd, item);
            }
        }

       
        public async Task<int> UpdateAddAsync(Expression<Func<T>> updateFileds, SqlExpression<T> q)
        {

            using (var conn = GetConn())
            {

                return await conn.UpdateAddAsync(updateFileds, q);

            }
        }
        public async Task<int> IncrementAsync(object item, string where)
        {
            var props = item.GetType().GetProperties();
            var fields = ModelDefinition<T>
                .Definition
                .FieldDefinitions
                .Where(f => props.Any(p => p.Name == f.PropertyInfo.Name));
            if (!fields.Any())
            {
                return 0;
            }
            string sqlCmd = string.Format("UPDATE `{0}` SET {1} {2}",
                ModelDefinition<T>.Definition.ModelName,
                string.Join(",", fields.Select(f => string.Format("`{0}`=`{0}`+@{1}", f.FieldName, f.Name))),
               GetMySqlExp().GetSqlExpression().UnsafeWhere(where).WhereExpression);
            using (var conn = GetConn())
            {
                return await conn.ExecuteAsync(sqlCmd, item);
            }
        }
        /// <summary>
        /// 异步更新
        /// </summary>
        public async Task<int> UpdateAsync(object item, Expression<Func<T, bool>> where)
        {
            using (var conn = GetConn())
            {
                return await conn.UpdateAsync(item, where);
            }
        }
    }
}