using System;
using System.Text.RegularExpressions;

namespace mobSocial.Data.Extensions
{
    public static class StringExtensions
    {


        public static string RemoveSpecChars(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = str.Replace(';', ',');
            str = str.Replace('\r', ' ');
            str = str.Replace('\n', ' ');
            str = str.Replace("\t", "   ");
            str = str.Replace("¼", "1/4");
            str = str.Replace("½", "1/2");
            str = str.Replace("¾", "3/4");

            return str;
        }

        public static string StripHtml(this string str)
        {
            // Needs modified to handle script tag content.

            string tagPattern = @"<[!--\W*?]*?[/]*?\w+.*?>";

            MatchCollection matches = Regex.Matches(str, tagPattern);

            foreach (Match match in matches)
            {
                str = str.Replace(match.Value, string.Empty);
            }

            return str;

        }

        public static string EncloseWith(this string str, char encloseCharacter)
        {
            return encloseCharacter + str + encloseCharacter;
        }

        public static string ToTitleCase(this string str)
        {
            string result = str;

            if (!string.IsNullOrEmpty(str))
            {
                str.ToLower();
                var words = str.Split(' ');
                for (int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if (s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        /// <summary>
        /// Replaces first occurance of search string with the replace string and returns the result string
        /// </summary>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool IsNullEmptyOrWhiteSpace(this string str)
        {
            str = str.Trim();
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }
    }
}
