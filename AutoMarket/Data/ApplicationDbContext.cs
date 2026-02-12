using AutoMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
   

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
        }
    }
}






//public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)