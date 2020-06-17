using System;
namespace Task20181213_P1
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
