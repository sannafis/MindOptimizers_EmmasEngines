﻿using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmmasEngines.Models
{
    public class Inventory
    {
        
        public int ID { get; set; }

        [Required(ErrorMessage = "UPC is required.")]
        [RegularExpression("^[0-9]{3}-[0-9]{4}-[0-9]{1}", ErrorMessage = "Please match the required format: ###-####-#")]
        [StringLength(10)]
        public string UPC { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Size is required. If there is no size, put 'N/A'.")]
        [StringLength(100, ErrorMessage = "Size cannot be more than 100 characters long.")]
        public string Size { get; set; } = "N/A";

        [Required(ErrorMessage = "Quantity is required. If there is no quantity, put 'N/A'.")]
        [StringLength(50, ErrorMessage = "Quantity cannot be more than 50 characters long.")]
        public string Quantity { get; set; } = "N/A";

        [Display(Name = "Cost (AVG)")]
        public double AdjustedPrice {
            get
            {
                if (OrderRequestInventories.Any())
                {
                    return Math.Round(Prices.Where(p=>p.Stock != 0).Select(x => x.PurchasedCost).Average(), 2);
                }
                else
                {
                    return 0.00;
                }
            }
        }

        [Display(Name = "Price (Retail)")]
        public double MarkupPrice {
            get
            {
                if (Prices.Any())
                {
                    
                    double average = Math.Round(Prices.Where(p => p.Stock != 0).Select(x => x.PurchasedCost).Average(), 2);
                    return Math.Round(((average * .23) + average),2);
                }
                else
                {
                    return 0.00;
                }
            }
        }

        public bool Current { get; set; } = true;

        //[Required(ErrorMessage = "Current status is required. Y/N")]
        //[StringLength(1, ErrorMessage = "Current must be one character long.")]
        //public string Current { get; set; }

        [Display(Name = "Stock")]
        public int TotalStock
        {
            get
            {
                return Prices.Select(x => x.Stock).Sum();
            }
        }

        //[Display(Name = "Supplier")]
        //[Required(ErrorMessage = "Please select a supplier.")]
        //public int SupplierID { get; set; }
        //public Supplier Supplier { get; set; }

        public ICollection<OrderRequestInventory> OrderRequestInventories { get; set; } = new HashSet<OrderRequestInventory>();
        
        public ICollection<Price> Prices { get; set; } = new HashSet<Price>();

        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new HashSet<InvoiceLine>();

        public ICollection<COGSReport> COGSReports { get; set; } = new HashSet<COGSReport>();

        public ICollection<SalesReportInventory> SalesReportInventories { get; set; } = new HashSet<SalesReportInventory>();

    }
}
