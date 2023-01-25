using eSMP.VModels;

namespace eSMP.Services.OrderRepo
{
    public interface IOrderReposity
    {
        public Result UpdateOrderAddress(int orderID, int AddressID);
        public Result DeleteOrder(int orderID);
        public Result DeleteOrderDetail(int orderDetailID);
        public Result UpdateAmountOrderDetail(int orderDetailID, int amount);
        public Result GetAllOrder(int userID, int? orderStatusID);
        public Result GetOrderInfo(int orderID);
        public Result AddOrderDetail(OrderDetailAdd orderDetail);
        public int GetWeightOrder(int OrderID);
        public double GetPriceItemOrder(int OrderID);
        public List<OrderDetailModel> GetOrderDetailModels(int orderID, int orderStatusID);
        public Result FeedBaclOrderDetail(FeedBackOrderDetail feedBack);
        public Result GetOrdersWithShipstatus(int? userID, int? storeID, DateTime? dateFrom, DateTime? dateTo, int? shipOrderStatus, int? page, string? userName, int? orderID);
        public bool CancelOrder(int orderID);
        public Result GetlistFeedback(int? page, bool isFeedback, int? userID);
        public Result GetFeedbackDetail(int orderDetailID);
        public Result HiddenFeedback(int orderDetailID);
        public Result BlockFeedback(int orderDetailID);
        public Result CheckPay(int orderID);
        public int GetUserIDByOrderID(int orderID);
        public int GetSuppilerIDByOrderID(int orderID);
        public int GetUserIDByOrderDetailID(int orderDetailID);
        public int GetSuppilerIDByOrderDetailID(int orderDetailID);
    }
}
