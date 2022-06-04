using System;
using System.Collections.Generic;
using System.Text;

namespace QLESS.Core.Entities
{
    public class CardTypePrivilege
    {
        // Properties
        public Guid CardTypeId { get; set; }
        public Guid PrivilegeId { get; set; }
        public virtual CardType CardType { get; set; }
        public virtual Privilege Privilege { get; set; }

        // Constructors
        public CardTypePrivilege() { }
        public CardTypePrivilege(CardType cardType, Privilege privilege)
        {
            CardType = cardType;
            CardTypeId = cardType.Id;
            Privilege = privilege;
            PrivilegeId = privilege.Id;
        }
    }
}
