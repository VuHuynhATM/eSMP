using eSMP.VModels;

namespace eSMP.Services.NotificationRepo
{
    public interface INotificationReposity
    {
        public Task<NotificationReponse> PushUserNotificationAsync(FirebaseNotification request);
        public Result GetListNotification(int userID, int? page);
        public void CreateNotifiaction(int userID, string title, int? storeID, int? orderID, int? ItemID);
    }
}
