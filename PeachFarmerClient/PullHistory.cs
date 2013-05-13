using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    [Serializable()]
    public class PullHistory : IPullHistory
    {
        private Dictionary<string, DateTime> _lastPullTimes;

        private IFileSystem _fileSystem;

        private string _filename;

        public PullHistory(IFileSystem fileSystem, string filename)
        {
            _lastPullTimes = new Dictionary<string, DateTime>();

            _fileSystem = fileSystem;

            _filename = filename;
        }

        public DateTime GetLastPullTime(string workerHost)
        {
            try
            {
                return _lastPullTimes[workerHost];
            }
            catch (KeyNotFoundException)
            {
                return new DateTime(0);
            }
        }

        public void SetLastPullTime(string workerHost, DateTime utcTime)
        {
            if (_lastPullTimes.ContainsKey(workerHost))
            {
                _lastPullTimes[workerHost] = utcTime;
            }
            else
            {
                _lastPullTimes.Add(workerHost, utcTime);
            }
        }

        public void Save()
        {
            using (Stream pullHistoryStream = _fileSystem.GetOutputStream(_filename))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(pullHistoryStream, this._lastPullTimes);
            }
        }

        public void Load()
        {
            if (_fileSystem.FileExists(_filename))
            {
                using (Stream pullHistoryStream = File.OpenRead(_filename))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    this._lastPullTimes = (Dictionary<string, DateTime>)deserializer.Deserialize(pullHistoryStream);
                }
            }
        }
    }
}
