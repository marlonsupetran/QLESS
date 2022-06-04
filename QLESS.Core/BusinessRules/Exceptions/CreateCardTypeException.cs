using System;

namespace QLESS.Core.BusinessRules
{
    public class CreateCardTypeException : ArgumentException
    {
        public CreateCardTypeException() { }
        public CreateCardTypeException(string message) : base(message) { }
        public CreateCardTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
