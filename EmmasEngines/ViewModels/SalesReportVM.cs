using EmmasEngines.Models;

namespace EmmasEngines.ViewModels
{
    public class SalesReportVM
    {
        public List<SalesReport> SavedReports { get; set; }
        public List<Employee> Employees { get; set; }
        public NewSalesReport NewReport { get; set; }
        public List<Report> SavedSalesReports { get; internal set; }
    }
    public class NewSalesReport
    {
        public string ReportName { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllEmployees { get; set; }
    }
}
