using QLESS.Core.Entities;
using System;
using System.ComponentModel;
using System.Linq;

namespace QLESS.Core.Strategies.DiscountStrategies
{
    [DisplayName("Senior & PWD Discount Scheme")]
    public class SeniorAndPwdDiscountStrategy : IDiscountStrategy
    {
        public decimal GetPrecentageDiscount(Card card)
        {
            if (card == null)
                throw new DiscountStrategyException("Card is invalid.");

            var discount = 0.2M;
            var sameDayTrips = card.Trips.Count(t => t.Exit.Value.Date == DateTime.Today);

            if (0 < sameDayTrips && sameDayTrips < 4)
            {
                discount += 0.03M;
            }

            return discount;
        }
    }
}
