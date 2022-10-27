using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Store_Withdrawal")]
    public class Store_Withdrawal
    {
        [Key]
        public int Store_WithdrawalID { get; set; }
        public int StoreID { get; set; }//fk
        [ForeignKey("StoreID")]
        public Store Store { get; set; }
        public int? ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public Image? Image { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public string Reason { get; set; }
        public double Price { get; set; }
        public int Withdrawal_StatusID { get; set; }//Fk
        [ForeignKey("Withdrawal_StatusID")]
        public Withdrawal_Status Withdrawal_Status { get; set; }
    }
}
