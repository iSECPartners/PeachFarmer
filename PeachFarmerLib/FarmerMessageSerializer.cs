using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeachFarmerLib.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PeachFarmerLib
{
    public class FarmerMessageSerializer
    {
        public void Serialize(Stream stream, IFarmerNetworkMessage message)
        {
            using (MemoryStream temporaryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(temporaryStream, message);

                byte[] sizeBytes = BitConverter.GetBytes(temporaryStream.Length);
                stream.Write(sizeBytes, 0, sizeBytes.Length);

                temporaryStream.Seek(0, SeekOrigin.Begin);
                temporaryStream.CopyTo(stream);
                stream.Flush();
            }
        }

        public IFarmerNetworkMessage Deserialize(Stream stream)
        {
            byte[] sizeBytes = new byte[sizeof(long)];
            stream.Read(sizeBytes, 0, sizeBytes.Length);
            long messageSize = BitConverter.ToInt64(sizeBytes, 0);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                CopyStreamBytes(stream, memoryStream, messageSize);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BinaryFormatter deserializer = new BinaryFormatter();
                return (IFarmerNetworkMessage) deserializer.Deserialize(memoryStream);
            }
        }

        private void CopyStreamBytes(Stream source, Stream destination, long bytesToCopy)
        {
            long bytesRemaining = bytesToCopy;
            byte[] buffer = new byte[Environment.SystemPageSize];

            while (bytesRemaining > 0)
            {
                int bytesToRead;
                if (buffer.Length < bytesRemaining)
                {
                    bytesToRead = buffer.Length;
                }
                else
                {
                    bytesToRead = (int)bytesRemaining;
                }

                int bytesRead = source.Read(buffer, 0, bytesToRead);
                destination.Write(buffer, 0, bytesRead);

                bytesRemaining -= bytesRead;
            }
        }
    }
}
