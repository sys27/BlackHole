using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class HuffmanCode
    {

        private const int BUFFER_SIZE = 65536;

        public HuffmanCode() { }

        private HuffmanNode BuildTree(SortedSet<HuffmanNode> sortedWeights)
        {
            while (sortedWeights.Count > 1)
            {
                var firstMin = sortedWeights.Min;
                sortedWeights.Remove(firstMin);
                var secondMin = sortedWeights.Min;
                sortedWeights.Remove(secondMin);

                sortedWeights.Add(new HuffmanNode(firstMin, secondMin));
            }

            return sortedWeights.First();
        }

        private SymbolCode[] GetCodes(HuffmanNode root, HuffmanNode[] symbols)
        {
            var codes = new SymbolCode[256];

            foreach (var symbol in symbols)
            {
                var current = symbol;
                var code = new SymbolCode();

                while (current != root)
                {
                    code.SetBit(current.Bit);

                    current = current.Parent;
                }

                codes[symbol.Symbol] = code;
            }

            return codes;
        }

        public IEnumerable<SymbolCode> GetCodes(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.CanRead || !input.CanSeek)
                // todo: exception
                throw new InvalidOperationException();

            var weights = new long[256];
            var buf = new byte[BUFFER_SIZE];
            int count;
            while ((count = input.Read(buf, 0, buf.Length)) > 0)
                for (int i = 0; i < count; i++)
                    weights[buf[i]]++;

            var sortedWeights = new SortedSet<HuffmanNode>();
            for (int i = 0; i < weights.Length; i++)
                if (weights[i] > 0)
                    sortedWeights.Add(new HuffmanNode((short)i, weights[i]));
            var symbols = sortedWeights.ToArray();
            var root = BuildTree(sortedWeights);

            return GetCodes(root, symbols);
        }

        public long Compress(Stream input, Stream output, IEnumerable<SymbolCode> codes)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (!input.CanRead || !input.CanSeek)
                // todo: exception
                throw new InvalidOperationException();
            if (!output.CanWrite)
                // todo: exception
                throw new InvalidOperationException();

            var buf = new byte[BUFFER_SIZE];
            int count;

            input.Position = 0;
            var bitOutput = new BitStream(output);
            long allBitsLength = 0;
            while ((count = input.Read(buf, 0, buf.Length)) > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var code = codes.ElementAt(buf[i]);
                    bitOutput.WriteBits(code.Bits, code.Length);
                    allBitsLength += code.Length;
                }
            }
            bitOutput.Flush();

            return allBitsLength;
        }

        public void Decompress(Stream input, Stream output)
        {

        }

    }

}
