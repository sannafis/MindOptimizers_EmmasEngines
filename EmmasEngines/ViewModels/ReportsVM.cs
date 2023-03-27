using EmmasEngines.Models;
using EmmasEngines.Utilities;

namespace EmmasEngines.ViewModels
{
    public class ReportsVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        

        // Sales Reports view
        public PaginatedList<SalesReport> SavedSalesReports { get; set; }
        public List<Employee> Employees { get; set; }
        public NewSalesReport NewReport { get; set; }
        public SalesReportVM SalesReportVM { get; set; }

        // COGS Reports
        public List<COGSReport> SavedCOGSReports { get; set; }
        

        // Hourly Reports
        public List<HourlyReport> SavedHourlyReports { get; set; }
        public NewHourlyReport NewHourlyReport { get; set; }
        public HourlyReportVM HourlyReportVM { get; set; }

        public NewSalesReport NewSalesReport { get; internal set; }
    }
}
