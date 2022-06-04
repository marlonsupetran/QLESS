using System;

namespace QLESS.Core.BusinessRules
{
    public class CardActivationException : ArgumentException
    {
        public CardActivationException() : base("Card does not exist.") { }
        public CardActivationException(string message) : base(message) { }
        public CardActivationException(string message, Exception innerException) : base(message, innerException) { }
    }
}