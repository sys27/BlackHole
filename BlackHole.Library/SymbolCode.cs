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
        private byte lenght;

        public SymbolCode() { }

        public override bool Equals(object obj)
        {
            var s = obj as SymbolCode;
            if (s == null)
                return false;

            return bits == s.bits && lenght == s.lenght;
        }

        public override string ToString()
        {
            return string.Format("Bits: {0}, Length: {1}", Convert.ToString(bits, 2), lenght);
        }

        public void SetBit(byte bit)
        {
            bits |= (uint)(bit << lenght);
            lenght++;
        }

        public void RemoveLastBit()
        {
            if (lenght <= 0)
                // todo: exception
                throw new Exception();

            lenght--;
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
                return lenght;
            }
            set
            {
                lenght = value;
            }
        }

    }

}
