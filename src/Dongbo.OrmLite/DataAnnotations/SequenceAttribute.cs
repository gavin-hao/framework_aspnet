using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SequenceAttribute : ServiceStack.DataAnnotations.SequenceAttribute
    {
        public SequenceAttribute(string name) : base(name)
        {
        }
    }
}