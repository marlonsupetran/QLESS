using QLESS.Core.Strategies.DiscountStrategies;
using QLESS.Core.Strategies.FareStrategies;
using System;
using System.Collections.Generic;

namespace QLESS.Core.Strategies
{
    public interface IStrategyFactory
    {
        IFareStrategy GetFareStrategy(Guid fareStrategyId);
        IDiscountStrategy GetDiscountStrategy(Guid discountStrategyId);
        ICollection<KeyValuePair<string, Guid>> GetFareStrategies();
        ICollection<KeyValuePair<string, Guid>> GetDiscountStrategies();
    }
}
