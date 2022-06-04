using System;

namespace QLESS.Core.BusinessRules
{
    public class InvalidIdException : ArgumentException
    {
        public InvalidIdException() : base("Invalid id.") { }
        public InvalidIdException(string message) : base(message) { }
        public InvalidIdException(string message, Exception innerException) : base(message, innerException) { }
    }
}
