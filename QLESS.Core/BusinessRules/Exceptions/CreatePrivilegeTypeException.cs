using System;

namespace QLESS.Core.BusinessRules
{
    public class CreatePriviledgeException : ArgumentException
    {
        public CreatePriviledgeException() { }
        public CreatePriviledgeException(string message) : base(message) { }
        public CreatePriviledgeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
