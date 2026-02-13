using AutoMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Make> Makes => Set<Make>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            // ✅ Seed Makes
            builder.Entity<Make>().HasData(
                new Make { Id = 1, Name = "BMW" },
                new Make { Id = 2, Name = "Audi" },
                new Make { Id = 3, Name = "Toyota" }
            );

            // ✅ Seed CarModels
            builder.Entity<CarModel>().HasData(
                new CarModel { Id = 1, Name = "3 Series", MakeId = 1 },
                new CarModel { Id = 2, Name = "A4", MakeId = 2 },
                new CarModel { Id = 3, Name = "Corolla", MakeId = 3 }
            );
        }
    }
}
