namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 描述，用于在数据库中生成描述
    /// </summary>
    public class DescriptionAttribute : ServiceStack.DataAnnotations.DescriptionAttribute
    {
        /// <summary>
        /// 描述，用于在数据库中生成描述
        /// </summary>
        /// <param name="description">描述字符串</param>
        public DescriptionAttribute(string description) : base(description) { }
    }
}