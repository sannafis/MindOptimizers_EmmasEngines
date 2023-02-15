using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class InvoicePayment
    {

        [Display(Name = "Invoice")]
        [Required(ErrorMessage = "Invoice is required")]
        public int InvoiceID { get; set; }

        public Invoice Invoice { get; set; }

        [Display(Name = "Payment")]
        [Required(ErrorMessage = "Payment is required")]
        public int PaymentID { get; set; }

        public Payment Payment { get; set; }
    }
}
