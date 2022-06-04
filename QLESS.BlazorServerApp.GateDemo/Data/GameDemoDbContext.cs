using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QLESS.Core.Entities;
using System.Linq;

namespace QLESS.Core.Data.EntityFramework
{
    public class GameDemoDbContext : EntityFrameworkDbContext
    {
        // Constructors
        public GameDemoDbContext(DbContextOptions<GameDemoDbContext> options) : base(options) { }
    }
}
