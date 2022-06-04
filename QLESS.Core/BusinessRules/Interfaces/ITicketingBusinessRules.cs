using QLESS.Core.Models;
using System;

namespace QLESS.Core.BusinessRules
{
    public interface ITicketingBusinessRules
    {
        ICardModel Activate(Guid cardNumber, Guid cardTypeId, Guid privilegeId, string privilegeCardNumber);
        ICardModel CheckBalance(Guid cardNumber);
        ICardReloadModel Reload(Guid cardNumber, decimal amount, decimal payment);
    }
}
