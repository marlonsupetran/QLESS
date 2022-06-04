using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLESS.Core.Entities
{
    public class Card : BaseEntity<Guid>
    {
        public Guid Number { get; set; }

        // Properties
        public DateTime Created { get; set; }
        public DateTime? Expiry { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public virtual CardType Type { get; set; }
        public virtual PrivilegeCard PrivilegeCard { get; set; }
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
