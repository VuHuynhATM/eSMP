﻿using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class SystemWithdrawalM
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class SystemWithdrawalView
    {
        public int System_WithdrawalID { get; set; }
        public Image Image { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class OrderSystemTransactionView
    {
        public int OrderSystem_TransactionID { get; set; }
        public int OrderStore_TransactionID { get; set; }
        public int orderID { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }

}
