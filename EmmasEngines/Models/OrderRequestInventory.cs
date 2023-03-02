using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class OrderRequestInventory
    {

        [Display(Name = "Order Request")]
        [Required(ErrorMessage = "Order Request ID is required")]
        public int OrderRequestID { get; set; }
        public OrderRequest OrderRequest { get; set; }

        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Inventory item is required")]
        public string InventoryUPC { get; set; }
        public Inventory Inventory { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Quantity cannot be less than 0")]
        public int Quantity { get; set; }
    }
}
