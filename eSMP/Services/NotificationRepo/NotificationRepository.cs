using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Notification = eSMP.Models.Notification;

namespace eSMP.Services.NotificationRepo
{
    public class NotificationRepository:INotificationReposity
    {
        public static string TOKEN = "AAAAoDgJlCQ:APA91bEteTyEFMRfc_ly8aU1DAWEBdAsSU_QUWm2djkaqNDm7nEaDFUVxp5DeGOSkF3FKRhwjBGLvjGQnOobw4levIE-bovbeva2tHRMItW8TH-9tRzb9I754oxNaSBzHhVQGEOcN0uH";
        private readonly WebContext _context;
        public static int PAGE_SIZE { get; set; } = 25;
        public NotificationRepository(WebContext context)
        {
            _context = context;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }
        public void CreateNotifiaction(int userID, string title, int? storeID, int? orderID, int? ItemID)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == userID);
                if (user != null)
                {
                    Notification notification = new Notification();
                    notification.UserID = userID;
                    notification.Title = title; 
                    notification.StoreID = storeID;
                    notification.OrderID = orderID; 
                    notification.ItemID = ItemID;
                    notification.IsActive = true;
                    notification.Create_Date = GetVnTime();
                    _context.Notifications.Add(notification);
                    _context.SaveChanges();
                }
            }
            catch
            {
                return;
            }
        }

        public Result GetListNotification(int userID, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listNotification=_context.Notifications.Where(n=>n.UserID==userID);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listNotification.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listNotification = listNotification.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                List<NotificationModel> list = new List<NotificationModel>();
                foreach (var notification in listNotification.ToList())
                {
                    NotificationModel notificationModel = new NotificationModel
                    {
                        UserID = notification.UserID,
                        Title = notification.Title,
                        Create_Date=notification.Create_Date,
                        IsActive = notification.IsActive,
                        ItemID = notification.ItemID,
                        NotificationID = notification.NotificationID,
                        OrderID = notification.OrderID,
                        StoreID = notification.StoreID
                    };
                    list.Add(notificationModel);
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = list;
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public async Task<NotificationReponse> PushUserNotificationAsync(FirebaseNotification request)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " +TOKEN);
                StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                var ghtkreponde = await client.PostAsync("https://fcm.googleapis.com/fcm/send", httpContent);
                var contents = ghtkreponde.Content.ReadAsStringAsync().Result;
                return null;
            }
            catch
            {
                return null;
            }
            
        }

    }
}
