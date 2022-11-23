using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class NotificationModel
    {
        public int NotificationID { get; set; }
        public DateTime Create_Date { get; set; }
        public string Title { get; set; }
        public int? OrderID { get; set; }
        public int? StoreID { get; set; }
        public int? ItemID { get; set; }
        public int UserID { get; set; }
        public Boolean IsActive { get; set; }
    }
}
