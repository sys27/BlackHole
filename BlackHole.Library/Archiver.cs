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

                var codes = huffman.GetCodes(input);
                var bitsLength = huffman.Compress(input, output, codes);

                var file = new ArchivedFile(inputFile, new FileInfo(inputFile).Length, bitsLength, 0, codes);
            }
        }

    }

}
