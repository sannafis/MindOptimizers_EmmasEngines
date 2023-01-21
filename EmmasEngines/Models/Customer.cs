using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot be more than 100 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Customer Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{3}-\\d{3}-\\d{4}-\\d$", ErrorMessage = "Please match the required format: ###-###-####")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot be more than 200 characters long.")]
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal Code is required.")]
        [RegularExpression("^[A-Z]\\d[A-Z] \\d[A-Z]\\d$", ErrorMessage = "Please match the required format: A#B #C#")]
        [StringLength(7, ErrorMessage = "Postal Code cannot be more than 7 characters long.")]
        public string Postal { get; set; }

        public Province Province { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Please select a city.")]
        public int CityID { get; set; }

        public City City { get; set; }
    }
}
