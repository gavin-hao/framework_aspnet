using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.ExtensionMethod
{
    public static class IEnumerableExtension
    {
        public static string Join(this IEnumerable<string> list, string separator)
        {
            return string.Join(separator, list);
        }


        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
