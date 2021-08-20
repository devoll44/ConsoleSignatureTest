using ConsoleSignature.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSignature
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int blockSize = 0;
                if (args == null || args.Length < 2 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]) || !int.TryParse(args[1], out blockSize))
                {
                    throw new ArgumentException("Wrong arguments");
                }

                using (FileSignatureService service = new FileSignatureService(blockSize, args[0]))
                {
                    int threadPerCore = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["ThreadCountPerCore"], out threadPerCore) && threadPerCore > 0)
                    {
                        //умножаем число процессоров на указанное число потоков минус основной поток
                        service.ThreadCount = Environment.ProcessorCount * threadPerCore - 1;
                    }
                    service.PrintFileSignatures();
                }

            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.StackTrace);
            }
            Console.ReadKey();
        }
    }
}
