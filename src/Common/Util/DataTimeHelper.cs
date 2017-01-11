using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dongbo.Common.Util
{
    public class DataTimeHelper
    {
        public static string GetDateDiff(DateTime time)
        {
            var span = DateTime.Now - time;
            var year = span.Days / 365;
            if (year > 0)
                return year + "年前";

            var month = span.Days / 30;
            if (month > 0)
                return month + "月前";

            var week = span.Days / 7;
            if (week > 0)
                return week + "周前";

            var day = span.Days;
            if (day > 0)
                return day + "天前";

            var hour = span.Hours;
            if (hour > 0)
                return hour + "小时前";

            var minute = span.Minutes;
            if (minute > 0)
                return minute + "分钟前";

            var second = span.Seconds;
            if (second > 15)
                return second + "秒前";

            return "刚刚";

        }
    }
}
