using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Sub_Item")]
    public class Sub_Item
    {
        [Key]
        public int Sub_ItemID { get; set; }
        public string Sub_ItemName { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public int ItemID { get; set; }//Fk
        [ForeignKey("ItemID")]
        public Item Item { get; set; }
        public Boolean IsActive { get; set; }
    }
}
