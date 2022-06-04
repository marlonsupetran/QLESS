using System;

namespace QLESS.Core.Models
{
    public class CardModel : ICardModel
    {
        public Guid Number { get; set; }
        public DateTime? Expiry { get; set; }
        public decimal Balance { get; set; }
    }
}
