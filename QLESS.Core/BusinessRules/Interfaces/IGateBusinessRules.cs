using System;

namespace QLESS.Core.BusinessRules
{
    public interface IGateBusinessRules
    {
        void Enter(Guid cardNumber, int entryStationNumber);
        void Exit(Guid cardNumber, int exitStationNumber);
    }
}