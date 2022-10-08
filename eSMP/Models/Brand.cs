using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Brand")]
    public class Brand
    {
        [Key]
        public int BrandID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}
