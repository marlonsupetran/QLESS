using System;

namespace QLESS.Core.BusinessRules
{
    public class CardTypeNotFoundException : ArgumentException
    {
        public CardTypeNotFoundException() : base("Card type does not exists.") { }
        public CardTypeNotFoundException(string message) : base(message) { }
        public CardTypeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
