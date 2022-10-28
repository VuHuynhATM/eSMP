using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Item_Image")]
    public class Item_Image
    {
        [Key]
        public int Item_ImageID { get; set; }
        public int ItemID { get; set; }//FK
        [ForeignKey("ItemID")]
        public virtual Item Item { get; set; }
        public int ImageID { get; set; }//Fk
        [ForeignKey("ImageID")]
        public virtual Image Image { get; set; }
    }
}
