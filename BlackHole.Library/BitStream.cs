﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class BitStream : Stream
    {

        private const int BUFFER_SIZE = 65536;
        private const int MAX_BITS = BUFFER_SIZE * 8;

        private Stream stream;
        private byte[] buf;
        // todo: to long or ulong
        private int bitLength;

        public BitStream(Stream stream)
        {
            this.stream = stream;

            buf = new byte[BUFFER_SIZE];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public void WriteBits(int bits, byte length)
        {
            var occupBits = bitLength % 8;
            var leftBits = 8 - occupBits;
            var index = bitLength / 8;

            if (leftBits >= length)
            {
                buf[index] |= (byte)(bits << (leftBits - length));
            }
            else
            {
                var newLength = length - leftBits;
                buf[index] |= (byte)(bits >> newLength);
                index++;
                if (index >= buf.Length)
                {
                    Flush();
                    index = 0;
                }

                buf[index] |= (byte)(bits << (8 - newLength));
            }

            bitLength += length;
            if (bitLength == MAX_BITS)
                Flush();
        }

        public override void Flush()
        {
            Write(buf, 0, bitLength / 8);
            stream.Flush();
            bitLength = 0;
        }

        public override bool CanRead
        {
            get
            {
                return stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        public int BitLength
        {
            get
            {
                return bitLength;
            }
        }

    }

}
