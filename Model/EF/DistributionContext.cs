using Microsoft.EntityFrameworkCore;

namespace DistributionAPI.Model
{
    public class DistributionContext : DbContext
    {
        public DbSet<DistributionData> distributionData { get; set; }
        public DbSet<Department> locationsData { get; set; }

        public DistributionContext(DbContextOptions<DistributionContext> options): base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
