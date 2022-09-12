using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Store")]
    public class Store
    {
        [Key]
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public DateTime Create_date { get; set; }
        public string Emaai { get; set; }
        public string Phone { get; set; }
        public Boolean IsActive { get; set; }
        [Required]
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public User User { get; set; }
        [Required]
        public int AddressID { get; set; }//FK
        [ForeignKey("AddressID")]
        public Address Address { get; set; }
    }
}
