using EmmasEngines.Models;

namespace EmmasEngines.ViewModels
{
    public class HourlyReportDetailsVM
    {
        public HourlyReport HourlyReport { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
