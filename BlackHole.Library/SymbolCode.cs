using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class SymbolCode
    {

        private uint bits;
        private byte length;

        public SymbolCode() { }

        public override bool Equals(object obj)
        {
            var s = obj as SymbolCode;
            if (s == null)
                return false;

            return bits == s.bits && length == s.length;
        }

        public override string ToString()
        {
            return string.Format("Bits: {0}, Length: {1}", Convert.ToString(bits, 2).PadLeft(length, '0'), length);
        }

        public void SetBit(byte bit)
        {
            bits |= (uint)(bit << length);
            length++;
        }

        public void RemoveLastBit()
        {
            if (length <= 0)
                throw new IndexOutOfRangeException();

            length--;
        }

        public uint Bits
        {
            get
            {
                return bits;
            }
            set
            {
                bits = value;
            }
        }

        public byte Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

    }

}
