using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class SalesReportInventory
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Inventory UPC is required")]
        public string InventoryUPC { get; set; }
        public Inventory Inventory { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Total is required")]
        public double Total { get; set; }

        [Display(Name = "Report")]
        [Required(ErrorMessage = "Sales Report ID is required")]
        public int SalesReportID { get; set; }
        public SalesReport SalesReport { get; set; }

        public ICollection<SalesReportEmployee> SalesReportEmployees { get; set; } = new HashSet<SalesReportEmployee>();

        public ICollection<SalesReportInventory> SalesReportInventories { get; set; } = new HashSet<SalesReportInventory>();
    }
}
