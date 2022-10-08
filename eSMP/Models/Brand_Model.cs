using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Brand_Model")]
    public class Brand_Model
    {
        [Key]
        public int Brand_ModelID { get; set; }
        public string Name { get; set; }
        public int BrandID { get; set; }//FK
        [ForeignKey("BrandID")]
        public Brand Brand { get; set; }
        public Boolean IsActive { get; set; }
    }
}
