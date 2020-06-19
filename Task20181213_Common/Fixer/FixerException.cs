using System;
namespace Task20181213.Common
{
    public class FixerException : Exception
    {
        public FixerErrorCode ErrorCode { get; private set; }
        internal FixerException(string message, FixerErrorCode errorCode) : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
