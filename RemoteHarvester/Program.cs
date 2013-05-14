using CommandLine;
using CommandLine.Text;
using PeachFarmerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class Program
    {
        static void Main(string[] args)
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

                if (options.Password != null && options.ServerCertFile == null)
                {
                    ShowPlaintextPasswordWarning();
                }

                CommandProcessor processor = new CommandProcessor(options);
                processor.MonitorFolder();
            }
        }

        private static void ShowPlaintextPasswordWarning()
        {
            Console.WriteLine("Warning: Server password is required but SSL is not enabled. Passwords will be sent in plaintext.\r\n" +
                              "Use the --server-cert parameter to enable SSL so that passwords are sent in an encrypted SSL tunnel.");
            Console.WriteLine();
        }
    }
}
