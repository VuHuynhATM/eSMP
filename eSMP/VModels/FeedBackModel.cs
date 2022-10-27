using eSMP.Models;

namespace eSMP.VModels
{
    public class FeedBackModel
    {
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string Sub_itemName { get; set; }
        public List<Image>? ImagesFB { get; set; }
        public double? Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime Create_Date { get; set; }
    }
}
