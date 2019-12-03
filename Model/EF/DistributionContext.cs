using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DistributionAPI.Model;

namespace DistributionAPI.Model
{
    public class DistributionContext : DbContext
    {
        public DbSet<DistributionData> distributionData { get; set; }
        public DbSet<LocationStack> locationsData { get; set; }

        public DistributionContext(DbContextOptions<DistributionContext> options): base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

    }
}
