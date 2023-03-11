using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class Supplier
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{3}-\\d{3}-\\d{4}", ErrorMessage = "Please match the required format: ###-###-####")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255, ErrorMessage = "Email cannot be more than 255 characters long.")]
        public string Email { get; set; }

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

        [Display(Name = "All Associated Orders")]
        public ICollection<OrderRequest> OrderRequests { get; set; } = new HashSet<OrderRequest>();

        [Display(Name = "All Products")]
        public ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();

    }
}
