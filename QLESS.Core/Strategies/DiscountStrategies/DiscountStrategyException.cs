using System;

namespace QLESS.Core.Strategies.DiscountStrategies
{
    public class DiscountStrategyException : ArgumentException
    {
        public DiscountStrategyException() { }
        public DiscountStrategyException(string message) : base(message) { }
        public DiscountStrategyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
