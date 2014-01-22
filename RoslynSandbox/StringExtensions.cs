using System;
using System.Linq;

namespace RoslynSandbox
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string ToPascalCase(this string str)
        {
            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }
    }
}