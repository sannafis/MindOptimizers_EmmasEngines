using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class InvoiceLine
    {

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Subtotal must be greater than or equal to 0.")]
        public int Quantity { get; set; }

        [Display(Name = "Sale Price")]
        [Required(ErrorMessage = "Sale price is required")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Subtotal must be greater than or equal to 0.00.")]
        public double SalePrice { get; set; }

        [Display(Name = "Invoice")]
        [Required(ErrorMessage = "Invoice is required")]
        public int InvoiceID { get; set; }

        public Invoice Invoice { get; set; }

        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Inventory is required")]
        public string InventoryUPC { get; set; }

        public Inventory Inventory { get; set; }

    }
}
