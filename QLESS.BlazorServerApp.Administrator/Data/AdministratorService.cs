using QLESS.Core.Data;
using QLESS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLESS.BlazorServerApp.Administrator.Data
{
    public class AdministratorService
    {
        // Properties
        public IRepository Repository { get; }
        public List<KeyValuePair<string, long>> ValidityOptions { get; }

        // Constructors
        public AdministratorService(IRepository repository)
        {
            Repository = repository;

            var today = DateTime.Today;
            ValidityOptions = (new[]
            {
                new KeyValuePair<string, long>("3 months", (today.AddMonths(3) - today).Ticks),
                new KeyValuePair<string, long>("6 months", (today.AddMonths(6) - today).Ticks),
                new KeyValuePair<string, long>("1 year", (today.AddYears(1) - today).Ticks),
                new KeyValuePair<string, long>("3 years", (today.AddYears(3) - today).Ticks),
                new KeyValuePair<string, long>("5 years", (today.AddYears(5) - today).Ticks)
            }).ToList();
        }

        // Methods
        public Task<CardType[]> GetCardTypesAsync()
        {
            return Task.FromResult(Repository.Read<CardType>().ToArray());
        }
        public Task<CardType> GetCardType(Guid id)
        {
            return Task.FromResult(Repository.Read<CardType, Guid>(id));
        }
        public void DeleteCardType(Guid id)
        {
            Repository.Delete<CardType, Guid>(id);
            Repository.SaveChanges();
        }
        public Task<Privilege[]> GetPrivilegesAsync()
        {
            return Task.FromResult(Repository.Read<Privilege>().ToArray());
        }
        public Task<Privilege> GetPrivilege(Guid id)
        {
            return Task.FromResult(Repository.Read<Privilege, Guid>(id));
        }
        public void DeletePrivilege(Guid id)
        {
            Repository.Delete<Privilege, Guid>(id);
            Repository.SaveChanges();
        }

    }
}
