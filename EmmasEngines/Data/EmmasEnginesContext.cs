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
        public DbSet<EmployeeLogin> EmployeeLogins { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }


        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<OrderRequestInventory> OrderRequestInventories { get; set; }

        public DbSet<Report> Reports { get; set; }
        public DbSet<SalesReport> SalesReports { get; set; }
        public DbSet<SalesReportEmployee> SalesReportEmployees { get; set; }
        public DbSet<SalesReportInventory> SalesReportInventories { get; set; }
        public DbSet<HourlyReport> HourlyReports { get; set; }
        public DbSet<COGSReport> COGSReports { get; set; }


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

            //Prevent Cascade Delete from Supplier to Inventory
            /* modelBuilder.Entity<Supplier>()
                .HasMany<Inventory>(p => p.Inventories)
                .WithOne(c => c.Supplier)
                .HasForeignKey(c => c.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);
			*/

            //Prevent Cascade Delete from Inventory to Price
            modelBuilder.Entity<Inventory>()
                .HasMany<Price>(p => p.Prices)
                .WithOne(c => c.Inventory)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Supplier to OrderRequests
            modelBuilder.Entity<Supplier>()
                .HasMany<OrderRequest>(p => p.OrderRequests)
                .WithOne(c => c.Supplier)
                .HasForeignKey(c => c.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Province to City
            modelBuilder.Entity<Province>()
                .HasMany<City>(p => p.Cities)
                .WithOne(c => c.Province)
                .OnDelete(DeleteBehavior.Restrict);

            //Position Key set to Title
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

            //Add a unique index to the Employee User name
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

            //1:many between inventory and prices
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.Prices)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);

            //1:many between OrderRequestInventory and OrderRequest
            modelBuilder.Entity<OrderRequest>()
                .HasMany(p => p.OrderRequestInventories)
                .WithOne(i => i.OrderRequest)
                .HasForeignKey(i => i.OrderRequestID)
                .OnDelete(DeleteBehavior.Restrict);

            /// -------- REPORTS -------- ///

            // Many:many between COGSReport and Inventories
            modelBuilder.Entity<COGSReport>()
                .HasMany<Inventory>(s => s.Inventories)
                .WithMany(c => c.COGSReports)
                .UsingEntity(j => j.ToTable("COGSReportInventory"));

            // Many:many between COGSReport and Invoices
            modelBuilder.Entity<COGSReport>()
                .HasMany<Invoice>(s => s.Invoices)
                .WithMany(c => c.COGSReports)
                .UsingEntity(j => j.ToTable("COGSReportInvoice"));

            // Many:many between HourlyReport and Employees
            modelBuilder.Entity<HourlyReport>()
                .HasMany<Employee>(s => s.Employees)
                .WithMany(c => c.HourlyReports)
                .UsingEntity(j => j.ToTable("HourlyReportEmployee"));

            //1:many between inventory and SalesReport_Inventories
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.SalesReportInventories)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);

            //1:many between inventory and SalesReport_Inventories
            modelBuilder.Entity<Inventory>()
                .HasMany(p => p.SalesReportInventories)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryUPC)
                .HasPrincipalKey(p => p.UPC);

            //1:many between SalesReport and SalesReport_Inventories
            modelBuilder.Entity<SalesReport>()
                .HasMany(p => p.SalesReportInventories)
                .WithOne(i => i.SalesReport)
                .HasForeignKey(i => i.SalesReportID);

            //1:many between SalesReport and SalesReport_Employee
            modelBuilder.Entity<SalesReport>()
                .HasMany(p => p.SalesReportEmployees)
                .WithOne(i => i.SalesReport)
                .HasForeignKey(i => i.SalesReportID);
        }
    }
}
