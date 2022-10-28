using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Feedback_Image")]
    public class Feedback_Image
    {
        [Key]
        public int Feedback_ImageID { get; set; }
        public int OrderDetailID { get; set; }//FK
        [ForeignKey("OrderDetailID")]
        public virtual OrderDetail OrderDetail { get; set; }
        public int ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public virtual Image Image { get; set; }
        public Boolean IsActive { get; set; }
    }
}
