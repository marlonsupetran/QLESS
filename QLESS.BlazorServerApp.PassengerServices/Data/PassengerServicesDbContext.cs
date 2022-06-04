using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QLESS.Core.Entities;
using System.Linq;

namespace QLESS.Core.Data.EntityFramework
{
    public class PassengerServicesDbContext : EntityFrameworkDbContext
    {
        // Constructors
        public PassengerServicesDbContext(DbContextOptions<PassengerServicesDbContext> options) : base(options) { }
    }
}
