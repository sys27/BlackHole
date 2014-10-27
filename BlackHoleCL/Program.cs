using BlackHole.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHoleCL
{

    class Program
    {

        private static void PrintHelp()
        {
            Console.WriteLine("Недостатня кількість аргументів.");
            Console.WriteLine("Використання BlackHole <параметри> <список файлів>");

            Console.WriteLine("Параметри:");
            Console.WriteLine("-c: Стистути");
            Console.WriteLine("-d: Розпакувати");
            Console.WriteLine("-o: Вихідний каталог/архів");
            Console.WriteLine("-h, -?: Допомога");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            var command = args[0];
            if (command == "-h" || command == "-?")
            {
                PrintHelp();
            }
            else if (command == "-c" && args.Length >= 2)
            {

            }
            else if (command == "-d" && args.Length >= 2)
            {

            }
            else
            {
                Console.WriteLine();
                PrintHelp();
            }

            //using (var input = new MemoryStream(Encoding.ASCII.GetBytes("AAAAAAAAAABBBBBBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCDDDDDEEEEEEEEEEEEEEEEEEEEEEEEEFFFFFFFFFF")))
            //{
            //    using (var output = new MemoryStream())
            //    {
            //        var hc = new HuffmanCode();
            //        hc.Compress(input, output);

            //        var arr = output.ToArray();
            //    }
            //}

            //using (var input = File.OpenRead("a.fb2"))
            //{
            //    using (var output = File.OpenWrite("a.fb2.bh"))
            //    {
            //        var hc = new HuffmanCode();
            //        hc.Compress(input, output);
            //    }
            //}
        }

    }

}
