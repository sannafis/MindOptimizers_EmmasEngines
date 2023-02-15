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

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }


        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<OrderRequestInventory> OrderRequestInventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add a unique index to the Inventory UPC
            modelBuilder.Entity<Inventory>()
            .HasIndex(p => p.UPC)
            .IsUnique();

            //1:many between inventory and prices
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.Prices)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);

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

            //Postion Key set to Title
            modelBuilder.Entity<Position>()
            .HasKey(p => p.Title);

            //Many to Many Primary Key
            modelBuilder.Entity<EmployeePosition>()
            .HasKey(p => new { p.EmployeeID, p.PositionTitle });

            modelBuilder.Entity<InvoicePayment>()
            .HasKey(p => new { p.InvoiceID, p.PaymentID });

            modelBuilder.Entity<InvoiceLine>()
            .HasKey(p => new { p.InvoiceID, p.InventoryUPC });

            modelBuilder.Entity<OrderRequestInventory>()
            .HasKey(p => new { p.OrderRequestID, p.InventoryUPC });

            //Add a unique index to the Position Title
            modelBuilder.Entity<Position>()
            .HasIndex(p => p.Title)
            .IsUnique();

            //Add a unique index to the Employee Username
            modelBuilder.Entity<Employee>()
            .HasIndex(p => p.Username)
            .IsUnique();

            //Add a unique index to the Payment Type
            modelBuilder.Entity<Payment>()
            .HasIndex(p => p.Type)
            .IsUnique();

            //Add a unique index to the OrderRequest Number
            modelBuilder.Entity<OrderRequest>()
            .HasIndex(p => p.ExternalOrderNum)
            .IsUnique();

            //Add a unique composite index to Employee Position
            modelBuilder.Entity<EmployeePosition>()
            .HasIndex(p => new { p.EmployeeID, p.PositionTitle})
            .IsUnique();

            //Add a unique composite index to Invoice Line
            modelBuilder.Entity<InvoiceLine>()
            .HasIndex(p => new { p.InvoiceID, p.InventoryUPC })
            .IsUnique();

            //Add a unique composite index to OrderRequestInventory
            modelBuilder.Entity<OrderRequestInventory>()
            .HasIndex(p => new { p.OrderRequestID, p.InventoryUPC })
            .IsUnique();

            //Prevent Cascade Delete from Position to EmployeePosition (Parent Perspective)
            modelBuilder.Entity<Position>()
                .HasMany<EmployeePosition>(p => p.EmployeePositions)
                .WithOne(c => c.Position)
                .HasForeignKey(c => c.PositionTitle)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Payment to InvoicePayment (Parent Perspective)
            modelBuilder.Entity<Payment>()
                .HasMany<InvoicePayment>(p => p.InvoicePayments)
                .WithOne(c => c.Payment)
                .HasForeignKey(c => c.PaymentID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Inventory to InvoiceLine (Parent Perspective)
            modelBuilder.Entity<Inventory>()
                .HasMany<InvoiceLine>(p => p.InvoiceLines)
                .WithOne(c => c.Inventory)
                .HasForeignKey(c => c.InventoryUPC)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Inventory to OrderRequestInventory (Parent Perspective)
            modelBuilder.Entity<Inventory>()
                .HasMany<OrderRequestInventory>(p => p.OrderRequestInventories)
                .WithOne(c => c.Inventory)
                .HasForeignKey(c => c.InventoryUPC)
                .OnDelete(DeleteBehavior.Restrict);

            //1:many between inventory and invoiceline
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.InvoiceLines)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);

            //1:many between inventory and OrderRequestInventory
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.OrderRequestInventories)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);
        }
    }
}
