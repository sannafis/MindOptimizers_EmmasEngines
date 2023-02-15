using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class EmployeePosition
    {

        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeID { get; set; }

        public Employee Employee { get; set; }

        [Display(Name = "Position")]
        [Required(ErrorMessage = "Position is required")]
        public string PositionTitle { get; set; }

        public Position Position { get; set; }
    }
}
