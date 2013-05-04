using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class TargetFileParser
    {
        public static List<string> ParseTargets(Stream targetFileStream)
        {
            List<string> targets = new List<string>();

            using (StreamReader reader = new StreamReader(targetFileStream))
            {
                targets = (from line in reader.ReadToEnd().Split(new char[] { '\r', '\n' } )
                               let trimmed = line.Trim()
                               where !string.IsNullOrWhiteSpace(trimmed)
                               select trimmed).ToList();
            }

            return targets;
        }
    }
}
