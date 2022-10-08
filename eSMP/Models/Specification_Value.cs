using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Specification_Value")]
    public class Specification_Value
    {
        [Key]
        public int Specification_ValueID { get; set; }
        public string Value { get; set; }
        public int SpecificationID { get; set; }//FK
        [ForeignKey("SpecificationID")]
        public Specification Specification { get; set; }
        public int ItemID { get; set; }//FK
        [ForeignKey("ItemID")]
        public Item Item { get; set; }
        public Boolean IsActive { get; set; }
    }
}
