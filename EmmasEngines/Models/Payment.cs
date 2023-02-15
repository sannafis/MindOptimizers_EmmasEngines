using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class Payment
    {
        public int ID { get; set; }

        [Display(Name = "Payment Type")]
        [Required(ErrorMessage = "Payment type is required")]
        [StringLength(50, ErrorMessage = "Payment type cannot be more than 50 characters long.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Payment Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be more than 100 characters long.")]
        public string Description { get; set; }

        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new HashSet<InvoicePayment>();

    }
}
