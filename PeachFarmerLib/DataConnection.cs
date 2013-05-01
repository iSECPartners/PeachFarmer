using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib
{
    public class DataConnection : IDataConnection
    {
        private INetworkConnection _networkConnection;

        public DataConnection(INetworkConnection networkConnection)
        {
            _networkConnection = networkConnection;
        }

        public void SendDateTime(DateTime dateTime)
        {
            byte[] dateTimeBytes = BitConverter.GetBytes(dateTime.Ticks);
            _networkConnection.WriteBytes(dateTimeBytes);
        }

        public DateTime ReceiveDateTime()
        {
            byte[] minModifiedTimeBytes = _networkConnection.ReadBytes(sizeof(long) / sizeof(byte));
            long minModifiedTimeLong = BitConverter.ToInt64(minModifiedTimeBytes, 0);
            DateTime minModifiedTimeUtc = new DateTime(minModifiedTimeLong);

            return minModifiedTimeUtc;
        }

        public void SendString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                SendInt(0);
                return;
            }
            else
            {
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);
                SendInt(valueBytes.Length);

                _networkConnection.WriteBytes(valueBytes);
            }
        }

        public string ReceiveString(int maxSize)
        {
            int stringLength = ReceiveInt();
            if (stringLength <= 0)
            {
                return null;
            }

            if (stringLength > maxSize)
            {
                throw new ArgumentException(string.Format("String larger than expected: ({0}), allowed: ({1})", stringLength, maxSize));
            }

            byte[] valueBytes = _networkConnection.ReadBytes(stringLength);

            return Encoding.UTF8.GetString(valueBytes);
        }

        public void SendByte(byte value)
        {
            _networkConnection.WriteBytes(new byte[] { value });
        }

        public byte ReceiveByte()
        {
            return _networkConnection.ReadBytes(1)[0];
        }

        public void SendBytes(byte[] value)
        {
            _networkConnection.WriteBytes(value);
        }

        public byte[] ReceiveBytes(int length)
        {
            return _networkConnection.ReadBytes(length);
        }

        public void SendInt(int value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            _networkConnection.WriteBytes(valueBytes);
        }

        public int ReceiveInt()
        {
            byte[] valueBytes = _networkConnection.ReadBytes(sizeof(int));
            return BitConverter.ToInt32(valueBytes, 0);
        }

        public void Close()
        {
            _networkConnection.Close();
        }

        public void Dispose()
        {
            _networkConnection.Dispose();
        }
    }
}
