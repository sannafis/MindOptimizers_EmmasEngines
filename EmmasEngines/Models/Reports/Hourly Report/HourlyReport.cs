using EmmasEngines.Models.Reports;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class HourlyReport
    {
        [Key, ForeignKey("Report")]
        [Required(ErrorMessage = "Report ID is required.")]
        [Range(minimum: 1, maximum: Int32.MaxValue, ErrorMessage = "Report ID is required")]
        public int ID { get; set; }
        public Report Report { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}
