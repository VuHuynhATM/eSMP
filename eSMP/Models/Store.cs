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
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Pick_date { get; set; }
        public int AddressID { get; set; }//FK
        [ForeignKey("AddressID")]
        public Address Address { get; set; }
        public int Store_StatusID { get; set; }//Fk
        [ForeignKey("Store_StatusID")]
        public Store_Status Store_Status { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public User User { get; set; }
        public int ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public Image Image { get; set; }
        public double Asset { get; set; }
    }
}
