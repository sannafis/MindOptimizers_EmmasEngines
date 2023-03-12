using EmmasEngines.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace EmmasEngines.Data
{
    public class EmmasEnginesInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            EmmasEnginesContext context = applicationBuilder.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<EmmasEnginesContext>();

            try
            {
                //context.Database.EnsureDeleted();
                //Create the database if it does not exist
                context.Database.Migrate();

                //Initialize Data
                // random number gen
                Random random = new Random();

                //Province 
                if (!context.Provinces.Any())
                {
                    var provinces = new (string Code, string Name)[]{("AB", "Alberta"), ("BC", "British Columbia"), ("MB", "Manitoba"), ("NB", "New Brunswick"), ("NL", "Newfoundland & Labrador"), ("NT", "Northwest Territories"), ("NS", "Nova Scotia"), ("NU", "Nunavut"), ("ON", "Ontario"), ("PE", "Prince Edward Island"), ("QB", "Quebec"), ("SK", "Saskatchewan"), ("YT", "Yukon")};

                    foreach (var p in provinces)
                    {
                        Province prov = new Province()
                        {
                            Name = p.Name,
                            Code = p.Code
                        };
                        context.Provinces.Add(prov);
                    }
                    context.SaveChanges();
                }

                //City
                if (!context.Cities.Any())
                {
                    string[] ontario = new string[] { "Toronto", "North York", "Scarbrough", "London", "Brampton", "Mississauga", "Ottawa", "Hamilton", "Etobicoke", "Barrie", "Belmont","Grimsby","Guelph","Kanata","Kitchener","Niagara Falls","Niagara-on-the-Lake", "Port Colborne","Sault Ste. Marie","Simcoe","St. Catharines","Thorold","Thunder Bay"};
                    string[] britishColumbia = new string[] { "Vancouver", "Victoria", "Surrey", "Richmond", "Kelowna", "Delta", "Coquitlam", "Burnaby", "Prince George", "Chilliwack" };
                    string[] alberta = new string[] { "Edmonton", "Red Deer", "Grand Prarie", "St. Albert", "Airdrie", "Medicine Hat", "Calgary","Fort McMurray", "Lethbridge" };
                    string[] manitoba = new string[] { "Winnipeg", "Portage la Prarie", "Thompson", "Brandon", "Dauphin", "Steinbach", "Winkler", "St. Andrews", "Belair", "Oakburn" };
                    string[] newBrunswick = new string[] { "Saint John", "Fredericton", "Riverview", "Bathurst", "Edmundston", "Moncton", "Miramichi", "Dieppe", "Quispamsis", "Rothesay" };
                    string[] novaScotia = new string[] { "Halifax", "Sydney", "New Glasgow", "Truro", "Dartmouth", "Glace Bay", "Amherst" };
                    string[] nunavut = new string[] { "Iqaluit", "Naujaat", "Gjoa Haven", "Taloyoak", "Arviat", "Cambridge Bay", "Kugluktuk", "Kugaaruk", "Baker Lake", "Arctic Bay" };
                    string[] newfoundland = new string[] { "St. John's", "Corner Brook", "Paradise", "Gander", "Stephenville", "Labrador City" };
                    string[] northwest = new string[] { "Yellowknife", "Fort Simpson", "Tulita", "Behchoko", "Ulukhaktok", "Norman Wells", "Aklavik" };
                    string[] pei = new string[] { "Charlottetown", "Stratford", "Mermaid", "Summerside", "Marshfield" };
                    string[] saskatchewan = new string[] { "Regina", "Moose Jaw", "Swift Current", "Saskatoon", "Prince Albert", "Yorkton", "Weyburn", "Lloydminster", "Estevan"};
                    string[] yukon = new string[] { "Whitehorse", "Marsh Lake", "Dawson", "Carcross", "Faro", "Mayo", "Destruction Bay", "Teslin" };

                    int[] provIDs = context.Provinces.Select(g => g.ID).ToArray();
                    int genreIDCount = provIDs.Length;

                    foreach (var i in ontario)
                    {
                        City city = new City(){ Name = i, ProvinceCode = "ON"};
                        context.Cities.Add(city);
                    }

                    foreach (var i in britishColumbia)
                    {
                        City city = new City() { Name = i, ProvinceCode = "BC" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in alberta)
                    {
                        City city = new City() { Name = i, ProvinceCode = "AB" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in manitoba)
                    {
                        City city = new City() { Name = i, ProvinceCode = "MB" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in newBrunswick)
                    {
                        City city = new City() { Name = i, ProvinceCode = "NB" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in novaScotia)
                    {
                        City city = new City() { Name = i, ProvinceCode = "NS" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in nunavut)
                    {
                        City city = new City() { Name = i, ProvinceCode = "NU" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in newfoundland)
                    {
                        City city = new City() { Name = i, ProvinceCode = "NL" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in northwest)
                    {
                        City city = new City() { Name = i, ProvinceCode = "NT" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in pei)
                    {
                        City city = new City() { Name = i, ProvinceCode = "PE" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in saskatchewan)
                    {
                        City city = new City() { Name = i, ProvinceCode = "SK" };
                        context.Cities.Add(city);
                    }

                    foreach (var i in yukon)
                    {
                        City city = new City() { Name = i, ProvinceCode = "YT" };
                        context.Cities.Add(city);
                    }
                    context.SaveChanges();
                }

                

                int[] cityIDs = context.Cities.Select(x => x.ID).ToArray();

                //Supplier 
                if (!context.Suppliers.Any())
                {
                    context.Suppliers.AddRange(
                     new Supplier
                     {
                         Name = "Kohl Inc.",
                         Phone = "289-989-1909",
                         Email = "kohler@domain.ca",
                         Address = "123 Kohl St",
                         Postal = "L3C 7A7",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"Great Lake Suppliers",
                         Phone = "519-111-1111",
                         Email = "greatsupply@domain.ca",
                         Address = "23 Huron Street",
                         Postal = "N2B 2P5",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"DEA Suppliers",
                         Phone = "902-439-1277",
                         Email = "deasupply@domain.ca",
                         Address = "4 Windy Lane",
                         Postal = "M4F 4J8",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"Prime Supplier",
                         Phone = "345-233-6665",
                         Email = "prime@domain.ca",
                         Address = "32 Optimus Road",
                         Postal = "M5V 1G3",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"Solid Lawns Inc.",
                         Phone = "534-333-1231",
                         Email = "solid@domain.ca",
                         Address = "456 Snake Blvd",
                         Postal = "M6S 7O7",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"Ontario Engines",
                         Phone = "453-234-4455",
                         Email = "onengines@domain.ca",
                         Address = "45 Rose St",
                         Postal = "G6J 5H3",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Supplier
                     {
                         Name = $"King of Engines",
                         Phone = "453-333-2345",
                         Email = "engineking@domain.ca",
                         Address = "1 Terry St",
                         Postal = "K6V 4H4",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     });
                    context.SaveChanges();
                }

                int[] suppIDs = context.Suppliers.Select(x => x.ID).ToArray();

                //Inventory 
                if (!context.Inventories.Any())
                {
                    context.Inventories.AddRange(
                     new Inventory
                     {
                         UPC = $"060-5910-0",
                         Name = "Mower Blade",
                         Size = "(S) - 8\" Length x 4\" Width",
                         Quantity = "3-Pack",
                         Current = true,
                         SupplierID = 5
                     }, new Inventory
                     {
                         UPC = $"063-5210-5",
                         Name = "Saw Blade",
                         Size = "(L) - 12\" Length x 5\" Width",
                         Quantity = "3-Pack",
                         Current = true,
                         SupplierID = 4
                     }, new Inventory
                     {
                         UPC = $"060-7007-0",
                         Name = "Atlas Lawnmower Engine Brake Cable",
                         Size = "54\" (137 cm) cable",
                         Quantity = "1",
                         Current = true,
                         SupplierID = 3
                     }, new Inventory
                     {
                         UPC = $"060-7410-3",
                         Name = "MTD Replacement Blade Adapter",
                         Size = "Fits 7/8\" crankshaft with 3/16\" key",
                         Quantity = "1",
                         Current = true,
                         SupplierID = 2
                     }, new Inventory
                     {
                         UPC = $"060-6410-4",
                         Name = "Champion 224cc OHV Horizontal Gas Engine",
                         Size = "Shaft dimensions (D x L): 2.4 D x 3/4\" D",
                         Quantity = "1",
                         Current = true,
                         SupplierID = 6
                     });
                    context.SaveChanges();
                }

                string[] invUPCs = context.Inventories.Select(x => x.UPC).ToArray();
                DateTime startDate = DateTime.Today;

                double priceMin = 20.00d;
                double priceMax = 40.99d;

                //Order Request Inventories
                if (!context.Prices.Any())
                {

                    int k = 0;//Start with the first inventory
                    foreach (string i in invUPCs)
                    {
                        int howMany = random.Next(1, invUPCs.Count());//add a few inventories to a order
                        for (int j = 1; j <= howMany; j++)
                        {
                            k = (k >= invUPCs.Count()) ? 0 : k;//Resets counter k to 0 if we have run out of inventory
                            Price o = new Price()
                            {
                                Stock = random.Next(20,51),
                                InventoryUPC = invUPCs[k],
                                PurchasedCost = random.NextDouble() * (priceMax - priceMin) + priceMin,
                                PurchasedDate = startDate.AddDays(-random.Next(30, 365))
                            };
                            context.Prices.Add(o);
                            k++;
                        }
                    }
                    context.SaveChanges();
                }


                //Customer 
                if (!context.Customers.Any())
                {
                    context.Customers.AddRange(
                     new Customer
                     {
                         FirstName = "Bob",
                         LastName = "Smith",
                         Phone = "445-677-5669",
                         Address = "111 Morning St.",
                         Postal = "K3F 4V7",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Simon",
                         LastName = "Belmont",
                         Phone = "134-435-5678",
                         Address = "7 Cross St.",
                         Postal = "J8D 2F4",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Sarah",
                         LastName = "Lam",
                         Phone = "566-345-7785",
                         Address = "23 Maple Road",
                         Postal = "L9C 1F1",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Adrian",
                         LastName = "Tepes",
                         Phone = "666-777-3456",
                         Address = "1 Valman Way",
                         Postal = "R4G 1K9",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Hal",
                         LastName = "Emmerich",
                         Phone = "983-123-7722",
                         Address = "34 Gear Ave.",
                         Postal = "K3F 4V7",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Terry",
                         LastName = "Bogard",
                         Phone = "111-663-3457",
                         Address = "98 Fury Road",
                         Postal = "K1N 6O4",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Bob",
                         LastName = "Smith",
                         Phone = "445-677-5669",
                         Address = "111 Morning St.",
                         Postal = "K3F 4V7",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Tina",
                         LastName = "Ochoa",
                         Phone = "982-463-2356",
                         Address = "2 Trent Ave.",
                         Postal = "J3S 5V8",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     }, new Customer
                     {
                         FirstName = "Timothy",
                         LastName = "Lee",
                         Phone = "355-466-7755",
                         Address = "12 Somewhere Road",
                         Postal = "L7N 9B2",
                         CityID = cityIDs[random.Next(cityIDs.Count())]
                     });
                    context.SaveChanges();
                }

                //Employee 
                if (!context.Employees.Any())
                {
                    context.Employees.AddRange(
                     new Employee
                     {
                         FirstName = "Emma",
                         LastName = "Henderson",
                         Username = "EmmaH",
                         Password = "123456789"
                     }, new Employee
                     {
                         FirstName = "Sam",
                         LastName = "Khan",
                         Username = "samk",
                         Password = "123456789"
                     }, new Employee
                     {
                         FirstName = "Wendy",
                         LastName = "Barlowe",
                         Username = "WendyB",
                         Password = "123456789"
                     }, new Employee
                     {
                         FirstName = "William",
                         LastName = "Tanner",
                         Username = "WilliamT",
                         Password = "123456789"
                     }, new Employee
                     {
                         FirstName = "Emily",
                         LastName = "Anderson",
                         Username = "EmilyA",
                         Password = "123456789"
                     });
                    context.SaveChanges();
                }

                //Position 
                if (!context.Positions.Any())
                {
                    context.Positions.AddRange(
                     new Position
                     {
                         Title = "Owner"
                     }, new Position
                     {
                         Title = "Ordering"
                     },new Position
                     {
                         Title = "Sales"
                     },new Position
                     {
                         Title = "Admin"
                     });
                    context.SaveChanges();
                }

                //Employee Position 
                if (!context.EmployeePositions.Any())
                {
                    context.EmployeePositions.AddRange(
                     new EmployeePosition
                     {
                         EmployeeID = 1,
                         PositionTitle = "Owner"
                     }, new EmployeePosition
                     {
                         EmployeeID = 2,
                         PositionTitle = "Ordering"
                     }, new EmployeePosition
                     {
                         EmployeeID = 3,
                         PositionTitle = "Sales"
                     }, new EmployeePosition
                     {
                         EmployeeID = 4,
                         PositionTitle = "Sales"
                     }, new EmployeePosition
                     {
                         EmployeeID = 5,
                         PositionTitle = "Admin"
                     });
                    context.SaveChanges();
                }

                int[] customerIDs = context.Customers.Select(x => x.ID).ToArray();
                //Order Request 
                if (!context.OrderRequests.Any())
                {
                    DateTime start = new DateTime(2022, 1, 1);
                    int range = (new DateTime(2023, 1, 1) - start).Days;
                    for (int i = 0; i < 10; i++) {
                        DateTime sendDate = start.AddDays(random.Next(range));
                         OrderRequest order = new OrderRequest()
                         {
                             Description = "Inventory Stock Order",
                             SentDate = sendDate,
                             ReceiveDate = (i%2 == 0? sendDate.AddDays(random.Next(10, 30)) : null), 
                             ExternalOrderNum = (100 + i).ToString(),
                             CustomerID = customerIDs[random.Next(customerIDs.Count())],
                             SupplierID = suppIDs[random.Next(suppIDs.Count())]
                         };
                        context.OrderRequests.Add(order);
                    }
                    context.SaveChanges();
                }

                string[] inventoryUPCs = context.Inventories.Select(x => x.UPC).ToArray();
                int[] orderIDs = context.OrderRequests.Select(x => x.ID).ToArray();

                //Order Request Inventories
                if (!context.OrderRequestInventories.Any())
                {

                    int k = 0;//Start with the first inventory
                    foreach (int i in orderIDs)
                    {
                        int howMany = random.Next(1, inventoryUPCs.Count());//add a few inventories to a order
                        for (int j = 1; j <= howMany; j++)
                        {
                            k = (k >= inventoryUPCs.Count()) ? 0 : k;//Resets counter k to 0 if we have run out of inventory
                            OrderRequestInventory o = new OrderRequestInventory()
                            {
                                OrderRequestID = i,
                                InventoryUPC = inventoryUPCs[k],
                                Quantity = random.Next(20, 51),
                                Price = random.NextDouble() * (priceMax - priceMin) + priceMin
                            };
                            context.OrderRequestInventories.Add(o);
                            k++;
                        }
                    }
                    context.SaveChanges();
                }
                int[] employeeIDs = context.Employees.Select(x => x.ID).ToArray();

                //Invoice 
                if (!context.Invoices.Any())
                {
                    for(int i=0; i<10; i++)
                    {
                        Invoice invoice = new Invoice()
                        {
                            Date = (DateTime.Today).AddDays(-random.Next(1, 360)),
                            Appreciation = random.NextDouble() * (100.00 - 10.00) + 10.00,
                            Description = "Sales Invoice",
                            CustomerID = customerIDs[random.Next(customerIDs.Count())],
                            EmployeeID = random.Next(3, 4)
                        };
                        context.Invoices.Add(invoice);
                    }
                    
                    context.SaveChanges();
                }                

                //Payment 
                if (!context.Payments.Any())
                {
                        context.Payments.AddRange(
                            new Payment
                            {
                                Type = "Cash",
                                Description = "Cash Payment"
                            }, new Payment
                            {
                                Type = "Debit",
                                Description = "Debit Payment"
                            }, new Payment
                            {
                                Type = "Credit",
                                Description = "Credit Payment"
                            }, new Payment
                            {
                                Type = "Cheque",
                                Description = "Cheque Payment"
                            });

                    context.SaveChanges();
                }

                int[] invoiceIDs = context.Invoices.Select(x => x.ID).ToArray();
                int[] paymentIDs = context.Payments.Select(x => x.ID).ToArray();

                //Invoice Payment 
                if (!context.InvoicePayments.Any())
                {
                    foreach(int i in invoiceIDs)
                    {
                        context.InvoicePayments.AddRange(
                            new InvoicePayment
                            {
                                InvoiceID = i,
                                PaymentID = paymentIDs[random.Next(paymentIDs.Count())]
                            });
                    }
                    context.SaveChanges();
                }

                //Invoice Line
                if (!context.InvoiceLines.Any())
                {

                    int k = 0;//Start with the first inventory
                    foreach (int i in invoiceIDs)
                    {
                        int howMany = random.Next(1, inventoryUPCs.Count());//add a few inventories to an invoice
                        for (int j = 1; j <= howMany; j++)
                        {
                            k = (k >= inventoryUPCs.Count()) ? 0 : k;//Resets counter k to 0 if we have run out of inventory
                            InvoiceLine invoiceLine = new InvoiceLine()
                            {
                                InvoiceID = i,
                                InventoryUPC = inventoryUPCs[k],
                                Quantity = random.Next(1, 3),
                                SalePrice = random.NextDouble() * (35.00 - 20.00) + 20.00
                            };
                            context.InvoiceLines.Add(invoiceLine);
                            k++;
                        }
                    }
                    context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }

        }
    }
}
