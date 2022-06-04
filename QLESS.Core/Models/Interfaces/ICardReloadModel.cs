namespace QLESS.Core.Models
{
    public interface ICardReloadModel : ICardModel
    {
        decimal Payment { get; set; }
        decimal Amount { get; set; }
        decimal Change { get; set; }
    }
}
