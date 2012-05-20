using System;
using System.Text;

namespace NerZul.Network
{
    class MailServiceException : Exception
    {
        public override string Message
        {
            get
            {
                return "Unable to get ID from mail service";
            }
        }
    };
};