using QLESS.Core.Data;

namespace QLESS.Core.BusinessRules
{
    public abstract class BaseBusinessRules : IBusinessRules
    {
        // Properties
        public IRepository Repository { get; }

        // Constructors
        protected BaseBusinessRules(IRepository repository)
        {
            Repository = repository;
        }
    }
}
