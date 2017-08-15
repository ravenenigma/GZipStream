using System;
using System.IO;
using System.Threading;
using VeemTeamTest.Common;

namespace VeemTeamTest
{
    class Program
    {
        public static bool Terminate { get; private set; }

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)  //срабатывает при нажатии Ctrl+C
            {
                e.Cancel = true;
                Terminate = true;
            };

            var cmd = "";
            var file = "";
            var archive = "";
            var valid = false;

            // Проверяем количество аргументов cmd если нормально то идем дальше
            if (args.Length == 0)
            {
                Console.WriteLine("Error: Starting args not found");
            }
            else
            {
                // Если аргумента не хватило выдаем ошибку об этом
                try
                {
                    cmd = args[0];

                    // Если 2-й аргумент не файл то выдаем ошибку об этом
                    file = args[1];
                    if (!File.Exists(file))
                    {
                        Console.WriteLine("Error: Target file not found");
                    }

                    archive = args[2];
                    if (!archive.Contains(".gz"))
                    {
                        archive += ".gz";
                    }

                    valid = true;
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine("Error: Starting args not enought\nMessage: {0}", e.Message);
                    valid = false;
                }
            }

            if (valid)
            {
                switch (cmd)
                {
                    case "compress":
                        Console.WriteLine("Info: Begin {0} operation", cmd);
                        Console.WriteLine("Info: Target {0} => {1}", file, archive);
                        try
                        {
                            var compressor = new Compressor(file, archive, new ConsoleSpinner());
                            compressor.Run();
                        }
                        catch (ArgumentNullException e)     //Ошибка нулевого аргумента
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (ThreadStateException e)      //Ошибка статуса потока
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (OutOfMemoryException e)      //Ошибка при недостаточной памяти
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "decompress":
                        Console.WriteLine("Info: Begin {0} operation", cmd);
                        Console.WriteLine("Info: Target {0} => {1}", archive, file);
                        try
                        {
                            var decompressor = new Decompressor(file, archive, new ConsoleSpinner());
                            decompressor.Run();
                        }
                        catch (ArgumentNullException e)     //Ошибка нулевого аргумента
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (ThreadStateException e)      //Ошибка статуса потока
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (OutOfMemoryException e)      //Ошибка при недостаточной памяти
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    default:
                        Console.WriteLine("Error: Operation ({0}) not recognized", cmd);
                        break;
                }
            }

            while (!Terminate) { }
            Console.WriteLine("Terminate success");
        }
    }
}
