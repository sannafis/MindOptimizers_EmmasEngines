using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class SalesReportEmployee
    {
        public int ID { get; set; }

        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Required(ErrorMessage = "Sales Amount is required")]
        public double Sales { get; set; }

        [Display(Name = "Report")]
        [Required(ErrorMessage = "Sales Report ID is required")]
        public int SalesReportID { get; set; }
        public SalesReport SalesReport { get; set; }


    }
}
