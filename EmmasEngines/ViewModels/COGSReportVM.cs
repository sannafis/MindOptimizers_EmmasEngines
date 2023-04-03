using EmmasEngines.Models;
using EmmasEngines.Utilities;

namespace EmmasEngines.ViewModels
{
    public class COGSReportVM
    {
        public List<COGSReport> SavedReports { get; set; }
        public List<Inventory> Inventories { get; set; }
        public List<Invoice> Invoices { get; set; }
        public NewCOGSReport NewReport { get; set; }
        public PaginatedList<Report> SavedCOGSReports { get; internal set; }
    }

    public class NewCOGSReport
    {
        public string ReportName { get; set; }
        public int? InventoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllInventory { get; set; }
    }
}
