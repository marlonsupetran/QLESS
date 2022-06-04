using QLESS.Core.Strategies.DiscountStrategies;
using QLESS.Core.Strategies.FareStrategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace QLESS.Core.Strategies
{
    public class StrategyFactory : IStrategyFactory
    {
        // Fields
        private ICollection<KeyValuePair<string, Guid>> fareStrategies;
        private ICollection<KeyValuePair<string, Guid>> discountStrategies;

        // Properties
        private IDictionary<Guid, IFareStrategy> FareStrategyDictionary => new Dictionary<Guid, IFareStrategy>()
        {
            { new Guid("DD37EF03-AEC3-4C8F-9D18-C526F3FC6B46"), new BaseFareStrategy() }
        };
        private IDictionary<Guid, IDiscountStrategy> DiscountStrategyDictionary => new Dictionary<Guid, IDiscountStrategy>()
        {
            { new Guid("C63B4FB4-2339-4EEA-9BDC-1C185F47F153"), new SeniorAndPwdDiscountStrategy() }
        };

        // Methods
        public IFareStrategy GetFareStrategy(Guid fareStrategyId)
        {
            return FareStrategyDictionary[fareStrategyId];
        }
        public IDiscountStrategy GetDiscountStrategy(Guid discountStrategyId)
        {
            return DiscountStrategyDictionary[discountStrategyId];
        }
        public ICollection<KeyValuePair<string, Guid>> GetFareStrategies()
        {
            return fareStrategies ??= FareStrategyDictionary
                .Select(f => 
                {
                    var type = f.Value.GetType();
                    var attribute = type
                        .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                        .FirstOrDefault() as DisplayNameAttribute;
                    var key = attribute?.DisplayName ?? PascalCaseToSeparateWords(type.Name);

                    return new KeyValuePair<string, Guid>(key, f.Key);
                })
                .ToList();
        }
        public ICollection<KeyValuePair<string, Guid>> GetDiscountStrategies()
        {
            return discountStrategies ??= DiscountStrategyDictionary
                .Select(f =>
                {
                    var type = f.Value.GetType();
                    var attribute = type
                        .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                        .FirstOrDefault() as DisplayNameAttribute;
                    var key = attribute?.DisplayName ?? PascalCaseToSeparateWords(type.Name);

                    return new KeyValuePair<string, Guid>(key, f.Key);
                })
                .ToList()
;
        }
        private string PascalCaseToSeparateWords(string s)
        {
            var r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return r.Replace(s, " ");
        }
    }
}
