using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class OrderRequest : IValidatableObject
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be more than 100 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Order Sent")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SentDate { get; set; } = null;

        [Display(Name = "Order Received")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReceiveDate { get; set; } = null;

        [Display(Name = "Ext. Order Number")]
        [Required(ErrorMessage = "External Order Number is required")]
        [RegularExpression("^[0-9]{3,5}$", ErrorMessage = "This field must be 3-5 digits long.")]
        [StringLength(5)]
        public string ExternalOrderNum { get; set; }

        [Display(Name = "Customer")]
        public int CustomerID { get; set; }

        public Customer Customer { get; set; }

        public OrderRequestInventory OrderRequestInventory { get; set; }    

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiveDate > DateTime.Today)
            {
                yield return new ValidationResult("Receive Date must be a valid date.", new[] { "ReceiveDate" });
            }

            if (SentDate > DateTime.Today)
            {
                yield return new ValidationResult("Sent Date must be a valid date.", new[] { "SentDate" });
            }

            if (ReceiveDate < SentDate || (ReceiveDate!=null && SentDate==null))
            {
                yield return new ValidationResult("This order cannot be received before it is sent.", new[] { "ReceiveDate" });
            }

        }
    }
}
