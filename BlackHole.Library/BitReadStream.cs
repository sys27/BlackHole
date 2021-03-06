﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class BitReadStream : Stream
    {

        private int BUFFER_SIZE = 65536;
        //private const int MAX_BITS = BUFFER_SIZE * 8;

        private Stream stream;
        private byte[] buf;
        private int count;
        private int currentIndex;
        private sbyte leftBits;
        private long bitsLength;
        private long currentBitsLength;

        public BitReadStream(Stream stream, long bitsLength)
        {
            this.stream = stream;

            this.leftBits = 7;
            this.bitsLength = bitsLength;
        }

        public byte? ReadBit()
        {
            if (currentBitsLength >= bitsLength)
                return null;

            if (buf == null)
            {
                buf = new byte[BUFFER_SIZE];
                count = Read(buf, 0, buf.Length);

                if (count == 0)
                    return null;
            }

            var b = buf[currentIndex];
            var result = (byte)((b >> leftBits) & 1);
            leftBits--;

            if (leftBits < 0)
            {
                leftBits = 7;
                currentIndex++;

                if (currentIndex >= count && currentBitsLength < bitsLength)
                {
                    count = Read(buf, 0, buf.Length);
                    currentIndex = 0;
                }
            }

            currentBitsLength++;
            return result;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buf, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
                return false;
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

        public int BufferSize
        {
            get
            {
                return BUFFER_SIZE;
            }
            set
            {
                BUFFER_SIZE = value;
            }
        }

    }

}
