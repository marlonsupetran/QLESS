using System;

namespace QLESS.Core.Models
{
    public class CardReloadModel : CardModel, ICardReloadModel
    {
        public decimal Payment { get; set; }
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
    }
}
