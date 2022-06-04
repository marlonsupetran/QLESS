using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Strategies;
using QLESS.Core.Strategies.DiscountStrategies;
using QLESS.Core.Strategies.FareStrategies;
using System;
using System.Linq;

namespace QLESS.Core.BusinessRules
{
    public class GateBusinessRules : BaseBusinessRules, IGateBusinessRules
    {
        // Fields
        protected IStrategyFactory StrategyFactory { get; private set; }

        // Constructors
        public GateBusinessRules(IRepository repository, IStrategyFactory strategyFactory) : base(repository)
        {
            StrategyFactory = strategyFactory;
        }

        // Methods
        public void Enter(Guid cardNumber, int entryStationNumber)
        {
            if (cardNumber == Guid.Empty)
                throw new CardNotFoundException("Card id is invalid.");

            if (!(Repository.Read<Card>(c => c.Number == cardNumber).FirstOrDefault() is Card card))
                throw new CardNotFoundException("Card not found.");

            if (card.Expiry < DateTime.Today)
                throw new GateException("Card has expired.");

            if (card.Trips.Any(t => !t.Exit.HasValue))
                throw new GateException("Card has pending trip.");

            card.Trips.Add(new Trip()
            {
                Entry = DateTime.Now,
                EntryStationNumber = entryStationNumber
            });

            Repository.SaveChanges();
        }
        public void Exit(Guid cardNumber, int exitStationNumber)
        {
            decimal fare;
            int entryStationNumber = 1;

            if (cardNumber == Guid.Empty)
                throw new CardNotFoundException("Card id is invalid.");

            if (!(Repository.Read<Card>(c => c.Number == cardNumber).FirstOrDefault() is Card card))
                throw new CardNotFoundException("Card not found.");

            if (!(card.Trips.LastOrDefault(t => !t.Exit.HasValue) is Trip trip))
                throw new GateException("Card has no penging trip.");

            if (!(StrategyFactory.GetFareStrategy(card.Type.FareStrategyId) is IFareStrategy fareStrategy))
            {
                throw new GateException("Fare strategy not found.");
            }
            else
            {
                fare = fareStrategy.GetFare(card, entryStationNumber, exitStationNumber);

                if (fare > card.Balance)
                    throw new GateException("Insufficient balance.");
            }

            if (card.Type.DiscountStrategyId != Guid.Empty)
            {
                if(!(StrategyFactory.GetDiscountStrategy(card.Type.DiscountStrategyId) is IDiscountStrategy discountStrategy))
                {
                    throw new GateException("Discount strategy not found.");
                }
                else
                {
                    fare *= 1 - discountStrategy.GetPrecentageDiscount(card);
                }
            }

            card.Balance -= fare;
            trip.Exit = DateTime.Now;
            trip.ExitStationNumber = exitStationNumber;

            Repository.SaveChanges();
        }
    }
}
