using QLESS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace QLESS.Core.Entities
{
    public class Privilege : BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string IdentificationNumberPattern { get; set; }

        public virtual ICollection<CardTypePrivilege> CardTypePrivileges { get; set; } = new List<CardTypePrivilege>();

        [NotMapped]
        public virtual ICollection<CardType> CardTypes
        {
            get => CardTypePrivileges.Select(c => c.CardType).ToList();
            set => CardTypePrivileges = new List<CardTypePrivilege>(value.Select(v => new CardTypePrivilege(v, this)));
        }
    }
}
