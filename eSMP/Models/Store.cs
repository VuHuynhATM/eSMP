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
        //public string Phone { get; set; }

        public int Store_StatusID { get; set; }//Fk
        [ForeignKey("Store_StatusID")]
        public Store_Status Store_Status { get; set; }

        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}
