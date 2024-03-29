﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime Create_Date { get; set; }
        public int OrderStatusID { get; set; }
        public double FeeShip { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public string Pick_Province { get; set; }
        public string Pick_District { get; set; }
        public string Pick_Ward { get; set; }
        public string Pick_Address { get; set; }
        public string Pick_Tel { get; set; }
        public string Pick_Name { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string? Reason { get; set; }
        public string? Pick_Time { get; set; }
        public string? Deliver_time { get; set; }
        public string? PaymentMethod { get; set; }
        public double? RefundPrice { get; set; } 
        public string? PackingLink { get; set; }
    }
}
