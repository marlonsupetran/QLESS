using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QLESS.Core.Entities;
using System.Linq;

namespace QLESS.Core.Data.EntityFramework
{
    public class AdministratorDbContext : EntityFrameworkDbContext
    {
        // Constructors
        public AdministratorDbContext(DbContextOptions<DbContext> options) : base(options) { }
    }
}
