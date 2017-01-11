using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 属于
    /// 用于在多表连接中标识该字段从哪个表获取
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BelongToAttribute : ServiceStack.DataAnnotations.BelongToAttribute
    {
        /// <summary>
        /// 属于表
        /// </summary>
        /// <param name="belongToTableType">目标表类型</param>
        public BelongToAttribute(Type belongToTableType) : base(belongToTableType)
        {
        }
    }
}