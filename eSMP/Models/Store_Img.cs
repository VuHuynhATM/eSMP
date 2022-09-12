using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Store_img")]
    public class Store_Img
    {
        [Key]
        public int ID { get; set; }
        public Boolean IsActive { get; set; }
        public int StoreID { get; set; }//FK
        [ForeignKey("StoreID")]
        public Store Store { get; set; }
        public int ImageID { get; set; }//Fk
        [ForeignKey("ImageID")]
        public Image Image { get; set; }
    }
}
