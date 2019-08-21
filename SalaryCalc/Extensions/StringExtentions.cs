using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SalaryCalc.Extensions
{
    public static class StringExtentions
    {
            public static List<string> EverythingBetween(this string source, string start, string end)
            {
                var results = new List<string>();

                string pattern = string.Format(
                    "{0}({1}){2}",
                    Regex.Escape(start),
                    ".+?",
                     Regex.Escape(end));

                foreach (Match m in Regex.Matches(source, pattern))
                {
                    results.Add(m.Groups[1].Value);
                }

                return results;
            }
            public static int Matches(this string text, string pattern)
        {
            int count = 0, i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
          public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

    }
}