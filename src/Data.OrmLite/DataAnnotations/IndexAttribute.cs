using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 索引
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class IndexAttribute : ServiceStack.DataAnnotations.IndexAttribute
    {
        /// <summary>
        /// 索引
        /// </summary>
        public IndexAttribute() : base()
        {
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="unique">唯一索引</param>
        public IndexAttribute(bool unique) : base(unique)
        {
        }
    }
}