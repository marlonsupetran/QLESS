using QLESS.Core.Entities;

namespace QLESS.Core.Strategies.DiscountStrategies
{
    public interface IDiscountStrategy
    {
        decimal GetPrecentageDiscount(Card card);
    }
}
