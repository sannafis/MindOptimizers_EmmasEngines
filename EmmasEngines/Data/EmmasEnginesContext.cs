using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using EmmasEngines.Models;
using System.Numerics;
//using EmmasEngines.ViewModels;

namespace EmmasEngines.Data
{
    public class EmmasEnginesContext : DbContext
    {
        public EmmasEnginesContext(DbContextOptions<EmmasEnginesContext> options) : base(options)
        {

        }

        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add a unique index to the Inventory UPC
            modelBuilder.Entity<Inventory>()
            .HasIndex(p => p.UPC)
            .IsUnique();

            //Add a unique index to the Province Code
            modelBuilder.Entity<Province>()
            .HasIndex(p => p.Code)
            .IsUnique();

            //Add a unique index to the Province Name
            modelBuilder.Entity<Province>()
            .HasIndex(p => p.Name)
            .IsUnique();

            //Prevent Cascade Delete from Supplier to Price
            modelBuilder.Entity<Supplier>()
                .HasMany<Price>(p => p.Prices)
                .WithOne(c => c.Supplier)
                .HasForeignKey(c => c.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Inventory to Price
            modelBuilder.Entity<Inventory>()
                .HasMany<Price>(p => p.Prices)
                .WithOne(c => c.Inventory)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Province to City
            modelBuilder.Entity<Province>()
                .HasMany<City>(p => p.Cities)
                .WithOne(c => c.Province)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
