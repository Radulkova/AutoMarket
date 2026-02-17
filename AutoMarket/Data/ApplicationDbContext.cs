using AutoMarket.Data.Models;
using AutoMarket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Make> Makes => Set<Make>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<CarImage> CarImages => Set<CarImage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // (по желание) връзки/ограничения ако искаш да са explicit
            builder.Entity<CarModel>()
                .HasOne(cm => cm.Make)
                .WithMany()
                .HasForeignKey(cm => cm.MakeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Car>()
                .HasOne(c => c.CarModel)
                .WithMany()
                .HasForeignKey(c => c.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);
          
            builder.Entity<CarImage>()
                .HasOne(ci => ci.Car)
                .WithMany(c => c.Images)
                .HasForeignKey(ci => ci.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarImage>()
                .Property(ci => ci.Url)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
