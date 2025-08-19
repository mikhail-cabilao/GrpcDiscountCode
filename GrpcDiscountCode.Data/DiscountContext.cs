using GrpcDiscountCode.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDiscountCode.Data
{
    public class DiscountContext : DbContext
    {
        public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();

        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscountCode>(e =>
            {
                e.ToTable("DiscountCodes");
                e.HasIndex(x => x.Code).IsUnique();
            });
        }
    }
}
