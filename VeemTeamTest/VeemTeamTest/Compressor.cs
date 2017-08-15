using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using VeemTeamTest.Common;

namespace VeemTeamTest
{
    class Compressor
    {
        /// <summary>
        /// Создаем поток
        /// </summary>
        private readonly Thread _thread;

        /// <summary>
        /// Создаем интерфейс консоли
        /// </summary>
        private readonly IConsoleSpinner _spinner;

        /// <summary>
        /// Входной файл
        /// </summary>
        protected string TargetFile { get; set; }

        /// <summary>
        /// Выходной файл
        /// </summary>
        protected string Archive { get; set; }

        public Compressor(string file, string archive, IConsoleSpinner spinner)
        {
            TargetFile = file;
            Archive = archive;
            _thread = new Thread(Compress);
            _spinner = spinner;
        }

        /// <summary>
        /// Метод проверки статуса выполнения текущего потока.
        /// Если не выполняется текущий поток, стартуем поток.
        /// </summary>
        public void Run()
        {
            if (!_thread.IsAlive)
            {
                _thread.Start();
            }
        }

        /// <summary>
        /// Метод чтения и записи в архивный файл, используя буффер. 
        /// lock - используется для многопоточной безопасности
        /// </summary>
        public void Compress()
        {
            lock (TargetFile) lock (Archive)
                {
                    _spinner.Start();

                    try
                    {
                        var buffer = new byte[1024 * 64];
                        using (var archive = new FileStream(Archive, FileMode.Create, FileAccess.Write))
                        {
                            using (var compressing = new GZipStream(archive, CompressionMode.Compress))
                            {
                                using (var file = new FileStream(TargetFile, FileMode.Open, FileAccess.Read))
                                {
                                    var bytesRead = file.Read(buffer, 0, buffer.Length);
                                    while (bytesRead != 0)
                                    {
                                        compressing.Write(buffer, 0, buffer.Length);
                                        bytesRead = file.Read(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                        }
                    }
                    catch (InvalidDataException)
                    {
                        Console.WriteLine("Error: The file being read contains invalid data.");
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Error:The file specified was not found.");
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Error: path is a zero-length string, contains only white space, or contains one or more invalid characters");
                    }
                    catch (PathTooLongException)
                    {
                        Console.WriteLine("Error: The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.");
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Console.WriteLine("Error: The specified path is invalid, such as being on an unmapped drive.");
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Error: An I/O error occurred while opening the file.");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Error: path specified a file that is read-only, the path is a directory, or caller does not have the required permissions.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Error: You must provide parameters for MyGZIP.");
                    }

                    _spinner.Stop();
                    Console.WriteLine("Info: compress completed, result in {0}", Archive);
                }
        }
    }
}
