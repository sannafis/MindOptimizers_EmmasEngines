using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmmasEngines.Data.EmmasEnginesMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UPC = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Quantity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Current = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.ID);
                    table.UniqueConstraint("AK_Inventories_UPC", x => x.UPC);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePositions",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionTitle = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePositions", x => new { x.EmployeeID, x.PositionTitle });
                    table.ForeignKey(
                        name: "FK_EmployeePositions_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePositions_Positions_PositionTitle",
                        column: x => x.PositionTitle,
                        principalTable: "Positions",
                        principalColumn: "Title",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProvinceCode = table.Column<string>(type: "TEXT", nullable: false),
                    ProvinceID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Postal = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    ProvinceID = table.Column<int>(type: "INTEGER", nullable: true),
                    CityID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Customers_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Customers_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Postal = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    ProvinceID = table.Column<int>(type: "INTEGER", nullable: true),
                    CityID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Suppliers_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suppliers_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Appreciation = table.Column<double>(type: "REAL", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CustomerID = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReceiveDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExternalOrderNum = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    CustomerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderRequests_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InventoryUPC = table.Column<string>(type: "TEXT", nullable: false),
                    PurchasedCost = table.Column<double>(type: "REAL", nullable: false),
                    PurchasedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prices_Inventories_InventoryUPC",
                        column: x => x.InventoryUPC,
                        principalTable: "Inventories",
                        principalColumn: "UPC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prices_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryUPC = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SalePrice = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => new { x.InvoiceID, x.InventoryUPC });
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Inventories_InventoryUPC",
                        column: x => x.InventoryUPC,
                        principalTable: "Inventories",
                        principalColumn: "UPC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayments", x => new { x.InvoiceID, x.PaymentID });
                    table.ForeignKey(
                        name: "FK_InvoicePayments_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderRequestInventories",
                columns: table => new
                {
                    OrderRequestID = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryUPC = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRequestInventories", x => new { x.OrderRequestID, x.InventoryUPC });
                    table.ForeignKey(
                        name: "FK_OrderRequestInventories_Inventories_InventoryUPC",
                        column: x => x.InventoryUPC,
                        principalTable: "Inventories",
                        principalColumn: "UPC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderRequestInventories_OrderRequests_OrderRequestID",
                        column: x => x.OrderRequestID,
                        principalTable: "OrderRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceID",
                table: "Cities",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CityID",
                table: "Customers",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ProvinceID",
                table: "Customers",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePositions_EmployeeID_PositionTitle",
                table: "EmployeePositions",
                columns: new[] { "EmployeeID", "PositionTitle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePositions_PositionTitle",
                table: "EmployeePositions",
                column: "PositionTitle");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Username",
                table: "Employees",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_UPC",
                table: "Inventories",
                column: "UPC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InventoryUPC",
                table: "InvoiceLines",
                column: "InventoryUPC");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceID_InventoryUPC",
                table: "InvoiceLines",
                columns: new[] { "InvoiceID", "InventoryUPC" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_PaymentID",
                table: "InvoicePayments",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerID",
                table: "Invoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EmployeeID",
                table: "Invoices",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequestInventories_InventoryUPC",
                table: "OrderRequestInventories",
                column: "InventoryUPC");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequestInventories_OrderRequestID_InventoryUPC",
                table: "OrderRequestInventories",
                columns: new[] { "OrderRequestID", "InventoryUPC" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequests_CustomerID",
                table: "OrderRequests",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequests_ExternalOrderNum",
                table: "OrderRequests",
                column: "ExternalOrderNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Type",
                table: "Payments",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Title",
                table: "Positions",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_InventoryUPC",
                table: "Prices",
                column: "InventoryUPC");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_SupplierID",
                table: "Prices",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Code",
                table: "Provinces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Name",
                table: "Provinces",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CityID",
                table: "Suppliers",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ProvinceID",
                table: "Suppliers",
                column: "ProvinceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeePositions");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "InvoicePayments");

            migrationBuilder.DropTable(
                name: "OrderRequestInventories");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "OrderRequests");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
