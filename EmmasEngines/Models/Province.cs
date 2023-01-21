using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class Province
    {
        public int ID { get; set; }

        [Display(Name = "Province")]
        [Required(ErrorMessage = "Province name is required.")]
        [StringLength(100, ErrorMessage = "Province name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Province")]
        [Required(ErrorMessage = "Province code is required.")]
        [StringLength(2, ErrorMessage = "Province code cannot be more than 2 characters long.")]
        public string Code { get; set; }

        [Display(Name = "Cities")]
        public ICollection<City> Cities { get; set; } = new HashSet<City>();
    }
}
