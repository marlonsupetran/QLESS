using QLESS.Core.Data;
using QLESS.Core.Entities;
using System.ComponentModel;

namespace QLESS.Core.Strategies.FareStrategies
{
    [DisplayName("Base Fare Scheme")]
    public class BaseFareStrategy : IFareStrategy
    {
        // Methods
        public decimal GetFare(Card card, int entryStationNumber, int exitStationNumber)
        {
            if (card == null)
                throw new FareStrategyException("Card not found.");

            if (card.Type == null)
                throw new FareStrategyException("Card type is invalid.");

            return card.Type.BaseFare;
        }
    }
}
