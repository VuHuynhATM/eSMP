using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ReportStatus")]
    public class ReportStatus
    {
        [Key]
        public int ReportStatusID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}
