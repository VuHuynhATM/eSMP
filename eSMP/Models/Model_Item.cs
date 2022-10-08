using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Model_Item")]
    public class Model_Item
    {
        [Key]
        public int Model_ItemID { get; set; }
        public int Brand_ModelID { get; set; }//FK
        [ForeignKey("Brand_ModelID")]
        public Brand_Model Brand_Model { get; set; }
        public int ItemID { get; set; }//Fk
        [ForeignKey("ItemID")]
        public Item Item { get; set; }
        public Boolean IsActive { get; set; }
    }
}
