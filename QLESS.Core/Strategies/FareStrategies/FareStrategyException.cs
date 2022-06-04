using System;

namespace QLESS.Core.Strategies.FareStrategies
{
    public class FareStrategyException : ArgumentException
    {
        public FareStrategyException() { }
        public FareStrategyException(string message) : base(message) { }
        public FareStrategyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
