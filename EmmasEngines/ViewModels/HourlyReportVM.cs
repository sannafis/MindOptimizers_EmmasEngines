using EmmasEngines.Models;
using EmmasEngines.Utilities;

namespace EmmasEngines.ViewModels
{
    public class HourlyReportVM
    {
        public List<HourlyReport> SavedReports { get; set; }
        public List<Employee> Employees { get; set; }
        public NewHourlyReport NewReport { get; set; }
        public PaginatedList<Report> SavedHourlyReports { get; internal set; }
    }
    public class NewHourlyReport
    {
        public string ReportName { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllEmployees { get; set; }
    }
}
