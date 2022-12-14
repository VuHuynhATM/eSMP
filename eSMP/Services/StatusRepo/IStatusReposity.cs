using eSMP.Models;

namespace eSMP.Services.StatusRepo
{
    public interface IStatusReposity
    {
        public Status GetItemStatus(int statusID);
        public Status GetSubItemStatus(int statusID);
        public Status GetfeedbackStatus(int statusID);
        public Status GetStoreStatus(int statusID);
        public Status GetWithdrawalStatus(int statusID);
        public Status GetOrderStatus(int statusID);
        public Status GetReportStatus(int statusID);
    }
}
