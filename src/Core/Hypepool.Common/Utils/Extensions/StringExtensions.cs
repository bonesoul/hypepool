#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hypepool.Common.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a str string to byte array.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(this string str)
        {
            if (str.StartsWith("0x"))
                str = str.Substring(2);

            var arr = new byte[str.Length >> 1];

            for (var i = 0; i < str.Length >> 1; ++i)
                arr[i] = (byte)((GetHexVal(str[i << 1]) << 4) + GetHexVal(str[(i << 1) + 1]));

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            var val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

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
