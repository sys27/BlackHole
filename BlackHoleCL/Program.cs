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
            //if (args.Length == 0)
            //{
            //    PrintHelp();
            //    return;
            //}

            //var command = args[0];
            //if (command == "-h" || command == "-?")
            //{
            //    PrintHelp();
            //}
            //else if (command == "-c" && args.Length >= 2)
            //{

            //}
            //else if (command == "-d" && args.Length >= 2)
            //{

            //}
            //else
            //{
            //    Console.WriteLine();
            //    PrintHelp();
            //}

            var a = new Archiver();
            a.Create(new[] { "a.jpg", "b.fb2", "c.txt" }, "!.bh");
            a.ExtractAll("!.bh", @"a\");
        }

    }

}
