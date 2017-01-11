using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Configuration
{
    public class AppSettingProvider
    {
        public static string GetSetting(string key)
        {
            return AppSettingCollection.Instance[key];
        }
    }
}
