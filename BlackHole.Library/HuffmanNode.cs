using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class HuffmanNode : IComparable<HuffmanNode>
    {

        private short symbol;
        private long weight;
        private byte bit;
        private HuffmanNode parent;
        private HuffmanNode left;
        private HuffmanNode right;

        internal HuffmanNode() { }

        public HuffmanNode(long weight)
            : this(-1, weight) { }

        public HuffmanNode(short symbol, long weight)
        {
            this.symbol = symbol;
            this.weight = weight;
        }

        public HuffmanNode(HuffmanNode left, HuffmanNode right)
        {
            this.symbol = (short)(left.symbol + right.symbol);
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
            return string.Format("{0}: {1}, Weight: {2}", symbol, (char)symbol, weight);
        }

        public int CompareTo(HuffmanNode other)
        {
            var result = weight.CompareTo(other.weight);
            if (result != 0)
                return result;

            return symbol.CompareTo(other.symbol);
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
