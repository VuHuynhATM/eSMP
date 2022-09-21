using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Store_Address")]
    public class Store_Address
    {
        [Key]
        public int Store_AddressID { get; set; }
        public int StoreID { get; set; }//FK
        [ForeignKey("StoreID")]
        public Store Store { get; set; }
        public int AddressID { get; set; }//FK
        [ForeignKey("AddressID")]
        public Address Address { get; set; }
    }
}
