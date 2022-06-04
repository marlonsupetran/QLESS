using QLESS.Core.Models;

namespace QLESS.Core.BusinessRules
{
    public interface IAdministratorBusinessRules
    {
        ICardTypeModel CreateOrEditCardType(ICardTypeModel model);
        IPrivilegeModel CreateOrEditPriviledge(IPrivilegeModel model);
    }
}
