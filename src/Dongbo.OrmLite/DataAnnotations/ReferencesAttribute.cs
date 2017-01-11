using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ReferencesAttribute : ServiceStack.DataAnnotations.ReferencesAttribute
    {
        public ReferencesAttribute(Type type) : base(type)
        {
        }
    }
}