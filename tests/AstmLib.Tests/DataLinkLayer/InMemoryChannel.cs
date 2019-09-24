using System;
using System.Collections.Generic;
using AstmLib.DataLinkLayer;

namespace AstmLib.Tests.DataLinkLayer
{
    public class InMemoryChannel : IAstmChannel
    {
        private readonly Queue<byte> _in;
        private readonly Queue<byte> _out;

        public InMemoryChannel(Queue<byte> @in, Queue<byte> @out)
        {
            _in = @in;
            _out = @out;
        }

        public bool Reopen(int millisecondsTimeout = 100)
        {
            return true;
        }

        public bool IsInFaultState => false;
        public int ReadTimeout { get; set; }

        public byte ReadByte()
        {
            if (_in.Count > 0)
                return _in.Dequeue();
            throw new TimeoutException();
        }

        public void WriteByte(byte b)
        {
            _out.Enqueue(b);
        }

        public void Write(byte[] buf, int offset, int length)
        {
            foreach (var b in buf)
            {
                _out.Enqueue(b);
            }
        }
    }
}