using System.ComponentModel.DataAnnotations;
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
        [StringLength(11)]
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
                if (Prices.Any())
                {
                    return Math.Round(Prices.Select(x => x.PurchasedCost).Average(), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "Price (Retail)")]
        public double MarkupPrice {
            get
            {
                if (Prices.Any())
                {
                    double average = Math.Round(Prices.Select(x => x.PurchasedCost).Average(), 2);
                    return Math.Round(((average * .23) + average));
                }
                else
                {
                    return 0;
                }
            }
        }

        [Required(ErrorMessage = "Current status is required.")]
        public bool Current { get; set; }

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

        [Display(Name = "All Prices")]
        public ICollection<Price> Prices { get; set; } = new HashSet<Price>();


    }
}
