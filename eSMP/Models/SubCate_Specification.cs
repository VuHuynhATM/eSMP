using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("SubCate_Specification")]
    public class SubCate_Specification
    {
        [Key]
        public int SubCate_SpecificationID { get; set; }
        public int Sub_CategoryID { get; set; }//Fk
        [ForeignKey("Sub_CategoryID")]
        public virtual Sub_Category Sub_Category { get; set; }
        public int SpecificationID { get; set; }//Fk
        [ForeignKey("SpecificationID")]
        public virtual Specification Specification { get; set; }
        public Boolean IsActive { get; set; }
    }
}
