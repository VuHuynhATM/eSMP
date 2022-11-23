namespace eSMP.VModels
{
    public class FirebaseNotification
    {
        public Notification notification { get; set; }
        public string to { get; set; }
    }
    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }
    }
    public class NotificationReponse
    {
        public string multicast_id { get; set; }
        public string success { get; set; }
        public string failure { get; set; }
        public string canonical_ids { get; set; }
        public List<result> results { get; set; }
    }
    public class result
    {
        public string message { get; set; }
    }
}
