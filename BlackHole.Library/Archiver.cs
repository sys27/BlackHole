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
            Create(inputFiles, File.OpenWrite(outputFile));
        }

        public void Create(string[] inputFiles, Stream output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            var archive = new Archive();
            foreach (var inputFile in inputFiles)
            {
                var input = File.OpenRead(inputFile);

                // todo: ???
                var codes = huffman.GetCodes(input).ToArray();

                var originalSize = new FileInfo(inputFile).Length;
                var file = new ArchivedFile(inputFile, originalSize, 0, 0, codes);

                var bw = new BinaryWriter(output);
                bw.Write(file.Name);
                bw.Write(file.OriginalSize);

                var fileInfoPosition = output.Position;
                bw.Seek(16, SeekOrigin.Current);

                bw.Write(codes.Length);
                var codesCount = 0;
                for (int i = 0; i < codes.Length; i++)
                {
                    if (codes[i] != null)
                    {
                        bw.Write((byte)i);
                        bw.Write(codes[i].Bits);
                        bw.Write(codes[i].Length);
                        codesCount++;
                    }
                }

                var bitsLength = huffman.Compress(input, output, codes);
                var compressedPosition = output.Position;
                var lastArchive = archive.LastOrDefault();
                var offset = lastArchive != null ? lastArchive.Offset + ((bitsLength / 8) + 1) : 0;
                file.BitsLength = bitsLength;
                file.Offset = offset;

                output.Position = fileInfoPosition;
                bw.Write(file.BitsLength);
                bw.Write(file.Offset);

                output.Position = compressedPosition;
            }
        }

    }

}
