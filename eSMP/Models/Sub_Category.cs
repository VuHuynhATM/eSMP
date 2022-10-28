using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Sub_Category")]
    public class Sub_Category
    {
        [Key]
        public int Sub_CategoryID { get; set; }
        public string Sub_categoryName { get; set; }
        public Boolean IsActive { get; set; }
        public int CategoryID { get; set; }//FK
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
