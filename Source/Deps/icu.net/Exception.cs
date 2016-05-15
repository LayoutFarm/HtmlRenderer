using System;
namespace Icu
{
    class WarningException : Exception
    {
        public WarningException(string msg)
            : base(msg)
        {
        }
    }
    class InvalidDataException : Exception
    {
        public InvalidDataException(string msg)
            : base(msg)
        {
        }
    }
    class InternalBufferOverflowException : Exception
    {
        public InternalBufferOverflowException(string msg)
            : base(msg)
        {
        }
    }
    class InvalidEnumArgumentException : Exception
    {
        public InvalidEnumArgumentException(string msg)
            : base(msg)
        {
        }
    }
}