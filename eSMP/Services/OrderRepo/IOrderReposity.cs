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
    }
}
