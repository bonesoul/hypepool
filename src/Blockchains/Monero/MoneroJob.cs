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
using System.Linq;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Native;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Buffers;
using Hypepool.Common.Utils.Extensions;
using Hypepool.Monero.Daemon.Responses;
using Hypepool.Monero.Stratum.Responses;
using NBitcoin.BouncyCastle.Math;

namespace Hypepool.Monero
{
    public class MoneroJob : IJob
    {
        public int Id { get; }

        public GetBlockTemplateResponse BlockTemplate { get; }

        private byte[] _blobTemplate;
        private uint extraNonce = 0;
        private readonly JobCounter _workerJobCounter;

        public MoneroJob(int id ,GetBlockTemplateResponse blockTemplate, byte[] instanceId)
        {
            Id = id;
            BlockTemplate = blockTemplate;

            _workerJobCounter = new JobCounter();
            PrepareBlobTemplate(instanceId);
        }

        private void PrepareBlobTemplate(byte[] instanceId)
        {
            _blobTemplate = BlockTemplate.Blob.HexToByteArray();

            // inject instanceId at the end of the reserved area of the blob
            var destOffset = (int)BlockTemplate.ReservedOffset + MoneroConstants.ExtraNonceSize;
            Buffer.BlockCopy(instanceId, 0, _blobTemplate, destOffset, MoneroConstants.InstanceIdSize);
        }

        public MoneroJobParams CreateWorkerJob(IStratumClient client)
        {
            var context = client.GetContextAs<MoneroWorkerContext>();
            var workerJob = new MoneroWorkerJob(_workerJobCounter.GetNext(), BlockTemplate.Height, context.Difficulty, ++extraNonce);

            var blob = EncodeBlob(workerJob.ExtraNonce);
            var target = EncodeTarget(workerJob.Difficulty);

            return null;
        }

        private string EncodeBlob(uint workerExtraNonce)
        {
            // clone template
            using (var blob = new PooledArraySegment<byte>(_blobTemplate.Length))
            {
                Buffer.BlockCopy(_blobTemplate, 0, blob.Array, 0, _blobTemplate.Length);

                // inject extranonce (big-endian at the beginning of the reserved area of the blob)
                var extraNonceBytes = BitConverter.GetBytes(workerExtraNonce.ToBigEndian());
                Buffer.BlockCopy(extraNonceBytes, 0, blob.Array, (int)BlockTemplate.ReservedOffset, extraNonceBytes.Length);

                return LibCryptonote.ConvertBlob(blob.Array, _blobTemplate.Length).ToHexString();
            }
        }

        private string EncodeTarget(double difficulty)
        {
            var diff = BigInteger.ValueOf((long)(difficulty * 255d));
            var quotient = MoneroConstants.Diff1.Divide(diff).Multiply(BigInteger.ValueOf(255));
            var bytes = quotient.ToByteArray();
            var padded = Enumerable.Repeat((byte)0, 32).ToArray();

            if (padded.Length - bytes.Length > 0)
                Buffer.BlockCopy(bytes, 0, padded, padded.Length - bytes.Length, bytes.Length);

            var result = new ArraySegment<byte>(padded, 0, 4).Reverse().ToHexString();

            return result;
        }
    }
}
