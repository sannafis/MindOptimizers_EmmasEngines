using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class Invoice : IValidatableObject
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Appreciation is required")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Subtotal must be greater than or equal to 0.00.")]
        public double Appreciation { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be more than 100 characters long.")]
        public string Description { get; set; }

        public double Subtotal 
        {
            get
            {
                if (InvoiceLines.Any())
                {
                    double total = 0.0;
                    foreach (InvoiceLine line in InvoiceLines)
                    {
                        total += line.Quantity * line.SalePrice;
                    }
                    return total;
                }
                else
                {
                    return 0.00;
                }
            }
        }

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerID { get; set; }

        public Customer Customer { get; set; }

        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeID { get; set; }

        public Employee Employee { get; set; }

        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new HashSet<InvoicePayment>();

        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new HashSet<InvoiceLine>();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today)
            {
                yield return new ValidationResult("Date must be a valid date.", new[] { "Date" });
            }
        }

    }
}
