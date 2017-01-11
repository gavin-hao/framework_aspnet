namespace Dongbo.OrmLite
{
    /// <summary>
    /// 数据库基类
    /// </summary>
    public abstract class Database
    {
        /// <summary>
        /// 在子类中返回当前数据库名称（架构名称）
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取表操作对象
        /// </summary>
        /// <typeparam name="T">要操作的表实体</typeparam>
        /// <returns></returns>
        protected Table<T> GetTable<T>() where T : class
        {
            return new Table<T>(Name);
        }
    }
}