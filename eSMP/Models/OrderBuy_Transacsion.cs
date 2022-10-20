﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("OrderBuy_Transacsion")]
    public class OrderBuy_Transacsion
    {
        [Key]
        public int ID { get; set; }
        public DateTime Create_Date { get; set; }
        public int ResultCode { get; set; }
        public long MomoTransactionID { get; set; }
        public int OrderID { get; set; }//FK
        [ForeignKey("OrderID")]
        public Order Order { get; set; }    
    }
}