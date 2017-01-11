using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 小数长度限制
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalLengthAttribute : ServiceStack.DataAnnotations.DecimalLengthAttribute
    {
        /// <summary>
        /// 小数长度限制
        /// </summary>
        public DecimalLengthAttribute() : base() { }

        /// <summary>
        /// 小数长度限制
        /// </summary>
        /// <param name="precision">总位数</param>
        public DecimalLengthAttribute(int precision) : base(precision) { }

        /// <summary>
        /// 小数长度限制
        /// </summary>
        /// <param name="precision">总位数</param>
        /// <param name="scale">小数位数</param>
        public DecimalLengthAttribute(int precision, int scale) : base(precision, scale) { }
    }
}