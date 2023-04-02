using EmmasEngines.Models;

namespace EmmasEngines.ViewModels
{
    public class SalesReportDetailsVM
    {
        public SalesReport SalesReport { get; set; }
        public IEnumerable<SalesReportEmployee> SalesReportEmployees { get; set; }
        public IEnumerable<SalesReportInventory> SalesReportInventories { get; set; }
        public double AppreciationEarned { get; set; }
        public double AppreciationEarnedToDate { get; set; }
    }
}
