using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace QLESS.Core.BusinessRules
{
    public class TicketingBusinessRules : BaseBusinessRules, ITicketingBusinessRules
    {
        // Constructors
        public TicketingBusinessRules(IRepository repository) : base(repository) { }

        // Methods
        public ICardModel Activate(Guid cardNumber, Guid cardTypeId, Guid privilegeId, string privilegeCardNumber)
        {
            if (cardNumber == Guid.Empty)
                throw new InvalidIdException("Card id is invalid.");

            if (Repository.Read<Card>(c => c.Number == cardNumber).FirstOrDefault() is Card existingCard)
                throw new DuplicateCardException();

            if (cardTypeId == Guid.Empty)
                throw new InvalidIdException("Card type id is invalid.");

            if (!(Repository.Read<CardType, Guid>(cardTypeId) is CardType cardType))
                throw new CardTypeNotFoundException();

            if (cardType.Privileges.Any() && privilegeId == Guid.Empty)
                throw new CardActivationException("Privilege id is invalid.");

            var newCard = new Card()
            {
                Number = cardNumber,
                Created = DateTime.Today,
                Balance = cardType.InitialBalance,
                Type = cardType,
                Expiry = cardType.Validity > 0
                    ? DateTime.Today.AddTicks(cardType.Validity) as DateTime?
                    : null
            };

            if (cardType.Privileges.Any())
            {
                if (!(cardType.Privileges?.SingleOrDefault(p => p.Id == privilegeId) is Privilege privilege))
                {
                    throw new CardActivationException("Privilege with specified id does not exist.");
                }
                else if (
                    !string.IsNullOrWhiteSpace(privilege.IdentificationNumberPattern) &&
                    !Regex.Match(privilegeCardNumber, privilege.IdentificationNumberPattern).Success)
                {
                    throw new CardActivationException("Privilege id is not in correct format.");
                }
                else
                {
                    newCard.PrivilegeCard = new PrivilegeCard()
                    {
                        IdentificationNumber = privilegeCardNumber,
                        Type = privilege
                    };
                }
            }

            Repository.Create(newCard);
            Repository.SaveChanges();

            return CreateCardDataModel(newCard);
        }
        public ICardModel CheckBalance(Guid cardNumber)
        {
            if (cardNumber == Guid.Empty)
                throw new InvalidIdException("Card id is invalid.");

            if (!(Repository.Read<Card>(c => c.Number == cardNumber).FirstOrDefault() is Card card))
                throw new CardNotFoundException();

            return CreateCardDataModel(card);
        }
        public ICardReloadModel Reload(Guid cardNumber, decimal amount, decimal payment)
        {
            if (cardNumber == Guid.Empty)
                throw new InvalidIdException("Card is invalid.");

            if (!(Repository.Read<Card>(c => c.Number == cardNumber).FirstOrDefault() is Card card))
                throw new CardNotFoundException();

            if (amount < card.Type.MinimumReloadAmount)
                throw new ReloadAmountException("Reload amount is below minimum amount.");

            if (amount > card.Type.MaximumReloadAmount)
                throw new ReloadAmountException("Reload amount exceeds maximum amount.");

            if ((payment - amount) is decimal change && change < 0)
                throw new ReloadAmountException("Reload amount is exceeds payment.");

            if (Math.Max(0, ((card.Balance + amount) - card.Type.MaximumBalance)) is decimal excess && excess > 0)
            {
                card.Balance = card.Type.MaximumBalance;
                change += excess;
            }
            else
            {
                card.Balance += amount;
            }

            Repository.SaveChanges();

            return CreateCardReloadDataModel(card, amount, payment, change);
        }
        private ICardModel CreateCardDataModel(Card card)
        {
            return new CardModel()
            {
                Number = card.Number,
                Balance = card.Balance,
                Expiry = card.Expiry
            };
        }
        private ICardReloadModel CreateCardReloadDataModel(Card card, decimal amount, decimal payment, decimal change)
        {
            return new CardReloadModel()
            {
                Number = card.Number,
                Balance = card.Balance,
                Expiry = card.Expiry,
                Amount = amount,
                Payment = payment,
                Change = change,
            };
        }
    }
}
