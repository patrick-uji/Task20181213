using System;
namespace Task20181213.Common
{
    public class FixerException : Exception
    {
        public int ErrorCode { get; private set; }
        public FixerException(string message, int errorCode) : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
