using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("OrderStore_Transaction")]
    public class OrderStore_Transaction
    {
        [Key]
        public int OrderStore_TransactionID { get; set; }
        public int StoreID { get; set; }
        [ForeignKey("StoreID")]
        public virtual Store Store { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
}
