using System;

namespace QLESS.Core.Data
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
    }
}
