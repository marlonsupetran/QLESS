using System;

namespace QLESS.Core.BusinessRules
{
    public class DuplicateCardException : InvalidOperationException
    {
        public DuplicateCardException() : base("Card is already active.") { }
        public DuplicateCardException(string message) : base(message) { }
        public DuplicateCardException(string message, Exception innerException) : base(message, innerException) { }
    }
}
