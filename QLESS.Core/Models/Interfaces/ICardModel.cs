using System;

namespace QLESS.Core.Models
{
    public interface ICardModel
    {
        Guid Number { get; set; }
        DateTime? Expiry { get; set; }
        decimal Balance { get; set; }
    }
}
