using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public class Employee
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Employee")]
        public string FullName 
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be more than 50 characters long.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passoword is required")]
        [StringLength(50, ErrorMessage = "Password cannot be more than 50 characters long.")]
        public string Password { get; set; }

        public ICollection<EmployeePosition> EmployeePositions { get; set; } = new HashSet<EmployeePosition>();

        public ICollection<SalesReportEmployee> SalesReportEmployees { get; set; } = new HashSet<SalesReportEmployee>();

        public ICollection<HourlyReport> HourlyReports { get; set; } = new HashSet<HourlyReport>();

    }
}
