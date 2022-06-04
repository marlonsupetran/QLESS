using System;

namespace QLESS.Core.BusinessRules
{
    public class CardNotFoundException : ArgumentException
    {
        public CardNotFoundException() : base("Card does not exist.") { }
        public CardNotFoundException(string message) : base(message) { }
        public CardNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}