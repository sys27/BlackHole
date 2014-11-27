using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class BitWriteStream : Stream
    {

        private int BUFFER_SIZE = 65536;
        private int MAX_BITS = 65536 * 8;

        private Stream stream;
        private byte[] buf;
        // todo: to long or ulong
        private int bitLength;

        public BitWriteStream(Stream stream)
        {
            this.stream = stream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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

        public void WriteBits(SymbolCode code)
        {
            WriteBits(code.Bits, code.Length);
        }

        public void WriteBits(uint bits, byte length)
        {
            if (buf == null)
                buf = new byte[BUFFER_SIZE];

            byte occupBits = (byte)(bitLength % 8);
            byte leftBits = (byte)(8 - occupBits);
            var index = bitLength / 8;

            if (leftBits >= length)
            {
                buf[index] |= (byte)(bits << (leftBits - length));
                bitLength += length;
            }
            else
            {
                var shift = length - leftBits;
                short tempLength = length;
                while (true)
                {
                    if (tempLength > leftBits)
                    {
                        buf[index] |= (byte)((bits >> shift) & (ushort.MaxValue >> (sizeof(short) * 8 - leftBits)));
                        bitLength += leftBits;
                    }
                    else
                    {
                        buf[index] |= (byte)(bits << shift);
                        bitLength += 8 - shift;
                    }

                    tempLength -= leftBits;
                    if (tempLength <= 0)
                        break;

                    index++;
                    if (index >= buf.Length)
                    {
                        Flush();
                        index = 0;
                    }

                    leftBits = 8;
                    if (shift > 8)
                        shift -= leftBits;
                    else
                        shift = 8 - shift;
                }
            }

            if (bitLength >= MAX_BITS)
                Flush();
        }

        public override void Flush()
        {
            Write(buf, 0, (int)Math.Ceiling(bitLength / 8.0));
            stream.Flush();
            buf = new byte[BUFFER_SIZE];
            bitLength = 0;
        }

        public override bool CanRead
        {
            get
            {
                return false;
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

        public int BufferSize
        {
            get
            {
                return BUFFER_SIZE;
            }
            set
            {
                BUFFER_SIZE = value;
                MAX_BITS = BUFFER_SIZE * 8;
            }
        }

    }

}
