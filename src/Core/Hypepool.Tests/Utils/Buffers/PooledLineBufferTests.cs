// original code from: https://github.com/coinfoundry/miningcore/blob/dev/src/MiningCore.Tests/Util/PooledLineBufferTests.cs

using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Hypepool.Common.Utils.Buffers;
using Hypepool.Core.Internals.Logging;
using Hypepool.Core.Utils.Buffers;
using Xunit;

namespace Hypepool.Tests.Utils.Buffers
{
    public class PooledLineBufferTests
    {
        [Fact]
        public void PooledLineBuffer_Partial_Line()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;

            var buf = GetBuffer("deadbeef");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public void PooledLineBuffer_Partial_Line_Double()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;

            var buf = GetBuffer("dead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            buf = GetBuffer("beef");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public void PooledLineBuffer_Partial_Line_Double_With_NewLine()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;
            var result = string.Empty;

            var buf = GetBuffer("dead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            buf = GetBuffer("beef\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                    result = GetString(x);
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            result.ShouldBeEquivalentTo("deadbeef");
        }

        [Fact]
        public void PooledLineBuffer_Partial_Line_Double_With_NewLine_With_Leading_NewLines()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;
            var result = string.Empty;

            var buf = GetBuffer("\n\ndead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            buf = GetBuffer("beef\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                    result = GetString(x);
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            result.ShouldBeEquivalentTo("deadbeef");
        }

        [Fact]
        public void PooledLineBuffer_Partial_Line_Double_With_NewLine_With_Trailing_NewLines()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;
            var result = string.Empty;

            var buf = GetBuffer("dead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) => recvCount++, (ex) => errCount++);

            buf = GetBuffer("beef\n\n\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                    result = GetString(x);
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            result.ShouldBeEquivalentTo("deadbeef");
        }

        [Fact]
        public void PooledLineBuffer_Partial_Dont_Emit_Empty_Lines()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;

            var buf = GetBuffer("\n\n\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public void PooledLineBuffer_Partial_Enforce_Limits()
        {
            var plb = new PooledLineBuffer(3);
            var recvCount = 0;
            var errCount = 0;

            var buf = GetBuffer("dead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void PooledLineBuffer_Partial_Enforce_Limits_Queued()
        {
            var plb = new PooledLineBuffer(5);
            var recvCount = 0;
            var errCount = 0;

            var buf = GetBuffer("dead");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(0);

            buf = GetBuffer("bee");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(0);
            errCount.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void PooledLineBuffer_Single_Line()
        {
            var plb = new PooledLineBuffer();
            var recvCount = 0;
            var errCount = 0;
            var result = string.Empty;

            var buf = GetBuffer("dead\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    recvCount++;
                    result = GetString(x);
                }, (ex) => errCount++);

            recvCount.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            result.ShouldBeEquivalentTo("dead");
        }

        [Fact]
        public void PooledLineBuffer_Multi_Line_Batch()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("dead\nbeef\nc0de\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(3);
            errCount.ShouldBeEquivalentTo(0);

            results[0].ShouldBeEquivalentTo("dead");
            results[1].ShouldBeEquivalentTo("beef");
            results[2].ShouldBeEquivalentTo("c0de");
        }

        [Fact]
        public void PooledLineBuffer_Single_Characters()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("a");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("b");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("c\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            results[0].ShouldBeEquivalentTo("abc");
        }

        [Fact]
        public void PooledLineBuffer_Single_Character_Lines()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("a\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("b\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("c\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(3);
            errCount.ShouldBeEquivalentTo(0);
            results[0].ShouldBeEquivalentTo("a");
            results[1].ShouldBeEquivalentTo("b");
            results[2].ShouldBeEquivalentTo("c");
        }

        [Fact]
        public void PooledLineBuffer_Combo1()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("dead\nbeef");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(1);
            errCount.ShouldBeEquivalentTo(0);
            results[0].ShouldBeEquivalentTo("dead");
        }

        [Fact]
        public void PooledLineBuffer_Combo2()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("dead\nbeef");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("c0de\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(2);
            errCount.ShouldBeEquivalentTo(0);
            results[0].ShouldBeEquivalentTo("dead");
            results[1].ShouldBeEquivalentTo("beefc0de");
        }

        [Fact]
        public void PooledLineBuffer_Combo3()
        {
            var plb = new PooledLineBuffer();
            var errCount = 0;
            var results = new List<string>();

            var buf = GetBuffer("dead\nbeef");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("c0de");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            buf = GetBuffer("feed\nbabe\n");

            plb.Receive(buf, buf.Length,
                (src, dst, count) => Array.Copy(src, 0, dst, 0, count),
                (x) =>
                {
                    results.Add(GetString(x));
                }, (ex) => errCount++);

            results.Count.ShouldBeEquivalentTo(3);
            errCount.ShouldBeEquivalentTo(0);
            results[0].ShouldBeEquivalentTo("dead");
            results[1].ShouldBeEquivalentTo("beefc0defeed");
            results[2].ShouldBeEquivalentTo("babe");
        }

        private byte[] GetBuffer(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private string GetString(PooledArraySegment<byte> seg)
        {
            return Encoding.UTF8.GetString(seg.Array, seg.Offset, seg.Size);
        }
    }
}
