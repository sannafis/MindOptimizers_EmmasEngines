using EmmasEngines.Models;

namespace EmmasEngines.ViewModels
{
    public class COGSReportDetailsVM
    {
        public COGSReport COGSReport { get; set; }
        public IEnumerable<Inventory> Inventories { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}
