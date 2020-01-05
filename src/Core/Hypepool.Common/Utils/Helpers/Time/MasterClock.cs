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

using System;

namespace Hypepool.Common.Utils.Helpers.Time
{
    /// <summary>
    /// Faster clock implementation.
    /// </summary>
    /// <remarks>
    ///  Uses UtcNow which is lot faster then Now.
    /// </remarks>
    /// <see href="https://stackoverflow.com/questions/1561791/optimizing-alternatives-to-datetime-now"/>
    public static class MasterClock
    {
        public static TimeSpan LocalUtcOffset { get; }

        /// <summary>
        /// Returns now.
        /// </summary>
        public static DateTime Now => DateTime.UtcNow + LocalUtcOffset;

        static MasterClock()
        {
            // get the offset.
            LocalUtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        }
    }
}
