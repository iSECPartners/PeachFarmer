using PeachFarmerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Program(args);
        }

        private Program(String[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                string usageError;
                if (!options.Validate(out usageError))
                {
                    Console.WriteLine(options.GetUsage());
                    Console.WriteLine("Error: " + usageError);
                    return;
                }

                CommandProcessor processor = new CommandProcessor(options);
                processor.PullFilesFromWorkers();
            }
        }
    }
}
