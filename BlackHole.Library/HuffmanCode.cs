using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class HuffmanCode
    {

        private int BUFFER_SIZE;

        public HuffmanCode() : this(65536) { }

        public HuffmanCode(int bufferSize)
        {
            BUFFER_SIZE = bufferSize;
        }

        private HuffmanNode BuildTree(SortedList<int, HuffmanNode> sortedWeights)
        {
            int i = 256;

            while (sortedWeights.Count > 1)
            {
                var firstMin = sortedWeights.First();
                sortedWeights.Remove(firstMin.Key);
                var secondMin = sortedWeights.First();
                sortedWeights.Remove(secondMin.Key);

                sortedWeights.Add(i, new HuffmanNode(firstMin.Value, secondMin.Value));
                i++;
            }

            return sortedWeights.First().Value;
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
                throw new InvalidOperationException();

            var weights = new long[256];
            var buf = new byte[BUFFER_SIZE];
            int count;
            while ((count = input.Read(buf, 0, buf.Length)) > 0)
                for (int i = 0; i < count; i++)
                    weights[buf[i]]++;

            var sortedWeights = new SortedList<int, HuffmanNode>();
            for (int i = 0; i < weights.Length; i++)
                if (weights[i] > 0)
                    sortedWeights.Add(i, new HuffmanNode((short)i, weights[i]));
            var symbols = sortedWeights.Values.ToArray();
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
                                    current.Right = new HuffmanNode { Parent = current, IsSymbol = false };

                                current = current.Right;
                            }
                            else
                            {
                                current.Right = new HuffmanNode
                                {
                                    Parent = current,
                                    Left = current.Right == null ? null : current.Right.Left,
                                    Right = current.Right == null ? null : current.Right.Right,
                                    Symbol = i,
                                    IsSymbol = true
                                };
                            }
                        }
                        else
                        {
                            if (check != 1)
                            {
                                if (current.Left == null)
                                    current.Left = new HuffmanNode { Parent = current, IsSymbol = false };

                                current = current.Left;
                            }
                            else
                            {
                                current.Left = new HuffmanNode
                                {
                                    Parent = current,
                                    Left = current.Left == null ? null : current.Left.Left,
                                    Right = current.Left == null ? null : current.Left.Right,
                                    Symbol = i,
                                    IsSymbol = true
                                };
                            }
                        }
                    }
                }
            }

            return root;
        }

        private long CompressInternal(Stream input, Stream output, IEnumerable<SymbolCode> codes, CancellationTokenSource tokenSource)
        {
            if (tokenSource != null)
                tokenSource.Token.ThrowIfCancellationRequested();

            var buf = new byte[BUFFER_SIZE];
            int count;

            long index = 0;

            input.Position = 0;
            var bitOutput = new BitWriteStream(output) { BufferSize = BUFFER_SIZE };
            long allBitsLength = 0;
            while ((count = input.Read(buf, 0, buf.Length)) > 0)
            {
                if (tokenSource != null)
                    tokenSource.Token.ThrowIfCancellationRequested();

                for (int i = 0; i < count; i++)
                {
                    var code = codes.ElementAt(buf[i]);
                    bitOutput.WriteBits(code);
                    allBitsLength += code.Length;

                    index++;
                }
            }
            bitOutput.Flush();

            return allBitsLength;
        }

        private void DecompressInternal(Stream input, Stream output, long bitsLength, HuffmanNode root, CancellationTokenSource tokenSource)
        {
            if (tokenSource != null)
                tokenSource.Token.ThrowIfCancellationRequested();

            var bitReader = new BitReadStream(input, bitsLength) { BufferSize = BUFFER_SIZE };

            var buf = new byte[BUFFER_SIZE];
            var index = 0;

            byte? b = null;

            do
            {
                var current = root;
                while (!current.IsSymbol)
                {
                    b = bitReader.ReadBit();
                    if (b == null)
                        break;

                    if (b == 1)
                        current = current.Right;
                    else
                        current = current.Left;
                }

                if (b != null)
                {
                    buf[index] = (byte)current.Symbol;
                    index++;
                }

                if (index >= BUFFER_SIZE)
                {
                    if (tokenSource != null)
                        tokenSource.Token.ThrowIfCancellationRequested();

                    output.Write(buf, 0, buf.Length);
                    buf = new byte[BUFFER_SIZE];
                    index = 0;
                }
            } while (b != null);

            if (index != 0)
                output.Write(buf, 0, index);
        }

        public long Compress(Stream input, Stream output, IEnumerable<SymbolCode> codes)
        {
            return CompressInternal(input, output, codes, null);
        }

        public async Task<long> CompressAsync(Stream input, Stream output, IEnumerable<SymbolCode> codes)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (!input.CanRead || !input.CanSeek)
                throw new InvalidOperationException();
            if (!output.CanWrite)
                throw new InvalidOperationException();

            return await Task.Run<long>(() => CompressInternal(input, output, codes, null));
        }

        public async Task<long> CompressAsync(Stream input, Stream output, IEnumerable<SymbolCode> codes, CancellationTokenSource tokenSource)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (!input.CanRead || !input.CanSeek)
                throw new InvalidOperationException();
            if (!output.CanWrite)
                throw new InvalidOperationException();

            return await Task.Run<long>(() => CompressInternal(input, output, codes, tokenSource), tokenSource.Token);
        }

        public void Decompress(Stream input, Stream output, long bitsLength, HuffmanNode root)
        {
            DecompressInternal(input, output, bitsLength, root, null);
        }

        public async Task DecompressAsync(Stream input, Stream output, long bitsLength, HuffmanNode root)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (root == null)
                throw new ArgumentNullException("root");

            await Task.Run(() => DecompressInternal(input, output, bitsLength, root, null));
        }

        public async Task DecompressAsync(Stream input, Stream output, long bitsLength, HuffmanNode root, CancellationTokenSource tokenSource)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (root == null)
                throw new ArgumentNullException("root");

            await Task.Run(() => DecompressInternal(input, output, bitsLength, root, tokenSource), tokenSource.Token);
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
