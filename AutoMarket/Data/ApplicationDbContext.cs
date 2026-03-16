using AutoMarket.Models;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany()
                .HasForeignKey(o => o.CarId)
                .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<Car>()
                .HasOne<IdentityUser>(c => c.Seller)
                .WithMany()
                .HasForeignKey(c => c.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.CarId });

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Favorite>()
                .HasOne(f => f.Car)
                .WithMany(c => c.Favorites)
                .HasForeignKey(f => f.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Favorite>()
                .Property(f => f.CreatedOn)
                .IsRequired();
        }
    }
}