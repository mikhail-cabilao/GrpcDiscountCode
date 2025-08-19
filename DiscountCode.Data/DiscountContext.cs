using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCode.Data
{
    internal class DiscountContext : DbContext
    {
        public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();

        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscountCode>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique(); // hard uniqueness guarantee across restarts/instances
            });
        }
    }

}
