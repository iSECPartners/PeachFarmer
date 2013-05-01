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
                while (!reader.EndOfStream)
                {
                    string target = reader.ReadLine().Trim();

                    if (string.IsNullOrWhiteSpace(target))
                    {
                        continue;
                    }

                    targets.Add(target);
                }
            }

            return targets;
        }
    }
}
