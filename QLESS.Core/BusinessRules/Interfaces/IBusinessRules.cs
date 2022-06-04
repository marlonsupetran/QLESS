using QLESS.Core.Data;

namespace QLESS.Core.BusinessRules
{
    public interface IBusinessRules
    {
        IRepository Repository { get; }
    }
}