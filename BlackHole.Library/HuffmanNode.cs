using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class HuffmanNode /*: IComparable<HuffmanNode>*/
    {

        private bool isSymbol;
        private short symbol;
        private long weight;
        private byte bit;
        private HuffmanNode parent;
        private HuffmanNode left;
        private HuffmanNode right;

        internal HuffmanNode()
        {
            this.isSymbol = false;
            //this.symbol = -1;
        }

        public HuffmanNode(short symbol, long weight)
        {
            this.isSymbol = true;
            this.symbol = symbol;
            this.weight = weight;
        }

        public HuffmanNode(HuffmanNode left, HuffmanNode right)
        {
            this.isSymbol = false;
            //this.symbol = (short)(left.symbol + right.symbol);
            this.weight = left.weight + right.weight;

            this.left = left;
            this.right = right;

            left.parent = this;
            left.bit = 0;
            right.parent = this;
            right.bit = 1;
        }

        public override string ToString()
        {
            if (isSymbol)
                return string.Format("{0}: {1}, Weight: {2}", symbol, (char)symbol, weight);

            return string.Format("Not symbol, Weight: {0}", weight);
        }

        public static int Compare(HuffmanNode x, HuffmanNode y)
        {
            return x.weight.CompareTo(y.weight);
        }

        public bool IsSymbol
        {
            get
            {
                return isSymbol;
            }
            internal set
            {
                isSymbol = value;
            }
        }

        public short Symbol
        {
            get
            {
                return symbol;
            }
            internal set
            {
                symbol = value;
            }
        }

        public long Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        public byte Bit
        {
            get
            {
                return bit;
            }
            set
            {
                bit = value;
            }
        }

        public HuffmanNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public HuffmanNode Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        public HuffmanNode Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

    }

}
