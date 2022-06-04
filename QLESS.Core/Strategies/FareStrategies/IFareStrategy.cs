using QLESS.Core.Entities;

namespace QLESS.Core.Strategies.FareStrategies
{
    public interface IFareStrategy
    {
        decimal GetFare(Card card, int entryStationNumber, int exitStationNumber);
    }
}
