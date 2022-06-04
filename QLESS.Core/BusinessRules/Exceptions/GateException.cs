using System;

namespace QLESS.Core.BusinessRules
{
    public class GateException : InvalidOperationException
    {
        public GateException() { }
        public GateException(string message) : base(message) { }
        public GateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
