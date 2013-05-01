using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertPairGenerator
{
    public class CommandLineOptions
    {
        private string _DestinationFolder;

        [Option('d', "destinationFolder", Required = false, MetaValue = "DIRPATH",
                HelpText = "Output directory to create certificates.")]
        public string DestinationFolder { get; set; }

        [Option('c', "client", Required = false,
                HelpText = "Create a client certificate.")]
        public bool CreateClientCert { get; set; }

        [Option('s', "server", Required = false,
                HelpText = "Create a server certificate.")]
        public bool CreateServerCert { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
