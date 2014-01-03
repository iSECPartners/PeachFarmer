using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public abstract class RequestProcessorBase
    {
        private byte[] _correctPasswordHash;

        public RequestProcessorBase(string password)
        {
            if (password != null)
            {
                _correctPasswordHash = PasswordHasher.CalculateHash(password);
            }
            else
            {
                _correctPasswordHash = null;
            }
        }

        public virtual ResponseMessageBase Process(RequestMessageBase requestMessage)
        {
            ResponseMessageBase response = CreateResponseMessage();

            response.IsPasswordCorrect = IsPasswordValid(requestMessage.ServerPassword);
            if (!response.IsPasswordCorrect)
            {
                return response;
            }

            ProcessImpl(requestMessage, response);

            return response;
        }

        protected abstract void ProcessImpl(RequestMessageBase requestMessage, ResponseMessageBase responseMessage);

        protected abstract ResponseMessageBase CreateResponseMessage();

        private bool IsPasswordValid(string sentPassword)
        {
            if (_correctPasswordHash == null)
            {
                return true;
            }

            if (sentPassword == null)
            {
                return false;
            }

            byte[] sentPasswordHash = PasswordHasher.CalculateHash(sentPassword);

            for (int i = 0; i < _correctPasswordHash.Length; i++)
            {
                if (_correctPasswordHash[i] != sentPasswordHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
