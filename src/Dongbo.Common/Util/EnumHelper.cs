using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dongbo.Common.Util
{
    public class EnumHelper
    {
        public static string GetEnumDesc(object enumValue)
        {
            if (enumValue == null) throw new ArgumentNullException("enumValue");
            if (!enumValue.GetType().IsEnum) throw new Exception("参数类型不正确");

            Type _enumType = enumValue.GetType();
            DescriptionAttribute dna = null;
            FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, enumValue));
            dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
               fi, typeof(DescriptionAttribute));
            if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                return dna.Description;
            return fi.Name;
        }

        public static int[] GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("参数类型不正确");

            int[] values = Enum.GetValues(enumType) as int[];
            return values;
        }
    }
}
