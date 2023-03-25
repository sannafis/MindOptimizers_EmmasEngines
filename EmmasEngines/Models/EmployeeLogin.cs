using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class EmployeeLogin
    {
        public int ID { get; set;  }

        public DateTime SignIn { get; set; }

        public DateTime SignOut { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
    }
}
