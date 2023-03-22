using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmmasEngines.Models
{
    public enum ReportType
    {
        Sales,
        COGS,
        Hourly
    }

    public class Report
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Start")]
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime DateStart { get; set; }

        [Display(Name = "End")]
        [Required(ErrorMessage = "End Date is required")]
        public DateTime DateEnd { get; set; }

        [Required(ErrorMessage = "Criteria is required")]
        public string Criteria { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public ReportType Type { get; set; }

        [Display(Name = "Created On")]
        [Required(ErrorMessage = "Date Created is required")]
        public DateTime DateCreated { get; set; }

        public SalesReport SalesReport { get; set; }

        public HourlyReport HourlyReport { get; set; }

        public COGSReport COGSReport { get; set; }
    }
}
