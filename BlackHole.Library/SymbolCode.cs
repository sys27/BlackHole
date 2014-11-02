using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class SymbolCode
    {

        private ushort bits;
        private byte lenght;

        public SymbolCode() { }

        public override string ToString()
        {
            return string.Format("Bits: {0}, Length: {1}", Convert.ToString(bits, 2), lenght);
        }

        public void SetBit(byte bit)
        {
            bits |= (ushort)(bit << lenght);
            lenght++;
        }

        public void RemoveLastBit()
        {
            if (lenght <= 0)
                // todo: exception
                throw new Exception();

            lenght--;
        }

        public ushort Bits
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
