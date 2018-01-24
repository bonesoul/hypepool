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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hypepool.Common.Utils.Buffers;
using Hypepool.Core.Utils.Extensions;
using Serilog;

namespace Hypepool.Core.Utils.Buffers
{
    public class PooledLineBuffer : IDisposable
    {
        private readonly Queue<PooledArraySegment<byte>> _recvQueue = new Queue<PooledArraySegment<byte>>();

        private readonly ILogger _logger;

        private int? _maxLength;

        private static readonly Encoding Encoding = Encoding.UTF8;

        private static readonly ArrayPool<byte> ByteArrayPool = ArrayPool<byte>.Shared;

        public PooledLineBuffer(int? maxLength = null)
        {
            this._maxLength = maxLength;
            _logger = Log.ForContext<PooledLineBuffer>().ForContext("Pool", "pool-name");
        }

        public void Receive<T>(T buffer, int bufferSize,
            Action<T, byte[], int> readBuffer,
            Action<PooledArraySegment<byte>> onNext,
            Action<Exception> onError,
            bool forceNewLine = false)
        {
            if (bufferSize == 0)
                return;

            // prevent flooding
            if (_maxLength.HasValue && bufferSize > _maxLength)
            {
                onError(new InvalidDataException($"Incoming data exceeds maximum of {_maxLength.Value}"));
                return;
            }

            var remaining = bufferSize;
            var buf = ArrayPool<byte>.Shared.Rent(bufferSize);
            var prevIndex = 0;
            var keepLease = false;

            try
            {
                // clear left-over contents
                if (buf.Length > bufferSize)
                    Array.Clear(buf, bufferSize, buf.Length - bufferSize);

                // read buffer
                readBuffer(buffer, buf, bufferSize);

                // diagnostics
                //_logger.Verbose($"recv: {Encoding.GetString(buf, 0, bufferSize)}");

                while (remaining > 0)
                {
                    // check if we got a newline
                    var index = buf.IndexOf(0xa, prevIndex, buf.Length - prevIndex);
                    var found = index != -1;

                    if (found || forceNewLine)
                    {
                        // fastpath
                        if (!forceNewLine && index + 1 == bufferSize && _recvQueue.Count == 0)
                        {
                            var length = index - prevIndex;

                            if (length > 0)
                            {
                                onNext(new PooledArraySegment<byte>(buf, prevIndex, length));
                                keepLease = true;
                            }

                            break;
                        }

                        // assemble line buffer
                        var queuedLength = _recvQueue.Sum(x => x.Size);
                        var segmentLength = !forceNewLine ? index - prevIndex : bufferSize - prevIndex;
                        var lineLength = queuedLength + segmentLength;
                        var line = ArrayPool<byte>.Shared.Rent(lineLength);
                        var offset = 0;

                        while (_recvQueue.TryDequeue(out var segment))
                        {
                            using (segment)
                            {
                                Array.Copy(segment.Array, 0, line, offset, segment.Size);
                                offset += segment.Size;
                            }
                        }

                        // append remaining characters
                        if (segmentLength > 0)
                            Array.Copy(buf, prevIndex, line, offset, segmentLength);

                        // emit
                        if (lineLength > 0)
                            onNext(new PooledArraySegment<byte>(line, 0, lineLength));

                        if (forceNewLine)
                            break;

                        prevIndex = index + 1;
                        remaining -= segmentLength + 1;
                        continue;
                    }

                    // store
                    if (prevIndex != 0)
                    {
                        var segmentLength = bufferSize - prevIndex;

                        if (segmentLength > 0)
                        {
                            var fragment = ArrayPool<byte>.Shared.Rent(segmentLength);
                            Array.Copy(buf, prevIndex, fragment, 0, segmentLength);
                            _recvQueue.Enqueue(new PooledArraySegment<byte>(fragment, 0, segmentLength));
                        }
                    }

                    else
                    {
                        _recvQueue.Enqueue(new PooledArraySegment<byte>(buf, 0, remaining));
                        keepLease = true;
                    }

                    // prevent flooding
                    if (_maxLength.HasValue && _recvQueue.Sum(x => x.Size) > _maxLength.Value)
                        onError(new InvalidDataException($"Incoming request size exceeds maximum of {_maxLength.Value}"));

                    break;
                }
            }

            finally
            {
                if (!keepLease)
                    ByteArrayPool.Return(buf);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            while (_recvQueue.TryDequeue(out var fragment))
                fragment.Dispose();
        }

        #endregion
    }
}
