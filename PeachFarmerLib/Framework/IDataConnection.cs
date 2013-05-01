using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface IDataConnection : IDisposable
    {
        void SendDateTime(DateTime dateTime);

        DateTime ReceiveDateTime();

        void SendString(string value);

        string ReceiveString(int maxSize);

        void SendByte(byte value);

        byte ReceiveByte();

        void SendBytes(byte[] value);

        byte[] ReceiveBytes(int length);

        void SendInt(int value);

        int ReceiveInt();

        void Close();
    }
}
