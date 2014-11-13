﻿using System;
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

            using (var bw = new BinaryWriter(output, new UTF8Encoding(), true))
            {
                var archive = new Archive();
                var allCodes = new IEnumerable<SymbolCode>[inputFiles.Length];
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

        private Archive ReadArchiveInfo(Stream input)
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

                    var file = new ArchivedFile(name, originalSize, bitsLength, offset, codes);
                    archive.Add(file);
                }

                return archive;
            }
        }

        private void ExtractFile(Stream input, ArchivedFile file, string folder)
        {
            var root = huffman.BuildTree(file.Codes);
            using (var output = new FileStream(Path.Combine(folder, file.Name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                input.Seek(file.Offset, SeekOrigin.Begin);
                huffman.Decompress(input, output, file.BitsLength, root);
            }
        }

        public void Extract(string inputFile, string fileName, string folder)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException("inputFile");

            using (var input = File.OpenRead(inputFile))
                Extract(input, fileName, folder);
        }

        public void Extract(Stream input, string fileName, string folder)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var archive = ReadArchiveInfo(input);

            var file = archive.Where(f => f.Name == fileName).FirstOrDefault();
            if (file == null)
                // todo: exception
                throw new Exception();

            ExtractFile(input, file, folder);
        }

        public void ExtractAll(string inputFile, string folder)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException("inputFile");

            using (var input = File.OpenRead(inputFile))
                ExtractAll(input, folder);
        }

        public void ExtractAll(Stream input, string folder)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var archive = ReadArchiveInfo(input);

            foreach (var file in archive)
                ExtractFile(input, file, folder);
        }

    }

}
