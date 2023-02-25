using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Specification")]
    public class Specification
    {
        [Key]
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public string? SpecificationSuggests { get; set; }
        public Boolean IsActive { get; set; }
    }
}
