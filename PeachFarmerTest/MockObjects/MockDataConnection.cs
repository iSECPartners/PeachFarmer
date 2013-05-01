using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    public class MockDataConnection : IDataConnection
    {
        public List<DateTime> DateTimesSentQueue { get; private set; }
        public List<string> StringsSentQueue { get; private set; }
        public List<byte> BytesSentQueue { get; private set; }
        public List<byte[]> ByteArraysSentQueue { get; private set; }
        public List<int> IntsSentQueue { get; private set; }

        public List<DateTime> DateTimesToReceiveQueue { get; private set; }
        public List<string> StringsToReceiveQueue { get; private set; }
        public List<byte> BytesToReceiveQueue { get; private set; }
        public List<byte[]> ByteArraysToReceiveQueue { get; private set; }
        public List<int> IntsToReceiveQueue { get; private set; }

        public MockDataConnection()
        {
            DateTimesSentQueue = new List<DateTime>();
            StringsSentQueue = new List<string>();
            BytesSentQueue = new List<byte>();
            ByteArraysSentQueue = new List<byte[]>();
            IntsSentQueue = new List<int>();

            DateTimesToReceiveQueue = new List<DateTime>();
            StringsToReceiveQueue = new List<string>();
            BytesToReceiveQueue = new List<byte>();
            ByteArraysToReceiveQueue = new List<byte[]>();
            IntsToReceiveQueue = new List<int>();
        }

        public void SendDateTime(DateTime value)
        {
            DateTimesSentQueue.Add(value);
        }

        public DateTime ReceiveDateTime()
        {
            DateTime value = DateTimesToReceiveQueue[0];
            DateTimesToReceiveQueue.RemoveAt(0);
            return value;
        }

        public void SendString(string value)
        {
            StringsSentQueue.Add(value);
        }

        public string ReceiveString(int maxSize)
        {
            string value = StringsToReceiveQueue[0];
            StringsToReceiveQueue.RemoveAt(0);
            return value;
        }

        public void SendByte(byte value)
        {
            BytesSentQueue.Add(value);
        }

        public byte ReceiveByte()
        {
            byte value = BytesToReceiveQueue[0];
            BytesToReceiveQueue.RemoveAt(0);
            return value;
        }

        public void SendBytes(byte[] value)
        {
            ByteArraysSentQueue.Add(value);
        }

        public byte[] ReceiveBytes(int length)
        {
            byte[] value = ByteArraysToReceiveQueue[0];
            ByteArraysToReceiveQueue.RemoveAt(0);
            return value;
        }

        public void SendInt(int value)
        {
            IntsSentQueue.Add(value);
        }

        public int ReceiveInt()
        {
            int value = IntsToReceiveQueue[0];
            IntsToReceiveQueue.RemoveAt(0);
            return value;
        }

        public void Close()
        {
            ;
        }

        public void Dispose()
        {
            ;
        }
    }
}
