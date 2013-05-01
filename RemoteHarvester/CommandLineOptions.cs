using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class CommandLineOptions
    {
        private string _logFolder;

        [Option('d', "logdirectory", Required = true, MetaValue = "DIRPATH",
                HelpText = "Location of Peach log folder directory to farm (can be directory for an individual run folder or entire logs directory).")]
        public string LogFolder
        {
            get
            {
                return _logFolder;
            }
            set
            {
                _logFolder = value;
                if (!_logFolder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    _logFolder += System.IO.Path.DirectorySeparatorChar;
                }
            }
        }

        [Option("password", Required = false, MetaValue = "PASSWORD",
                HelpText = "Password required to access the server.")]
        public string Password { get; set; }

        [Option("server-cert", Required = false, MetaValue = "CERTFILE",
                HelpText = "File containing pfx formatted server x.509 certificate (to authenticate the local server).")]
        public string ServerCertFile { get; set; }

        [Option("client-cert", Required = false, MetaValue = "CERTFILE",
                HelpText = "File containing pfx formatted client x.509 certificate (for certificate pinning to a remote client).")]
        public string ClientCertFile { get; set; }

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
