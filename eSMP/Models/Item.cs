﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Item")]
    public class Item
    {
        [Key]
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public DateTime Create_date { get; set; }
        public int StoreID { get; set; }//FK
        [ForeignKey("StoreID")]
        public virtual Store Store { get; set; }
        public int Sub_CategoryID { get; set; }
        [ForeignKey("Sub_CategoryID")]
        public virtual Sub_Category Sub_Category { get; set; }
        public int Item_StatusID { get; set; }
        public string? StatusText { get; set; }

    }
}
