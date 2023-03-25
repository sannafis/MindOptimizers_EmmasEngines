using EmmasEngines.Models;

namespace EmmasEngines.ViewModels
{
    public class ReportsVM
    {
        // Sales Reports view
        public List<Report> SavedSalesReports { get; set; }
        public List<Employee> Employees { get; set; }
        public NewSalesReport NewReport { get; set; }
        public SalesReportVM SalesReportVM { get; set; }

        // COGS Reports
        public List<COGSReport> SavedCOGSReports { get; set; }
        

        // Hourly Reports
        public List<HourlyReport> SavedHourlyReports { get; set; }
        public NewSalesReport NewSalesReport { get; internal set; }
    }
}
