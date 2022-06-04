using System;

namespace QLESS.Core.Models
{
    public class PrivilegeModel : IPrivilegeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IdentificationNumberPattern { get; set; }
    }
}
