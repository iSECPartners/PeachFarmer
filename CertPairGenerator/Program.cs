using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertPairGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (!options.CreateClientCert && !options.CreateServerCert)
                {
                    Console.WriteLine(options.GetUsage());
                    Console.WriteLine("Error: Must generate either a client or server certificate.");
                    return;
                }

                string outputDirectory;
                try
                {
                    outputDirectory = GetOutputDirectory(options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                DoCreateCertificates(options.CreateServerCert, options.CreateClientCert, outputDirectory);
            }
        }

        private static string GetOutputDirectory(CommandLineOptions options)
        {
            string outputDirectory;

            if (!string.IsNullOrEmpty(options.DestinationFolder))
            {
                outputDirectory = options.DestinationFolder;

                if (!Directory.Exists(outputDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Failed to create output directory ({0}): {1}", outputDirectory, ex.Message), ex);
                    }
                }
            }
            else
            {
                outputDirectory = Directory.GetCurrentDirectory();
            }

            return outputDirectory;
        }

        private static void DoCreateCertificates(bool createServerCert, bool createClientCert, string outputDirectory)
        {
            if (createServerCert)
            {
                string serverCertOutputPath = Path.Combine(outputDirectory, "RemoteHarvester.pfx");
                Console.WriteLine("Generating server certificate: {0}", serverCertOutputPath);
                CreatePfxFile("PeachFarmerServer", serverCertOutputPath);
            }

            if (createClientCert)
            {
                string clientCertOutputPath = Path.Combine(outputDirectory, "PeachFarmerClient.pfx");
                Console.WriteLine("Generating client certificate: {0}", clientCertOutputPath);
                CreatePfxFile("PeachFarmerClient", clientCertOutputPath);
            }
        }

        private static void CreatePfxFile(string commonName, string outputFilename)
        {
            try
            {
                string x500 = string.Format("CN=\"{0}\"", commonName);
                byte[] certValue = CertificateGenerator.CreateSelfSignCertificatePfx(x500, DateTime.Now, DateTime.Now.AddYears(10));

                using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilename, FileMode.CreateNew)))
                {
                    writer.Write(certValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating certificate: {0}", ex.Message);
            }
        }
    }
}
