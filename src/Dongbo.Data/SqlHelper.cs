using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Data
{
    public class SqlHelper
    {
        public static readonly string[] IllegalWords = {
            "'",
            "<",
            ">",
            ";", 
            "(", 
            ")", 
            "* ", 
            "% ", 
            "--",
            "and ",
            "or ",
            "select ",
            "update ", 
            "delete ", 
            "drop ", 
            "create ", 
            "union ", 
            "insert ", 
            "net ", 
            "truncate ", 
            "exec ", 
            "declare ", 
            "and ", 
            "count ", 
            "chr ", 
            "mid ", 
            "master ", 
            "char "
        };

        /// <summary>
        /// Check if the sql parameter value contains illegal words
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// true: The parameter value does NOT contain illegal words
        /// false: The parameter value does contain illegal words
        /// </returns>
        public static bool CheckSqlParameter(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (var word in IllegalWords)
                {
                    if (value.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                        return false;
                }
            }

            return true;
        }
    }
}
