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
    public class View<T> where T : class
    {
        private string _database;
        /// <summary>
        /// 表操作基类
        /// </summary>
        /// <param name="database">表所属数据库</param>
        public View(string database)
        {
            _database = database;
        }

        /// <summary>
        /// 取第一个结果
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            var list = await SelectAsync(where);
            return list.FirstOrDefault();
        }
        /// <summary>
        /// 取第一个结果
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Action<SqlExp<T>> where)
        {
            var list = await SelectAsync(where);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 取第一个结果
        /// </summary>
        /// <param name="sql">sql语句</param> 
        public async Task<T> FirstOrDefaultAsync(string sql,object anonType=null)
        {
            var list = await SelectAsync(sql,anonType);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="where">条件表达式</param>
        public async Task<List<T>> SelectAsync(Expression<Func<T, bool>> where)
        {
            using (var conn = GetConn())
            {
                return await conn.SelectAsync(where);
            }
        }

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="exp">SQL条件表达式</param>
        public async Task<List<T>> SelectAsync(Action<SqlExp<T>> exp)
        {
            var temp = GetMySqlExp();
            exp(temp);
            using (var conn = GetConn())
            {
                return await conn.SelectAsync(temp.GetSqlExpression());
            }
        }
        //public async Task<List<T>> SelectAsync(SqlExpression<T> exp)
        //{

        //    using (var conn = GetConn())
        //    {
        //        return await conn.SelectAsync(exp);
        //    }
        //}
        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="sql">SQL语句</param>
        public async Task<List<T>> SelectAsync(string sql, object anonType = null)
        {
            using (var conn = GetConn())
            {
                return await conn.SelectAsync<T>(sql, anonType);
            }
        }

        public async Task<List<T>> SelectAsync()
        {
            using (var conn = GetConn())
            {

                return await conn.SelectAsync<T>();
            }
        }
        /// <summary>
        /// 异步查询计数接口
        /// </summary>
        public async Task<long> CountAsync()
        {
            using (var conn = GetConn())
            {
                return await conn.CountAsync<T>();
            }
        }

        /// <summary>
        /// 异步查询计数接口
        /// </summary>
        public async Task<long> CountAsync(Action<SqlExp<T>> exp)
        {
            var temp = GetMySqlExp();
            exp(temp);
            using (var conn = GetConn())
            {

                return await conn.CountAsync(temp.GetSqlExpression());
            }
        }

        //public async Task<long> CountAsync(SqlExpression<T> exp)
        //{

        //    using (var conn = GetConn())
        //    {

        //        return await conn.CountAsync(exp);
        //    }
        //}
        /// <summary> 获取sql表达式生成器，用生成复杂sql操作 </summary>
        public SqlExp<T> GetMySqlExp()
        {
            return new SqlExp<T>(MySqlDialect.Provider.SqlExpression<T>());
        }

        internal System.Data.IDbConnection GetConn()
        {
            return ConnectionFactory.GetConn(_database);
        }
    }
}