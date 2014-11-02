using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class Archiver
    {

        private HuffmanCode huffman;

        public Archiver()
        {
            huffman = new HuffmanCode();
        }

        public void Create(string[] inputFiles, string outputFile)
        {
            if (inputFiles == null)
                throw new ArgumentNullException("inputFile");
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentNullException("outputFile");

            using (var output = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Create(inputFiles, output);
            }
        }

        public void Create(string[] inputFiles, Stream output)
        {
            if (inputFiles == null || inputFiles.Length == 0)
                throw new ArgumentNullException("inputFile");
            if (output == null)
                throw new ArgumentNullException("output");

            var archive = new Archive();
            var allCodes = new IEnumerable<SymbolCode>[inputFiles.Length];
            var bw = new BinaryWriter(output);
            var beginPosition = output.Position;
            bw.Seek(4, SeekOrigin.Current);

            for (int i = 0; i < inputFiles.Length; i++)
            {
                var inputFile = inputFiles[i];
                using (var input = File.OpenRead(inputFile))
                {
                    var codes = huffman.GetCodes(input).ToArray();
                    allCodes[i] = codes;

                    var originalSize = new FileInfo(inputFile).Length;
                    var file = new ArchivedFile(inputFile, originalSize, 0, 0, codes);

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
                var inputFile = inputFiles[i];
                using (var input = File.OpenRead(inputFile))
                {
                    var codes = allCodes[i];
                    var file = archive[i];

                    var offset = output.Position;
                    var bitsLength = huffman.Compress(input, output, codes);
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

        public void ExtractAll(string inputFile, string folder)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException("inputFile");

            using (var input = File.OpenRead(inputFile))
            {
                ExtractAll(input, folder);
            }
        }

        public void ExtractAll(Stream input, string folder)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var br = new BinaryReader(input);

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
                    var bits = br.ReadUInt16();
                    var length = br.ReadByte();

                    codes[symbol] = new SymbolCode { Bits = bits, Length = length };
                }

                var file = new ArchivedFile(name, originalSize, bitsLength, offset, codes);
                archive.Add(file);
            }

            foreach (var file in archive)
            {
                using (var output = new FileStream(Path.Combine(folder, file.Name), FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    input.Seek(file.Offset, SeekOrigin.Begin);
                    huffman.Decompress(input, output, file.Codes);
                }
            }
        }

    }

}
