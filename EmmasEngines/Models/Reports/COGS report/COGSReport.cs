using EmmasEngines.Models.Reports;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class COGSReport
    {
        [Key, ForeignKey("Report")]
        [Required(ErrorMessage = "Report ID is required.")]
        [Range(minimum: 1, maximum: Int32.MaxValue, ErrorMessage = "Report ID is required")]
        public int ID { get; set; }

        public Report Report { get; set; }

        [Display(Name = "Material (Start)")]
        [Required(ErrorMessage = "Start Cost is required")]
        public double StartCost { get; set; }

        [Display(Name = "Materials (Purchased)")]
        [Required(ErrorMessage = "Purchased Cost is required")]
        public double PurchasedCost { get; set; }

        [Display(Name = "Materals (End)")]
        [Required(ErrorMessage = "End Cost is required")]
        public double EndCost { get; set; }

        [Display(Name = "Cost of Goods Sold")]
        [Required(ErrorMessage = "Cost of Goods Sold is required")]
        public double COGS { get; set; }

        [Display(Name = "Sales Revenue")]
        [Required(ErrorMessage = "Sale Revenue is required")]
        public double SaleRevenue { get; set; }

        [Display(Name = "Gross Profit")]
        [Required(ErrorMessage = "Gross Profit is required")]
        public double GrossProfit { get; set; }

        [Display(Name = "Profit Margin %")]
        [Required(ErrorMessage = "Profit Margin is required")]
        public double ProfitMargin { get; set; }

        public ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();

        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
    }
}
