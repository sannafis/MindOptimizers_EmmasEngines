using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class City
    {
        public int ID { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City name is required.")]
        [StringLength(100, ErrorMessage = "City name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Province")]
        [Required(ErrorMessage = "Please select a Province.")]
        public string ProvinceCode { get; set; }

        public Province Province { get; set; }
    }
}
