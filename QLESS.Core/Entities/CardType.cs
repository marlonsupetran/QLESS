using QLESS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace QLESS.Core.Entities
{
    public class CardType : BaseEntity<Guid>
    {
        public Guid FareStrategyId { get; set; }
        public Guid DiscountStrategyId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(255)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumReloadAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumReloadAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseFare { get; set; }

        public long Validity { get; set; }
        public virtual ICollection<CardTypePrivilege> CardTypePrivileges { get; set; } = new List<CardTypePrivilege>();
        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

        [NotMapped]
        public virtual ICollection<Privilege> Privileges 
        {
            get => CardTypePrivileges.Select(c => c.Privilege).ToList();
            set => CardTypePrivileges = new List<CardTypePrivilege>(value.Select(v => new CardTypePrivilege(this, v)));
        }
    }
}
