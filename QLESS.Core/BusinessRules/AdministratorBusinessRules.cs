using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Extensions;
using QLESS.Core.Models;
using QLESS.Core.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QLESS.Core.BusinessRules
{
    public class AdministratorBusinessRules : BaseBusinessRules, IAdministratorBusinessRules
    {
        // Fields
        protected IStrategyFactory StrategyFactory { get; private set; }

        // Constructors
        public AdministratorBusinessRules(IRepository repository, IStrategyFactory strategyFactory) : base(repository)
        {
            StrategyFactory = strategyFactory;
        }

        // Methods
        public ICardTypeModel CreateOrEditCardType(ICardTypeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new CreateCardTypeException("Name should not be null, empty, or whitespace.");

            if (model.Description?.Trim().Length > 255)
                throw new CreateCardTypeException("Description should not exceed 255 characters.");

            if (model.MinimumBalance > model.MaximumBalance || model.MinimumBalance < 0)
                throw new CreateCardTypeException("Minimum balance should be greater than or equal to zero but less than maximum balance.");

            if (model.InitialBalance > model.MaximumBalance || model.InitialBalance < 0)
                throw new CreateCardTypeException("Initial balance should be greater than or equal to zero but less than maximum balance.");

            if (model.MinimumReloadAmount > model.MaximumReloadAmount || model.MinimumReloadAmount < 0)
                throw new CreateCardTypeException("Minimum reload amount should be greater than or equal to zero but less than maximum reload amount.");

            if (model.BaseFare > model.InitialBalance || model.BaseFare < 0)
                throw new CreateCardTypeException("Base fare should be greater than or equal to zero but less than initial balance.");

            if (StrategyFactory.GetFareStrategy(model.FareStrategyId) == null)
                throw new CreateCardTypeException("Fare strategy not found.");

            if (model.DiscountStrategyId != Guid.Empty &&
                StrategyFactory.GetDiscountStrategy(model.DiscountStrategyId) == null)
                throw new CreateCardTypeException("Discount strategy not found.");

            var privileges = new List<Privilege>();
            if (model.PrivilegeIds != null && model.PrivilegeIds.Any())
            {
                privileges = Repository
                    .Read<Privilege>(p => model.PrivilegeIds.Contains(p.Id))
                    .ToList();
            }

            CardType cardType;
            if (model.Id != Guid.Empty)
            {
                if ((cardType = Repository.Read<CardType, Guid>(model.Id)) == null)
                {
                    throw new CreateCardTypeException("Card type with specified id does not exist.");
                }
                else
                {
                    cardType.FareStrategyId = model.FareStrategyId;
                    cardType.DiscountStrategyId = model.DiscountStrategyId;
                    cardType.Name = model.Name;
                    cardType.Description = model.Description;
                    cardType.InitialBalance = model.InitialBalance;
                    cardType.MinimumBalance = model.MinimumBalance;
                    cardType.MaximumBalance = model.MaximumBalance;
                    cardType.MinimumReloadAmount = model.MinimumReloadAmount;
                    cardType.MaximumReloadAmount = model.MaximumReloadAmount;
                    cardType.BaseFare = model.BaseFare;
                    cardType.Validity = model.Validity;

                    var toInsert = (from a in privileges
                                   join b in cardType.Privileges on a.Id equals b.Id into ab
                                   from c in ab.DefaultIfEmpty()
                                   where c == null
                                   select a).ToList();

                    var toRemove = (from a in cardType.Privileges
                                   join b in privileges on a.Id equals b.Id into ab
                                   from c in ab.DefaultIfEmpty()
                                   where c == null
                                   select a).ToList();

                    foreach (var r in toRemove)
                    {
                        var ctp = cardType.CardTypePrivileges.SingleOrDefault(c => c.PrivilegeId == r.Id);
                        cardType.CardTypePrivileges.Remove(ctp);
                    }

                    foreach (var i in toInsert)
                    {
                        cardType.CardTypePrivileges.Add(new CardTypePrivilege(cardType, i));
                    }
                }
            }
            else
            {
                cardType = Repository.Create(new CardType()
                {
                    FareStrategyId = model.FareStrategyId,
                    DiscountStrategyId = model.DiscountStrategyId,
                    Name = model.Name,
                    Description = model.Description,
                    InitialBalance = model.InitialBalance,
                    MinimumBalance = model.MinimumBalance,
                    MaximumBalance = model.MaximumBalance,
                    MinimumReloadAmount = model.MinimumReloadAmount,
                    MaximumReloadAmount = model.MaximumReloadAmount,
                    BaseFare = model.BaseFare,
                    Validity = model.Validity
                });

                cardType.Privileges = privileges;
            }

            Repository.SaveChanges();

            model.Id = cardType.Id;
            return model;
        }
        public IPrivilegeModel CreateOrEditPriviledge(IPrivilegeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name?.Trim()))
                throw new CreatePriviledgeException("Name should not be null, empty, or whitespace.");

            if (model.Description?.Trim().Length > 255)
                throw new CreatePriviledgeException("Description should not exceed 255 characters.");

            if (!string.IsNullOrWhiteSpace(model.IdentificationNumberPattern) &&
                !IsValidRegexPattern(model.IdentificationNumberPattern?.Trim()))
                throw new CreatePriviledgeException("Identification number pattern should be a valid regular expression.");

            Privilege privilege;
            if (model.Id != Guid.Empty)
            {
                if ((privilege = Repository.Read<Privilege, Guid>(model.Id)) == null)
                {
                    throw new CreatePriviledgeException("Privilege with specified id does not exist.");
                }
                else
                {
                    privilege.Name = model.Name;
                    privilege.Description = model.Description;
                    privilege.IdentificationNumberPattern = model.IdentificationNumberPattern;
                }
            }
            else
            {
                privilege = Repository.Create(new Privilege()
                {
                    Name = model.Name,
                    Description = model.Description,
                    IdentificationNumberPattern = model.IdentificationNumberPattern
                });
            }

            Repository.SaveChanges();

            model.Id = privilege.Id;
            return model;
        }
        private bool IsValidRegexPattern(string pattern)
        {
            try
            {
                _ = new Regex(pattern);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
