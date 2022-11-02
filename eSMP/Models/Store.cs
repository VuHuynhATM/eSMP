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
        public virtual Address Address { get; set; }
        public int Store_StatusID { get; set; }//Fk
        [ForeignKey("Store_StatusID")]
        public virtual Store_Status Store_Status { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public virtual Image Image { get; set; }
        public double Asset { get; set; }
        public double? AmountActive { get; set; }
        public long? MomoTransactionID { get; set; }
        public DateTime? Actice_Date { get; set; }
    }
}
