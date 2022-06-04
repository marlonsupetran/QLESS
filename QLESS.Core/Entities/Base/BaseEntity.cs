using System;

namespace QLESS.Core.Entities
{
    public abstract class BaseEntity<T> : IEntity<T> where T : struct
    {
        public virtual T Id { get; set; }
    }
}
