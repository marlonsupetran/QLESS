using QLESS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLESS.BlazorServerApp.Administrator.Models
{
    public class CardTypeModel : ICardTypeModel
    {
        public Guid Id { get; set; }
        public Guid DiscountStrategyId { get; set; }

        [RegularExpression(
            "^((?!00000000-0000-0000-0000-000000000000).)*$",
            ErrorMessage = "Please select a fare scheme.")]
        public Guid FareStrategyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Range(0, int.MaxValue)]
        public decimal InitialBalance { get; set; }
        
        [Range(0, int.MaxValue)]
        public decimal MinimumBalance { get; set; }

        [Range(0, int.MaxValue)]
        public decimal MaximumBalance { get; set; }
        
        [Range(0, int.MaxValue)]
        public decimal MinimumReloadAmount { get; set; }
        
        [Range(0, int.MaxValue)]
        public decimal MaximumReloadAmount { get; set; }
        
        [Range(0, int.MaxValue)]
        public decimal BaseFare { get; set; }
        
        [Required]
        public long Validity { get; set; }

        public ICollection<Guid> PrivilegeIds { get; set; }
    }
}
