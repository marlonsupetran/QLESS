using QLESS.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLESS.BlazorServerApp.Administrator.Models
{
    public class PrivilegeModel : IPrivilegeModel
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string IdentificationNumberPattern { get; set; }
    }
}
