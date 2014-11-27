using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class Archiver
    {

        private HuffmanCode huffman;

        public Archiver() : this(65536) { }

        public Archiver(int bufferSize)
        {
            huffman = new HuffmanCode(bufferSize);
        }

        public IEnumerable<SymbolCode> GetCodes(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            using (var input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return GetCodes(input);
        }

        public IEnumerable<SymbolCode> GetCodes(Stream input)
        {
            return huffman.GetCodes(input);
        }

        public Archive ReadArchiveInfo(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            using (var input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadArchiveInfo(input);
        }

        public Archive ReadArchiveInfo(Stream input)
        {
            using (var br = new BinaryReader(input, new UTF8Encoding(), true))
            {
                int filesCount = br.ReadInt32();
                var archive = new Archive();

                for (int i = 0; i < filesCount; i++)
                {
                    var name = br.ReadString();
                    var originalSize = br.ReadInt64();
                    var bitsLength = br.ReadInt64();
                    var offset = br.ReadInt64();

                    var codesCount = br.ReadInt16();
                    var codes = new SymbolCode[256];
                    for (int j = 0; j < codesCount; j++)
                    {
                        var symbol = br.ReadByte();
                        var bits = br.ReadUInt32();
                        var length = br.ReadByte();

                        codes[symbol] = new SymbolCode { Bits = bits, Length = length };
                    }

                    archive.Add(new ArchivedFile(name, originalSize, bitsLength, offset, codes));
                }

                return archive;
            }
        }

        public async Task CreateAsync(string[] inputFiles, string outputFile, CancellationTokenSource tokenSource)
        {
            if (inputFiles == null)
                throw new ArgumentNullException("inputFile");
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentNullException("outputFile");

            using (var output = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                await CreateAsync(inputFiles, output, tokenSource);
        }

        public async Task CreateAsync(string[] inputFiles, Stream output, CancellationTokenSource tokenSource)
        {
            if (inputFiles == null || inputFiles.Length == 0)
                throw new ArgumentNullException("inputFile");
            if (output == null)
                throw new ArgumentNullException("output");

            await Task.Run(async () =>
            {
                var token = tokenSource.Token;
                token.ThrowIfCancellationRequested();

                using (var bw = new BinaryWriter(output, new UTF8Encoding(), true))
                {
                    var archive = new Archive();
                    var allCodes = new IEnumerable<SymbolCode>[inputFiles.Length];
                    var beginPosition = output.Position;
                    bw.Seek(4, SeekOrigin.Current);

                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        var inputFile = inputFiles[i];
                        using (var input = File.OpenRead(inputFile))
                        {
                            var codes = huffman.GetCodes(input).ToArray();
                            allCodes[i] = codes;

                            var originalSize = new FileInfo(inputFile).Length;
                            var file = new ArchivedFile(Path.GetFileName(inputFile), originalSize, 0, 0, codes);

                            bw.Write(file.Name);
                            bw.Write(file.OriginalSize);

                            file.InfoPosition = output.Position;
                            bw.Seek(16, SeekOrigin.Current);

                            var codesPosition = output.Position;
                            bw.Seek(2, SeekOrigin.Current);
                            short codesCount = 0;
                            for (int j = 0; j < codes.Length; j++)
                            {
                                if (codes[j] != null)
                                {
                                    bw.Write((byte)j);
                                    bw.Write(codes[j].Bits);
                                    bw.Write(codes[j].Length);
                                    codesCount++;
                                }
                            }
                            var originalPosition = output.Position;
                            output.Position = codesPosition;
                            bw.Write(codesCount);
                            output.Position = originalPosition;

                            archive.Add(file);
                        }
                    }

                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        var inputFile = inputFiles[i];
                        using (var input = File.OpenRead(inputFile))
                        {
                            var codes = allCodes[i];
                            var file = archive[i];

                            var offset = output.Position;
                            var bitsLength = await huffman.CompressAsync(input, output, codes, tokenSource);
                            var compressedPosition = output.Position;
                            var lastArchive = archive.LastOrDefault();
                            file.BitsLength = bitsLength;
                            file.Offset = offset;

                            output.Position = file.InfoPosition;
                            bw.Write(file.BitsLength);
                            bw.Write(file.Offset);
                            output.Position = compressedPosition;
                        }
                    }

                    var endPosition = output.Position;
                    output.Position = beginPosition;
                    bw.Write(archive.FilesCount);
                    output.Position = endPosition;
                }
            }, tokenSource.Token);
        }

        private async Task ExtractFileAsync(Stream input, ArchivedFile file, string folder, CancellationTokenSource tokenSource)
        {
            var root = huffman.BuildTree(file.Codes);
            using (var output = new FileStream(Path.Combine(folder, file.Name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                input.Seek(file.Offset, SeekOrigin.Begin);
                await huffman.DecompressAsync(input, output, file.BitsLength, root, tokenSource);
            }
        }

        public async Task ExtractAsync(string inputFile, string fileName, string folder, CancellationTokenSource tokenSource)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException("inputFile");

            using (var input = File.OpenRead(inputFile))
                await ExtractAsync(input, fileName, folder, tokenSource);
        }

        public async Task ExtractAsync(Stream input, string fileName, string folder, CancellationTokenSource tokenSource)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            await Task.Run(async () =>
            {
                var archive = ReadArchiveInfo(input);

                var file = archive.FirstOrDefault(f => f.Name == fileName);
                if (file == null)
                    throw new FileNotFoundException();

                await ExtractFileAsync(input, file, folder, tokenSource);
            });
        }

        public async Task ExtractAllAsync(string inputFile, string folder, CancellationTokenSource tokenSource)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException("inputFile");

            using (var input = File.OpenRead(inputFile))
                await ExtractAllAsync(input, folder, tokenSource);
        }

        public async Task ExtractAllAsync(Stream input, string folder, CancellationTokenSource tokenSource)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            await Task.Run(async () =>
            {
                var archive = ReadArchiveInfo(input);

                foreach (var file in archive)
                    await ExtractFileAsync(input, file, folder, tokenSource);
            }, tokenSource.Token);
        }

        public HuffmanCode HuffmanCode
        {
            get
            {
                return huffman;
            }
        }

    }

}
