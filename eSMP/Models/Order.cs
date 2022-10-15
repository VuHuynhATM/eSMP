using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime Create_Date { get; set; }
        public Boolean IsPay { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public User User { get; set; }
        public int AddressID { get; set; }//FK 
        [ForeignKey("AddressID")]
        public Address Address { get; set; }
    }
}
