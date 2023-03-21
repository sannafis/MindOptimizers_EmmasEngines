using EmmasEngines.Models.Reports;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class HourlyReport
    {
        [Key, ForeignKey("Report")]
        public int ID { get; set; }

        public Report Report { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}
