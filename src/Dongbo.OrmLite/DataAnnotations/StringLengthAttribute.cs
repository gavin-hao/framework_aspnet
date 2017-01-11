namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 字符串长度
    /// </summary>
    public class StringLengthAttribute : ServiceStack.DataAnnotations.StringLengthAttribute
    {
        public StringLengthAttribute(int maximumLength) : base(maximumLength)
        {
        }

        public StringLengthAttribute(int minimumLength, int maximumLength) : base(minimumLength, maximumLength)
        {
        }
    }
}