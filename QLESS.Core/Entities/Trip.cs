using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLESS.Core.Entities
{
    public class Trip : BaseEntity<Guid>
    {
        public DateTime Entry { get; set; }
        public DateTime? Exit { get; set; }
        public int EntryStationNumber { get; set; }
        public int ExitStationNumber { get; set; }
    }
}
