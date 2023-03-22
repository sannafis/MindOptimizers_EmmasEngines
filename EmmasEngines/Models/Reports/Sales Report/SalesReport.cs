using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class SalesReport
    {
        [Key, ForeignKey("Report")]
        [Required(ErrorMessage = "Report ID is required.")]
        [Range(minimum: 1, maximum: Int32.MaxValue, ErrorMessage = "Report ID is required")]
        public int ID { get; set; }

        public Report Report { get; set; }

        [Display(Name = "Cash")]
        [Required(ErrorMessage = "Cash Amount is required")]
        public double CashAmount { get; set; } = 0.00;

        [Display(Name = "Debit")]
        [Required(ErrorMessage = "Debit Amount is required")]
        public double DebitAmount { get; set; } = 0.00;

        [Display(Name = "Credit")]
        [Required(ErrorMessage = "Credit Amount is required")]
        public double CreditAmount { get; set; } = 0.00;

        [Display(Name = "Cheque")]
        [Required(ErrorMessage = "Cheque Amount is required")]
        public double ChequeAmount { get; set; } = 0.00;

        [Required(ErrorMessage = "Total is required")]
        public double Total { get; set; }

        [Display(Name = "Sales Tax")]
        [Required(ErrorMessage = "Sales Tax is required")]
        public double SalesTax { get; set; }

        [Display(Name = "Other Tax")]
        [Required(ErrorMessage = "Other Tax is required")]
        public double OtherTax { get; set; }

        [Display(Name = "Total Tax")]
        [Required(ErrorMessage = "Total Tax is required")]
        public double TotalTax { get; set; }

        public ICollection<SalesReportEmployee> SalesReportEmployees { get; set; } = new HashSet<SalesReportEmployee>();
        public ICollection<SalesReportInventory> SalesReportInventories { get; set; } = new HashSet<SalesReportInventory>();
    }
}
