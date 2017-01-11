using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    internal static class StringExtensions
    {
        static string email = @"^([a-zA-Z0-9][_\.\-]*)+\@([A-Za-z0-9])+((\.|-|_)[A-Za-z0-9]+)*((\.[A-Za-z0-9]{2,4}){1,2})$";
        static string phones = @"^(\+?0?86\-?)?1[345789][0-9]{9}$";
        static string creditCard = @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$";

        static string isbn10Maybe = "^(?:[0-9]{9}X|[0-9]{10})$";
        static string isbn13Maybe = "^(?:[0-9]{13})$";

        //var ipv4Maybe = /^(\d?\d?\d)\.(\d?\d?\d)\.(\d?\d?\d)\.(\d?\d?\d)$/
        //  , ipv6 = /^::|^::1|^([a-fA-F0-9]{1,4}::?){1,7}([a-fA-F0-9]{1,4})$/;

        //var uuid = {
        //    '3': /^[0-9A-F]{8}-[0-9A-F]{4}-3[0-9A-F]{3}-[0-9A-F]{4}-[0-9A-F]{12}$/i
        //  , '4': /^[0-9A-F]{8}-[0-9A-F]{4}-4[0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$/i
        //  , '5': /^[0-9A-F]{8}-[0-9A-F]{4}-5[0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$/i
        //  , all: /^[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}$/i
        //};

        //var alpha = /^[a-zA-Z]+$/
        //  , alphanumeric = /^[a-zA-Z0-9]+$/
        //  , numeric = /^-?[0-9]+$/
        //  , int = /^(?:-?(?:0|[1-9][0-9]*))$/
        //  , float = /^(?:-?(?:[0-9]+))?(?:\.[0-9]*)?(?:[eE][\+\-]?(?:[0-9]+))?$/
        //  , hexadecimal = /^[0-9a-fA-F]+$/
        //  , hexcolor = /^#?([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$/;

        //var ascii = /^[\x00-\x7F]+$/
        //  , multibyte = /[^\x00-\x7F]/
        //  , fullWidth = /[^\u0020-\u007E\uFF61-\uFF9F\uFFA0-\uFFDC\uFFE8-\uFFEE0-9a-zA-Z]/
        //  , halfWidth = /[\u0020-\u007E\uFF61-\uFF9F\uFFA0-\uFFDC\uFFE8-\uFFEE0-9a-zA-Z]/;

        //var surrogatePair = /[\uD800-\uDBFF][\uDC00-\uDFFF]/;

        //var base64 = /^(?:[A-Za-z0-9+\/]{4})*(?:[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{4})$/;






        public static bool IsMobilePhone(this string text)
        {
            return Regex.IsMatch(text, phones);
        }

        public static bool IsEmail(this string text)
        {
            return Regex.IsMatch(text, email);
        }

        public static string NormalizeEmail(this string email)
        {

            if (!email.IsEmail())
                return "";
            var parts = email.Split('@');
            parts[1] = parts[1].ToLower();

            parts[0] = parts[0].ToLower();
            if (parts[1] == "gmail.com" || parts[1] == "googlemail.com")
            {
                parts[0] = parts[0].Replace(".", "").Split('+')[0];
                parts[1] = "gmail.com";
            }
            return String.Join("@", parts);
        }
    }
}
