using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hypepool.Core.Extensions
{
    public static class StringExtensions
    {
        public static string FormatJson(this string input)
        {
            try
            {
                return JToken.Parse(input).ToString(Formatting.Indented);
            }
            catch
            {
                return input;
            }
        }
    }
}
