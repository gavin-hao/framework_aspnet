using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class AliasAttribute : ServiceStack.DataAnnotations.AliasAttribute
    {
        /// <summary>
        /// 别名构造函数
        /// </summary>
        /// <param name="name">数据库中的名字</param>
        public AliasAttribute(string name) : base(name)
        {
        }
    }
}