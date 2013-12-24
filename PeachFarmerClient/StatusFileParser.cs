using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PeachFarmerClient
{

    public class StatusFileParser : IStatusFileParser
    {
        private enum StatusLineType
        {
            Unknown = 0,
            FaultDetected,
            IterationX,
            IterationXofN,
            TestFinished
        };

        public PeachStatus Parse(Stream statusFileStream)
        {

            PeachStatus peachStatus = new PeachStatus();

            using (StreamReader sr = new StreamReader(statusFileStream))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    StatusLineType lineType = identifyLineType(line);
                    UInt64 startingIteration;
                    UInt64 finalIteration;
                    UInt64 faultingIteration;

                    switch (lineType)
                    {
                        case StatusLineType.FaultDetected:
                            ParseFaultDetectedLine(line, out faultingIteration);

                            peachStatus.LastCompletedIteration = faultingIteration;
                            peachStatus.LastUpdate = ParseDate(line);
                            break;
                        case StatusLineType.IterationX:
                            ParseIterationXLine(line, out startingIteration);

                            peachStatus.LastCompletedIteration = startingIteration - 1;
                            peachStatus.LastUpdate = ParseDate(line);
                            break;
                        case StatusLineType.IterationXofN:
                            ParseIterationXofNLine(line, out startingIteration, out finalIteration);

                            peachStatus.LastCompletedIteration = startingIteration - 1;
                            peachStatus.FinalIteration = finalIteration;
                            peachStatus.LastUpdate = ParseDate(line);
                            break;
                        case StatusLineType.TestFinished:
                            peachStatus.Finished = true;
                            return peachStatus;
                        default:
                            break;
                    }
                }
            }

            return peachStatus;
        }

        private StatusLineType identifyLineType(string line)
        {
            if (Regex.IsMatch(line, @"Fault detected at iteration \d+ :"))
            {
                return StatusLineType.FaultDetected;
            }
            else if (Regex.IsMatch(line, @"Iteration \d+ :"))
            {
                return StatusLineType.IterationX;
            }
            else if (Regex.IsMatch(line, @"Iteration \d+ of \d+ :"))
            {
                return StatusLineType.IterationXofN;
            }
            else if (Regex.IsMatch(line, @"Test finished:"))
            {
                return StatusLineType.TestFinished;
            }

            return StatusLineType.Unknown;
        }

        private void ParseIterationXLine(string line, out UInt64 iterationNumber)
        {
            iterationNumber = ParseFirstNumber(line);
        }

        private void ParseIterationXofNLine(string line, out UInt64 iterationNumber, out UInt64 finalIterationNumber)
        {
            string matchValue = Regex.Match(line, @"\d+ of \d+").Value;
            string[] parts = matchValue.Split(new string[] {" of "}, StringSplitOptions.None);
            iterationNumber = UInt64.Parse(parts[0]);
            finalIterationNumber = UInt64.Parse(parts[1]);
        }

        private void ParseFaultDetectedLine(string line, out UInt64 iterationNumber)
        {
            iterationNumber = ParseFirstNumber(line);
        }

        private UInt64 ParseFirstNumber(string line)
        {
            return UInt64.Parse(Regex.Match(line, @"\d+").Value);
        }

        private DateTime ParseDate(string line)
        {
            string date = Regex.Match(line, @"\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d\d:\d\d (A|P)M").Value;
            return DateTime.Parse(date);
        }
    }
}
