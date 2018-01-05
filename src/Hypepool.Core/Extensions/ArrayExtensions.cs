using System;
using System.Collections.Generic;
using System.Text;

namespace Hypepool.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOf(this byte[] arr, byte val, int start, int count)
        {
            for (var i = start; i < start + count; i++)
            {
                if (arr[i] == val)
                    return i;
            }

            return -1;
        }
    }
}
