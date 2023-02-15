using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class OrderRequestInventory
    {

        [Display(Name = "Order Request")]
        [Required(ErrorMessage = "Order Request is required")]
        public int OrderRequestID { get; set; }

        public OrderRequest OrderRequest { get; set; }

        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Inventory is required")]
        public string InventoryUPC { get; set; }

        public Inventory Inventory { get; set; }
    }
}
