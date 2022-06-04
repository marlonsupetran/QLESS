using System;
using System.Collections.Generic;

namespace QLESS.Core.Models
{
    public class CardTypeModel : ICardTypeModel
    {
        public Guid Id { get; set; }
        public Guid FareStrategyId { get; set; }
        public Guid? DiscountStrategyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal MaximumBalance { get; set; }
        public decimal MinimumReloadAmount { get; set; }
        public decimal MaximumReloadAmount { get; set; }
        public decimal BaseFare { get; set; }
        public long Validity { get; set; }
        public IEnumerable<Guid> PrivilegeIds { get; set; }
    }
}
