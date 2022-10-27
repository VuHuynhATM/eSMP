using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("OrderSystem_Transaction")]
    public class OrderSystem_Transaction
    {
        [Key]
        public int OrderSystem_TransactionID { get; set; }
        public int OrderStore_TransactionID { get; set; }//Fk
        [ForeignKey("OrderStore_TransactionID")]
        public OrderStore_Transaction OrderStore_Transaction { get; set; }
        public int SystemID { get; set; }//fk
        [ForeignKey("SystemID")]
        public eSMP_System eSMP_System { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
}
