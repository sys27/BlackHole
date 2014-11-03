﻿using System;
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

            if (symbols.Length == 1)
            {
                var symbol = symbols[0];
                codes[symbol.Symbol] = new SymbolCode { Bits = 0, Length = 1 };
            }
            else
            {
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

        public HuffmanNode BuildTree(IEnumerable<SymbolCode> codes)
        {
            var root = new HuffmanNode();
            var codesArr = codes.ToArray();

            for (short i = 0; i < codesArr.Length; i++)
            {
                var code = codesArr[i];
                if (code != null)
                {
                    var current = root;
                    for (var check = 1 << code.Length - 1; check != 0; check >>= 1)
                    {
                        if ((code.Bits & check) != 0)
                        {
                            if (check != 1)
                            {
                                if (current.Right == null)
                                    current.Right = new HuffmanNode { Parent = current };

                                current = current.Right;
                            }
                            else
                            {
                                current.Right = new HuffmanNode { Parent = current, Symbol = i };
                            }
                        }
                        else
                        {
                            if (check != 1)
                            {
                                if (current.Left == null)
                                    current.Left = new HuffmanNode { Parent = current };

                                current = current.Left;
                            }
                            else
                            {
                                current.Left = new HuffmanNode { Parent = current, Symbol = i };
                            }
                        }
                    }
                }
            }

            return root;
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
            var bitOutput = new BitWriteStream(output);
            long allBitsLength = 0;
            while ((count = input.Read(buf, 0, buf.Length)) > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var code = codes.ElementAt(buf[i]);
                    bitOutput.WriteBits(code);
                    allBitsLength += code.Length;
                }
            }
            bitOutput.Flush();

            return allBitsLength;
        }

        public void Decompress(Stream input, Stream output, IEnumerable<SymbolCode> codes)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (codes == null || codes.Count() == 0)
                throw new ArgumentNullException("codes");

            var bitReader = new BitReadStream(input);
            byte? b = null;
            while ((b = bitReader.ReadBit()) != null)
            {
                Console.WriteLine(b);
            }
        }

    }

}
