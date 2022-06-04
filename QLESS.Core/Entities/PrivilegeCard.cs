using System;

namespace QLESS.Core.Entities
{
    public class PrivilegeCard : BaseEntity<Guid>
    {
        public string IdentificationNumber { get; set; }
        public virtual Privilege Type { get; set; }
    }
}
