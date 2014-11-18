using BlackHole.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackHoleCL
{

    class Program
    {

        private static void PrintHelp()
        {
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
                Console.WriteLine("Недостатня кількість аргументів.\n");
                PrintHelp();
                return;
            }

            var command = args[0];
            if (command == "-h" || command == "/h" || command == "-?" || command == "/?")
            {
                PrintHelp();
            }
            else if ((command == "-c" || command == "/c") && args.Length >= 2)
            {
                var archiver = new Archiver();

                var outputFile = "output.bh";
                var files = new List<string>();

                for (int i = 1; i < args.Length; i++)
                {
                    var item = args[i];
                    if (item == "-o")
                    {
                        if (i + 1 < args.Length)
                        {
                            outputFile = args[i + 1];
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Введено неправильну кількість аргументів.");
                            return;
                        }
                    }
                    else
                    {
                        files.Add(item);
                    }
                }

                archiver.CreateAsync(files.ToArray(), outputFile, new CancellationTokenSource()).GetAwaiter().GetResult();
            }
            else if ((command == "-d" || command == "/d") && args.Length >= 2)
            {
                var archiver = new Archiver();

                var inputFile = args[1];
                var outputFolder = string.Empty;

                if (args.Length > 2 && args[2] == "-o")
                {
                    if (args.Length > 3)
                    {
                        outputFolder = args[3];
                    }
                    else
                    {
                        Console.WriteLine("Введено неправильну кількість аргументів.");
                        return;
                    }
                }

                archiver.ExtractAllAsync(inputFile, outputFolder, new CancellationTokenSource()).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Введено неправильні аргументи.\n");
                PrintHelp();
            }
        }

    }

}
