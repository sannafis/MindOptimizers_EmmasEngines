using System.ComponentModel.DataAnnotations;

namespace EmmasEngines.Models
{
    public class Position
    {
        [Required(ErrorMessage = "Position title is required")]
        [StringLength(50, ErrorMessage = "Title cannot be more than 50 characters long.")]
        public string Title { get; set; }

        public ICollection<EmployeePosition> EmployeePositions { get; set; } = new HashSet<EmployeePosition>();
    }
}
