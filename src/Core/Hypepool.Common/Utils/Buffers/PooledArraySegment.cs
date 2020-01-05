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
using System.Buffers;

namespace Hypepool.Common.Utils.Buffers
{
    public struct PooledArraySegment<T> : IDisposable
    {
        public PooledArraySegment(T[] array, int offset, int size)
        {
            Array = array;
            Size = size;
            Offset = offset;
        }

        public PooledArraySegment(int size, int offset = 0)
        {
            Array = ArrayPool<T>.Shared.Rent(size);
            Size = size;
            Offset = offset;
        }

        public T[] Array { get; private set; }
        public int Size { get; }
        public int Offset { get; }

        public T[] ToArray()
        {
            var result = new T[Size];
            System.Array.Copy(Array, Offset, result, 0, Size);
            return result;
        }

        #region IDisposable

        public void Dispose()
        {
            var array = Array;

            if (array != null)
            {
                ArrayPool<T>.Shared.Return(array);
                Array = null;
            }
        }

        #endregion
    }
}
