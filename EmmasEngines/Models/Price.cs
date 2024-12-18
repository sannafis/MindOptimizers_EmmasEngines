﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmasEngines.Models
{
    public class Price
    {
        public int ID { get; set; }

        //Inventory foreign key
       [Display(Name = "Inventory UPC")]
       [Required(ErrorMessage = "Inventory associated with this price is required.")]
        public string InventoryUPC { get; set; }

        [Display(Name = "Inventory")]
        public Inventory Inventory { get; set; }

        [Display(Name = "Purchase Price")]
        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.00.")]
        public double PurchasedCost { get; set; }

        [Display(Name = "Purchase Date")]
        [Required(ErrorMessage = "Purchase date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PurchasedDate { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
        public int Stock { get; set; }

        //[Display(Name = "Supplier")]
        //[Required(ErrorMessage = "Please select a supplier.")]
        //public int SupplierID { get; set; }

        //public Supplier Supplier { get; set; }

    }
}
