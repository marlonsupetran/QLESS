using System;

namespace QLESS.Core.BusinessRules
{
    public class ReloadAmountException : ArgumentOutOfRangeException
    {
        public ReloadAmountException() { }
        public ReloadAmountException(string message) : base(message) { }
        public ReloadAmountException(string message, Exception innerException) : base(message, innerException) { }
    }
}
