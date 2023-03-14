using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class DataExchangeStoreModel
    {
        public int ExchangeStoreID { get; set; }
        public string ExchangeStoreName { get; set; }
        public double ExchangePrice { get; set; }
        public DateTime Create_date { get; set; }
        public int ExchangeStatusID { get; set; }
        public Image? Image { get; set; }
        public int? OrderID { get; set; }
    }
    public class DataExchangeStoreFinish
    {
        public int ExchangeStoreID { get; set; }
        public IFormFile File { get; set; }
    }
}
