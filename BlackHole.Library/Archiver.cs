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
            Create(inputFiles, new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None));
        }

        public void Create(string[] inputFiles, Stream output)
        {
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
                    bw.Seek(1, SeekOrigin.Current);
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

    }

}
