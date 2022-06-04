using System;

namespace QLESS.Core.Models
{
    public interface IPrivilegeModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string IdentificationNumberPattern { get; set; }
    }
}
