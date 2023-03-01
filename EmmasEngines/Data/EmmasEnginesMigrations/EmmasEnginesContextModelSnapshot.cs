﻿// <auto-generated />
using System;
using EmmasEngines.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmmasEngines.Data.EmmasEnginesMigrations
{
    [DbContext(typeof(EmmasEnginesContext))]
    partial class EmmasEnginesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.14");

            modelBuilder.Entity("EmmasEngines.Models.City", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProvinceCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProvinceID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("EmmasEngines.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int>("CityID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("TEXT");

                    b.Property<string>("Postal")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProvinceID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CityID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("EmmasEngines.Models.Employee", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("EmmasEngines.Models.EmployeePosition", b =>
                {
                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PositionTitle")
                        .HasColumnType("TEXT");

                    b.HasKey("EmployeeID", "PositionTitle");

                    b.HasIndex("PositionTitle");

                    b.HasIndex("EmployeeID", "PositionTitle")
                        .IsUnique();

                    b.ToTable("EmployeePositions");
                });

            modelBuilder.Entity("EmmasEngines.Models.Inventory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Current")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Quantity")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("UPC")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("UPC")
                        .IsUnique();

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("EmmasEngines.Models.Invoice", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Appreciation")
                        .HasColumnType("REAL");

                    b.Property<int>("CustomerID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("EmmasEngines.Models.InvoiceLine", b =>
                {
                    b.Property<int>("InvoiceID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InventoryUPC")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<double>("SalePrice")
                        .HasColumnType("REAL");

                    b.HasKey("InvoiceID", "InventoryUPC");

                    b.HasIndex("InventoryUPC");

                    b.HasIndex("InvoiceID", "InventoryUPC")
                        .IsUnique();

                    b.ToTable("InvoiceLines");
                });

            modelBuilder.Entity("EmmasEngines.Models.InvoicePayment", b =>
                {
                    b.Property<int>("InvoiceID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PaymentID")
                        .HasColumnType("INTEGER");

                    b.HasKey("InvoiceID", "PaymentID");

                    b.HasIndex("PaymentID");

                    b.ToTable("InvoicePayments");
                });

            modelBuilder.Entity("EmmasEngines.Models.OrderRequest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalOrderNum")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ReceiveDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("SentDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("ExternalOrderNum")
                        .IsUnique();

                    b.ToTable("OrderRequests");
                });

            modelBuilder.Entity("EmmasEngines.Models.OrderRequestInventory", b =>
                {
                    b.Property<int>("OrderRequestID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InventoryUPC")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderRequestID", "InventoryUPC");

                    b.HasIndex("InventoryUPC");

                    b.HasIndex("OrderRequestID", "InventoryUPC")
                        .IsUnique();

                    b.ToTable("OrderRequestInventories");
                });

            modelBuilder.Entity("EmmasEngines.Models.Payment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("EmmasEngines.Models.Position", b =>
                {
                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Title");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("EmmasEngines.Models.Price", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("InventoryUPC")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("PurchasedCost")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("PurchasedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Stock")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SupplierID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("InventoryUPC");

                    b.HasIndex("SupplierID");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("EmmasEngines.Models.Province", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("EmmasEngines.Models.Supplier", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int>("CityID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("TEXT");

                    b.Property<string>("Postal")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProvinceID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CityID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("EmmasEngines.Models.City", b =>
                {
                    b.HasOne("EmmasEngines.Models.Province", "Province")
                        .WithMany("Cities")
                        .HasForeignKey("ProvinceID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Province");
                });

            modelBuilder.Entity("EmmasEngines.Models.Customer", b =>
                {
                    b.HasOne("EmmasEngines.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Province", "Province")
                        .WithMany()
                        .HasForeignKey("ProvinceID");

                    b.Navigation("City");

                    b.Navigation("Province");
                });

            modelBuilder.Entity("EmmasEngines.Models.EmployeePosition", b =>
                {
                    b.HasOne("EmmasEngines.Models.Employee", "Employee")
                        .WithMany("EmployeePositions")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Position", "Position")
                        .WithMany("EmployeePositions")
                        .HasForeignKey("PositionTitle")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("EmmasEngines.Models.Invoice", b =>
                {
                    b.HasOne("EmmasEngines.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("EmmasEngines.Models.InvoiceLine", b =>
                {
                    b.HasOne("EmmasEngines.Models.Inventory", "Inventory")
                        .WithMany("InvoiceLines")
                        .HasForeignKey("InventoryUPC")
                        .HasPrincipalKey("UPC")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Invoice", "Invoice")
                        .WithMany("InvoiceLines")
                        .HasForeignKey("InvoiceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("EmmasEngines.Models.InvoicePayment", b =>
                {
                    b.HasOne("EmmasEngines.Models.Invoice", "Invoice")
                        .WithMany("InvoicePayments")
                        .HasForeignKey("InvoiceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Payment", "Payment")
                        .WithMany("InvoicePayments")
                        .HasForeignKey("PaymentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Invoice");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("EmmasEngines.Models.OrderRequest", b =>
                {
                    b.HasOne("EmmasEngines.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("EmmasEngines.Models.OrderRequestInventory", b =>
                {
                    b.HasOne("EmmasEngines.Models.Inventory", "Inventory")
                        .WithMany("OrderRequestInventories")
                        .HasForeignKey("InventoryUPC")
                        .HasPrincipalKey("UPC")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.OrderRequest", "OrderRequest")
                        .WithMany()
                        .HasForeignKey("OrderRequestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");

                    b.Navigation("OrderRequest");
                });

            modelBuilder.Entity("EmmasEngines.Models.Price", b =>
                {
                    b.HasOne("EmmasEngines.Models.Inventory", "Inventory")
                        .WithMany("Prices")
                        .HasForeignKey("InventoryUPC")
                        .HasPrincipalKey("UPC")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Supplier", "Supplier")
                        .WithMany("Prices")
                        .HasForeignKey("SupplierID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Inventory");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("EmmasEngines.Models.Supplier", b =>
                {
                    b.HasOne("EmmasEngines.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmmasEngines.Models.Province", "Province")
                        .WithMany()
                        .HasForeignKey("ProvinceID");

                    b.Navigation("City");

                    b.Navigation("Province");
                });

            modelBuilder.Entity("EmmasEngines.Models.Employee", b =>
                {
                    b.Navigation("EmployeePositions");
                });

            modelBuilder.Entity("EmmasEngines.Models.Inventory", b =>
                {
                    b.Navigation("InvoiceLines");

                    b.Navigation("OrderRequestInventories");

                    b.Navigation("Prices");
                });

            modelBuilder.Entity("EmmasEngines.Models.Invoice", b =>
                {
                    b.Navigation("InvoiceLines");

                    b.Navigation("InvoicePayments");
                });

            modelBuilder.Entity("EmmasEngines.Models.Payment", b =>
                {
                    b.Navigation("InvoicePayments");
                });

            modelBuilder.Entity("EmmasEngines.Models.Position", b =>
                {
                    b.Navigation("EmployeePositions");
                });

            modelBuilder.Entity("EmmasEngines.Models.Province", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("EmmasEngines.Models.Supplier", b =>
                {
                    b.Navigation("Prices");
                });
#pragma warning restore 612, 618
        }
    }
}
