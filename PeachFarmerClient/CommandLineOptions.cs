using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class CommandLineOptions
    {
        private string _destinationFolder;

        [Option('h', "host", MutuallyExclusiveSet = "targets",
                HelpText = "Hostname of worker to query.")]
        public string WorkerHost { get; set; }

        [Option('i', "inputList", MutuallyExclusiveSet="targets", MetaValue = "FILE",
                HelpText = "File containing hostnames of workers to query (one per line).")]
        public string WorkerHostFile { get; set; }

        [Option('d', "destination", MetaValue="DIRPATH", Required = true,
                HelpText = "Destination folder to store files.")]
        public string DestinationFolder
        {
            get
            {
                return _destinationFolder;
            }
            set
            {
                _destinationFolder = value;
                if (!_destinationFolder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    _destinationFolder += System.IO.Path.DirectorySeparatorChar;
                }
            }
        }

        [Option("password", Required = false, MetaValue = "PASSWORD",
                HelpText = "Password for remote workers.")]
        public string Password { get; set; }

        [Option("server-cert", Required = false, MetaValue = "CERTFILE",
                HelpText = "File containing pfx formatted server x.509 certificate (to authenticate the remote server).")]
        public string ServerCertFile { get; set; }

        [Option("client-cert", Required = false, MetaValue = "CERTFILE",
                HelpText = "File containing pfx formatted client x.509 certificate (for mutual authentication to the remote server).")]
        public string ClientCertFile { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public string GetValidationError()
        {
            if ((WorkerHost == null) && (WorkerHostFile == null))
            {
                return "Must enter a target host or file of target hosts.";
            }

            return null;
        }
    }
}
