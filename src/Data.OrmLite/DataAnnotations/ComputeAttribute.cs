using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 计算属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputeAttribute : ServiceStack.DataAnnotations.ComputeAttribute
    {
        /// <summary>
        /// 计算属性
        /// </summary>
        public ComputeAttribute() : base() { }

        /// <summary>
        /// 计算属性
        /// </summary>
        /// <param name="expression">计算表达式</param>
        public ComputeAttribute(string expression) : base(expression) { }
    }
}