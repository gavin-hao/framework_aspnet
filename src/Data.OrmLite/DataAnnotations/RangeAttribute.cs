using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    public class RangeAttribute : ServiceStack.DataAnnotations.RangeAttribute
    {
        public RangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public RangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        public RangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }
    }
}