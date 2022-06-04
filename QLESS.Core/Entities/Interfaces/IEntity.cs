using System;

namespace QLESS.Core.Entities
{
    public interface IEntity<T> where T : struct
    {
        T Id { get; }
    }
}
