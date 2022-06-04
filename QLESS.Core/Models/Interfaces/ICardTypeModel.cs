using System;
using System.Collections.Generic;

namespace QLESS.Core.Models
{
    public interface ICardTypeModel
    {
        Guid Id { get; set; }
        Guid FareStrategyId { get; set; }
        Guid DiscountStrategyId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        decimal InitialBalance { get; set; }
        decimal MinimumBalance { get; set; }
        decimal MaximumBalance { get; set; }
        decimal MinimumReloadAmount { get; set; }
        decimal MaximumReloadAmount { get; set; }
        decimal BaseFare { get; set; }
        long Validity { get; set; }
        ICollection<Guid> PrivilegeIds { get; set; }
    }
}