using eSMP.Models;
using eSMP.Services.FileRepo;
using eSMP.Services.ShipRepo;
using eSMP.Services.StatusRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace eSMP.Services.OrderRepo
{
    public class OrderRepository : IOrderReposity
    {
        private readonly WebContext _context;
        private readonly Lazy<IShipReposity> _shipReposity;
        private readonly Lazy<IStoreReposity> _storeReposity;
        private readonly Lazy<IFileReposity> _fileReposity;
        private readonly Lazy<IStatusReposity> _statusReposity;

        public static int PAGE_SIZE { get; set; } = 6;

        public OrderRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IStoreReposity> storeReposity, Lazy<IFileReposity> fileReposity, Lazy<IStatusReposity> statusReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _storeReposity = storeReposity;
            _fileReposity = fileReposity;
            _statusReposity = statusReposity;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }

        public bool CheckAmount(int subItemID, int amount)
        {
            try
            {
                var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subItemID && si.Amount >= amount);
                if (subItem != null)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public Result AddOrderDetail(OrderDetailAdd orderDetail)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = GetItem(orderDetail.Sub_ItemID);
                if (CheckSubItemInOrder(orderDetail.Sub_ItemID, orderDetail.UserID))
                {
                    var odexist = _context.OrderDetails.SingleOrDefault(od => od.Sub_ItemID == orderDetail.Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID && o.UserID == orderDetail.UserID).OrderStatusID == 2);
                    if (CheckAmount(orderDetail.Sub_ItemID, orderDetail.Amount + odexist.Amount))
                    {
                        odexist.Amount = odexist.Amount + orderDetail.Amount;
                        odexist.DiscountPurchase = GetDiscount(orderDetail.Sub_ItemID);
                        odexist.PricePurchase = GetSub_Item(orderDetail.Sub_ItemID).Price;
                        //ship
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == odexist.OrderID);
                        int weight = GetWeightOfSubItem(item.ItemID) * orderDetail.Amount;
                        int weightOrder = GetWeightOrder(order.OrderID);
                        var ship = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, weight + weightOrder);
                        if (ship.success)
                        {
                            order.FeeShip = ship.fee.fee;
                        }
                        else
                        {
                            order.FeeShip = 0;
                        }
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Thêm vào giỏ hàng thành công";
                        result.Data = order.OrderID;
                        result.TotalPage = numpage;
                        return result;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Số lượng sản phẩm không đủ";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }

                }
                else
                {
                    int storeID = GetStoreBySubItemID(orderDetail.Sub_ItemID).StoreID;
                    var order = _context.Orders.SingleOrDefault(o => o.OrderStatusID == 2 && o.UserID == orderDetail.UserID && _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID).StoreID == storeID) != null) != null);
                    if (order != null)
                    {
                        int weight = GetWeightOfSubItem(item.ItemID) * orderDetail.Amount;
                        int weightOrder = GetWeightOrder(order.OrderID);
                        var ship = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, weight + weightOrder);
                        if (ship.success)
                        {
                            order.FeeShip = ship.fee.fee;
                        }
                        else
                        {
                            order.FeeShip = 0;
                        }
                        if (CheckAmount(orderDetail.Sub_ItemID, orderDetail.Amount))
                        {
                            OrderDetail newod = new OrderDetail();
                            newod.OrderID = order.OrderID;
                            newod.PricePurchase = GetSub_Item(orderDetail.Sub_ItemID).Price;
                            newod.Amount = orderDetail.Amount;
                            newod.DiscountPurchase = GetDiscount(orderDetail.Sub_ItemID);
                            newod.Sub_ItemID = orderDetail.Sub_ItemID;

                            _context.OrderDetails.Add(newod);
                            _context.SaveChanges();
                            result.Success = true;
                            result.Message = "Thêm vào giỏ hàng thành công";
                            result.Data = order.OrderID;
                            result.TotalPage = numpage;
                            return result;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Số lượng sản phẩm không đủ";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                    else
                    {
                        Address storeAddress = GetStoreAddress(orderDetail.Sub_ItemID);
                        Address userAddress = GetAddressDefault(orderDetail.UserID);
                        Order o = new Order();
                        o.Pick_Name = storeAddress.UserName;
                        o.Pick_Province = storeAddress.Province;
                        o.Pick_District = storeAddress.District;
                        o.Pick_Ward = storeAddress.Ward;
                        o.Pick_Address = storeAddress.Context;
                        o.Pick_Tel = storeAddress.Phone;

                        o.Name = userAddress.UserName;
                        o.Province = userAddress.Province;
                        o.District = userAddress.District;
                        o.Ward = userAddress.Ward;
                        o.Address = userAddress.Context;
                        o.Tel = userAddress.Phone;

                        o.Create_Date = GetVnTime();
                        o.OrderStatusID = 2;
                        o.UserID = orderDetail.UserID;
                        //ship
                        int weight = GetWeightOfSubItem(item.ItemID) * orderDetail.Amount;
                        var ship = _shipReposity.Value.GetFeeAsync(o.Province, o.District, o.Pick_Province, o.Pick_District, weight);
                        if (ship.success)
                        {
                            o.FeeShip = ship.fee.fee;
                        }
                        else
                        {
                            o.FeeShip = 0;
                        }
                        if (CheckAmount(orderDetail.Sub_ItemID, orderDetail.Amount))
                        {
                            OrderDetail od = new OrderDetail();
                            od.Order = o;
                            od.PricePurchase = GetSub_Item(orderDetail.Sub_ItemID).Price;
                            od.Amount = orderDetail.Amount;
                            od.DiscountPurchase = GetDiscount(orderDetail.Sub_ItemID);
                            od.Sub_ItemID = orderDetail.Sub_ItemID;

                            _context.OrderDetails.Add(od);
                            _context.SaveChanges();
                            result.Success = true;
                            result.Message = "Thêm vào giỏ hàng thành công";
                            result.Data = o.OrderID;
                            result.TotalPage = numpage;
                            return result;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Số lượng sản phẩm không đủ";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Thêm vào giỏ hàng thất bại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public int GetWeightOrder(int OrderID)
        {
            var orderDetail = _context.OrderDetails.Where(x => x.OrderID == OrderID).ToList();
            int weight = 0;
            foreach (var detail in orderDetail)
            {
                var item = GetItem(detail.Sub_ItemID);
                weight = weight + GetWeightOfSubItem(item.ItemID) * detail.Amount;
            }
            return weight;
        }
        public Address GetStoreAddress(int subItemID)
        {
            var storeaddressID = GetStoreBySubItemID(subItemID).AddressID;
            return _context.Addresss.SingleOrDefault(s => s.AddressID == storeaddressID);
        }
        public int GetWeightOfSubItem(int itemID)
        {
            var weight = _context.Specification_Values.SingleOrDefault(sv => sv.ItemID == itemID && sv.SpecificationID == 2).Value;
            return int.Parse(weight);
        }

        public Store GetStoreBySubItemID(int sub_itemID)
        {
            try
            {
                return _context.Stores.SingleOrDefault(s => s.StoreID == _context.Items.SingleOrDefault(i => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sub_itemID).ItemID == i.ItemID).StoreID);
            }
            catch
            {
                return null;
            }
        }
        public double GetDiscount(int sub_ItemID)
        {
            try
            {
                return _context.Items.SingleOrDefault(i => i.ItemID == _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sub_ItemID).ItemID).Discount;
            }
            catch
            {
                return -1;
            }
        }
        public Sub_Item GetSub_Item(int sub_ItemID)
        {
            try
            {
                return _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sub_ItemID);
            }
            catch
            {
                return null;
            }
        }
        public Address GetAddressDefault(int userID)
        {
            try
            {
                return _context.Addresss.FirstOrDefault(a => a.AddressID == _context.User_Addresses.FirstOrDefault(ud => ud.UserID == userID).AddressID);
            }
            catch
            {
                return null;
            }
        }
        public bool CheckSubItemInOrder(int sub_ItemID, int userID)
        {
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.Sub_ItemID == sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID && o.UserID == userID).OrderStatusID == 2);
                if (orderDetail == null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Result DeleteOrder(int orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listOrderDetail = _context.OrderDetails.Where(od => od.OrderID == orderID && _context.Orders.SingleOrDefault(o => o.OrderID == orderID).OrderStatusID == 2).ToList();
                if (listOrderDetail.Count() > 0)
                {
                    foreach (var item in listOrderDetail)
                    {
                        _context.OrderDetails.Remove(item);
                    }
                }
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 2);
                if (order == null)
                {
                    result.Success = false;
                    result.Message = "Xoá không thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Xoá thành công";
                result.Data = order;
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
        public Result DeleteOrderDetail(int orderDetailID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID && _context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID).OrderStatusID == 2);
                if (orderDetail == null)
                {
                    result.Success = false;
                    result.Message = "Xoá không thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                _context.OrderDetails.Remove(orderDetail);
                _context.SaveChanges();
                return GetOrderInfo(orderDetail.OrderID);
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
        public Address GetAddressByID(int addressID)
        {
            return _context.Addresss.SingleOrDefault(a => a.AddressID == addressID);
        }

        public Item GetItem(int subItemID)
        {
            return _context.Items.SingleOrDefault(i => i.ItemID == _context.Sub_Items.FirstOrDefault(si => si.Sub_ItemID == subItemID).ItemID);
        }

        public StoreViewModel GetStoreViewModel(int orderID)
        {
            try
            {
                var orderdetail = _context.OrderDetails.FirstOrDefault(od => od.OrderID == orderID);
                var store = GetStoreBySubItemID(orderdetail.Sub_ItemID);
                StoreViewModel model = _storeReposity.Value.GetStoreModel(store.StoreID);
                return model;
            }
            catch
            {
                return null;
            }
        }

        public List<OrderDetailModel> GetOrderDetailModels(int orderID, int orderStatusID)
        {
            try
            {
                List<OrderDetailModel> list = new List<OrderDetailModel>();
                var listOrderDetails = _context.OrderDetails.Where(od => od.OrderID == _context.Orders.SingleOrDefault(o => o.OrderStatusID == orderStatusID && o.OrderID == orderID).OrderID);
                if (listOrderDetails.Count() > 0)
                {
                    foreach (var orderDetail in listOrderDetails.ToList())
                    {
                        var subitem = GetSub_Item(orderDetail.Sub_ItemID);
                        if (orderStatusID != 2)
                        {
                            if (orderDetail.Feedback_StatusID != null)
                            {
                                OrderDetailModel model = new OrderDetailModel
                                {
                                    Amount = orderDetail.Amount,
                                    DiscountPurchase = orderDetail.DiscountPurchase,
                                    OrderDetailID = orderDetail.OrderDetailID,
                                    PricePurchase = orderDetail.PricePurchase,
                                    FeedBack_Date = orderDetail.FeedBack_Date,
                                    Feedback_Rate = orderDetail.Feedback_Rate,
                                    Feedback_Title = orderDetail.Feedback_Title,
                                    Feedback_Status = _statusReposity.Value.GetfeedbackStatus(orderDetail.Feedback_StatusID.Value),
                                    Sub_ItemID = subitem.Sub_ItemID,
                                    Sub_ItemName = subitem.Sub_ItemName,
                                    sub_ItemImage = subitem.Image.Path,
                                    ItemID = subitem.ItemID,
                                    ListImageFb = GetListImageFB(orderDetail.OrderDetailID),
                                };
                                list.Add(model);
                            }
                            else
                            {
                                OrderDetailModel model = new OrderDetailModel
                                {
                                    Amount = orderDetail.Amount,
                                    DiscountPurchase = orderDetail.DiscountPurchase,
                                    OrderDetailID = orderDetail.OrderDetailID,
                                    PricePurchase = orderDetail.PricePurchase,
                                    FeedBack_Date = orderDetail.FeedBack_Date,
                                    Feedback_Rate = orderDetail.Feedback_Rate,
                                    Feedback_Title = orderDetail.Feedback_Title,
                                    Feedback_Status = null,
                                    Sub_ItemID = subitem.Sub_ItemID,
                                    Sub_ItemName = subitem.Sub_ItemName,
                                    sub_ItemImage = subitem.Image.Path,
                                    ItemID = subitem.ItemID,
                                    ListImageFb = GetListImageFB(orderDetail.OrderDetailID),
                                };
                                list.Add(model);
                            }
                            
                            
                        }
                        else
                        {
                            OrderDetailModel model = new OrderDetailModel
                            {
                                Amount = orderDetail.Amount,
                                DiscountPurchase = GetItem(orderDetail.Sub_ItemID).Discount,
                                OrderDetailID = orderDetail.OrderDetailID,
                                PricePurchase = subitem.Price,
                                Sub_ItemID = subitem.Sub_ItemID,
                                Sub_ItemName = subitem.Sub_ItemName,
                                sub_ItemImage = subitem.Image.Path,
                                ItemID = subitem.ItemID,
                            };
                            list.Add(model);
                        }

                    }
                }
                return list;
            }
            catch
            {
                return new List<OrderDetailModel>();
            }
        }

        public Result GetAllOrder(int userID, int? orderStatusID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                List<OrderModel> list = new List<OrderModel>();
                var listOrder = _context.Orders.Where(o => o.UserID == userID).AsQueryable();
                if (orderStatusID.HasValue)
                {
                    listOrder = listOrder.Where(o => o.OrderStatusID == orderStatusID);
                }
                listOrder = listOrder.OrderByDescending(o => o.Create_Date);
                if (listOrder.Count() > 0)
                {
                    foreach (var order in listOrder.ToList())
                    {
                        OrderModel model = new OrderModel
                        {
                            OrderID = order.OrderID,
                            StoreView = GetStoreViewModel(order.OrderID),
                            Create_Date = order.Create_Date,
                            UserID = order.UserID,
                            PriceItem = GetPriceItemOrder(order.OrderID),
                            OrderStatus = _statusReposity.Value.GetOrderStatus(order.OrderStatusID),
                            Pick_Address = order.Pick_Address,
                            Pick_Province = order.Pick_Province,
                            Pick_District = order.Pick_District,
                            Pick_Ward = order.Pick_Ward,
                            Pick_Name = order.Pick_Name,
                            Pick_Tel = order.Pick_Tel,
                            Address = order.Address,
                            District = order.District,
                            Province = order.Province,
                            Ward = order.Ward,
                            Name = order.Name,
                            Tel = order.Tel,
                            Details = GetOrderDetailModels(order.OrderID, order.OrderStatusID),
                            FeeShip = order.FeeShip,
                            Reason = order.Reason,
                            Pick_Time= order.Pick_Time,
                        };
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa có sản phảm trong giỏ hàng";
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

        public Result GetOrderInfo(int orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    if (order.OrderStatusID == 2)
                    {
                        var ship = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, GetWeightOrder(order.OrderID));
                        if (ship.success)
                            order.FeeShip = ship.fee.fee;
                        else
                            order.FeeShip = 0;
                        order.FeeShip = order.FeeShip;
                        _context.SaveChanges();
                    }
                    var shiporder = GetShipOrder(order.OrderID);
                    if (shiporder != null)
                    {
                        OrderModel model = new OrderModel
                        {
                            OrderID = order.OrderID,
                            StoreView = GetStoreViewModel(order.OrderID),
                            Create_Date = order.Create_Date,
                            UserID = order.UserID,
                            OrderStatus = _statusReposity.Value.GetOrderStatus(order.OrderStatusID),
                            PriceItem = GetPriceItemOrder(orderID),
                            Pick_Address = order.Pick_Address,
                            Pick_Province = order.Pick_Province,
                            Pick_District = order.Pick_District,
                            Pick_Ward = order.Pick_Ward,
                            Pick_Name = order.Pick_Name,
                            Pick_Tel = order.Pick_Tel,
                            Address = order.Address,
                            District = order.District,
                            Province = order.Province,
                            Ward = order.Ward,
                            Name = order.Name,
                            Tel = order.Tel,
                            Details = GetOrderDetailModels(order.OrderID, order.OrderStatusID),
                            FeeShip = order.FeeShip,
                            Reason = order.Reason,
                            Pick_Time = order.Pick_Time,
                            ShipOrderID = shiporder.ShipStatusID,
                            FirebaseID=order.User.FirebaseID,
                        };
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    else
                    {
                        OrderModel model = new OrderModel
                        {
                            OrderID = order.OrderID,
                            StoreView = GetStoreViewModel(order.OrderID),
                            Create_Date = order.Create_Date,
                            UserID = order.UserID,
                            OrderStatus = _statusReposity.Value.GetOrderStatus(order.OrderStatusID),
                            PriceItem = GetPriceItemOrder(orderID),
                            Pick_Address = order.Pick_Address,
                            Pick_Province = order.Pick_Province,
                            Pick_District = order.Pick_District,
                            Pick_Ward = order.Pick_Ward,
                            Pick_Name = order.Pick_Name,
                            Pick_Tel = order.Pick_Tel,
                            Address = order.Address,
                            District = order.District,
                            Province = order.Province,
                            Ward = order.Ward,
                            Name = order.Name,
                            Tel = order.Tel,
                            Details = GetOrderDetailModels(order.OrderID, order.OrderStatusID),
                            FeeShip = order.FeeShip,
                        };
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
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

        public Result UpdateOrderAddress(int orderID, int AddressID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    Address address = GetAddressByID(AddressID);
                    order.Address = address.Context;
                    order.Province = address.Province;
                    order.District = address.District;
                    order.Ward = address.Ward;
                    order.Name = address.UserName;
                    order.Tel = address.Phone;
                    FeeReponse shipModel = _shipReposity.Value.GetFeeAsync(order.Province, order.District, address.Province, address.District, GetWeightOrder(orderID));
                    if (shipModel.success)
                    {
                        if (shipModel.fee.delivery)
                        {
                            order.FeeShip = shipModel.fee.fee;
                            _context.SaveChanges();
                            result = GetOrderInfo(order.OrderID);
                            return result;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Địa chỉ nằm ngoài vùng hoạt động giao hàng";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Địa chỉ không Hợp lệ";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }

                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
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

        public Result UpdateAmountOrderDetail(int orderDetailID, int amount)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderdetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID);
                if (orderdetail != null)
                {
                    if (GetSub_Item(orderdetail.Sub_ItemID).Amount >= amount)
                    {
                        orderdetail.Amount = amount;
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = orderdetail.Amount;
                        result.TotalPage = numpage;
                        return result;
                    }
                    result.Success = false;
                    result.Message = "Sản phẩm không đủ số lượng";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
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

        public double GetPriceItemOrder(int orderID)
        {
            try
            {
                double total = 0;
                var listorderdetail = _context.OrderDetails.Where(od => od.OrderID == orderID);

                if (_context.Orders.SingleOrDefault(o => o.OrderID == orderID).OrderStatusID == 1)
                {
                    if (listorderdetail.Count() > 0)
                    {
                        foreach (var item in listorderdetail.ToList())
                        {
                            total = total + item.PricePurchase * item.Amount * (1 - item.DiscountPurchase);
                        }
                    }
                }
                else
                {
                    if (listorderdetail.Count() > 0)
                    {
                        foreach (var item in listorderdetail.ToList())
                        {
                            total = total + GetSub_Item(item.Sub_ItemID).Price * item.Amount * (1 - GetDiscount(item.Sub_ItemID));
                        }
                    }
                }
                return total;
            }
            catch
            {
                return 0;
            }
        }

        public int NumFeedBackRateItem(int itemID)
        {
            var orderdetai = _context.OrderDetails.Where(od => od.Feedback_Rate != null && od.Sub_ItemID == _context.Sub_Items.FirstOrDefault(si => si.ItemID == itemID).Sub_ItemID);
            return orderdetai.Count();
        }

        public Result FeedBaclOrderDetail(FeedBackOrderDetail feedBack)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderdetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == feedBack.OrderDetaiID);
                if (orderdetail != null)
                {
                    orderdetail.FeedBack_Date = GetVnTime();
                    orderdetail.Feedback_Rate = (double)feedBack.Rate;
                    orderdetail.Feedback_Title = feedBack.Text;
                    orderdetail.Feedback_StatusID = 1;
                    var listImage = feedBack.feedbackImages;
                    var item = _context.Items.SingleOrDefault(i => i.ItemID == _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == orderdetail.Sub_ItemID).ItemID);
                    if (item != null)
                    {
                        int numRate = NumFeedBackRateItem(item.ItemID);
                        item.Rate = (item.Rate + feedBack.Rate) / (numRate + 1);
                        _context.SaveChanges();
                    }
                    if (listImage != null)
                    {
                        foreach (var image in listImage)
                        {
                            Guid myuuid = Guid.NewGuid();
                            string myuuidAsString = myuuid.ToString();
                            var filename = "eSMP" + orderdetail.OrderDetailID + myuuidAsString;
                            string path = _fileReposity.Value.UploadFile(image, filename).Result;

                            Image i = new Image();
                            i.Crete_date = GetVnTime();
                            i.FileName = filename;
                            i.Path = path;
                            i.IsActive = true;

                            Feedback_Image fi = new Feedback_Image();
                            fi.IsActive = true;
                            fi.OrderDetailID = orderdetail.OrderDetailID;
                            fi.Image = i;

                            _context.Feedback_Images.Add(fi);
                            _context.SaveChanges();
                        }

                    }
                    if (orderdetail.Feedback_StatusID != null)
                    {
                        FeedbackViewModel model = new FeedbackViewModel
                        {
                            Comment = orderdetail.Feedback_Title,
                            Create_Date = orderdetail.FeedBack_Date,
                            Feedback_Status = _statusReposity.Value.GetfeedbackStatus(orderdetail.Feedback_StatusID.Value),
                            orderDetaiID = orderdetail.OrderDetailID,
                            Rate = orderdetail.Feedback_Rate,
                            subItemImage = orderdetail.Sub_Item.Image.Path,
                            Sub_itemName = orderdetail.Sub_Item.Sub_ItemName,
                            ImagesFB = GetListImageFB(orderdetail.OrderDetailID),

                        };
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    else
                    {
                        FeedbackViewModel model = new FeedbackViewModel
                        {
                            Comment = orderdetail.Feedback_Title,
                            Create_Date = orderdetail.FeedBack_Date,
                            Feedback_Status = null,
                            orderDetaiID = orderdetail.OrderDetailID,
                            Rate = orderdetail.Feedback_Rate,
                            subItemImage = orderdetail.Sub_Item.Image.Path,
                            Sub_itemName = orderdetail.Sub_Item.Sub_ItemName,
                            ImagesFB = GetListImageFB(orderdetail.OrderDetailID),
                        };
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
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

        public Result GetOrdersWithShipstatus(int? userID, int? storeID, DateTime? dateFrom, DateTime? dateTo, int? shipOrderStatus, int? page, string? userName, int? orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orders = _context.Orders.AsQueryable();
                orders = orders.Where(o => o.OrderStatusID != 2);
                var num = orders.Count();
                if (userID.HasValue)
                {
                    orders = orders.Where(o => o.UserID == userID);
                }
                if (storeID.HasValue)
                {
                    orders = orders.Where(o => _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID).StoreID == storeID) != null) != null);
                }
                if (userName!=null)
                {
                    orders = orders.Where(o => EF.Functions.Collate(o.User.UserName, "SQL_Latin1_General_CP1_CI_AI").Contains(userName));
                }
                if (orderID.HasValue)
                {
                    orders = orders.Where(o => o.OrderID==orderID);
                }
                if (shipOrderStatus.HasValue)
                {
                    string status = shipOrderStatus + "";
                    switch (status)
                    {
                        case "-1":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "-1" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "-3" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "127" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "9" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "7" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "49" ||
                                                        o.OrderStatusID == 3
                            );
                            break;
                        case "1":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "1" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "-2" &&
                                                         o.OrderStatusID != 3
                            );
                            break;
                        case "2":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "2" &&
                                                         o.OrderStatusID != 3
                            );
                            break;
                        case "3":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "12" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "123" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "8" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "128" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "3" &&
                                                         o.OrderStatusID != 3
                            );
                            break;
                        case "4":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "-4" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "8" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "10" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "410" &&
                                                         o.OrderStatusID != 3
                            );
                            break;
                        case "5":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "5" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "45" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "6" &&
                                                         o.OrderStatusID != 3
                            );
                            break;
                        case "6":
                            orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "11" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "13" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "20" ||
                                                        _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID == "21" 
                            );
                            break;
                    }
                    if (dateFrom.HasValue)
                    {
                        string vnTimeZoneKey = "SE Asia Standard Time";
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateFrom.Value, vnTimeZone);

                        orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Create_Date >= VnTime);
                    }
                    if (dateTo.HasValue)
                    {
                        string vnTimeZoneKey = "SE Asia Standard Time";
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateTo.Value, vnTimeZone);

                        orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Create_Date <= VnTime);
                    }
                }
                else
                {
                    if (dateFrom.HasValue)
                    {
                        string vnTimeZoneKey = "SE Asia Standard Time";
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateFrom.Value, vnTimeZone);

                        orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Create_Date >= VnTime);
                    }
                    if (dateTo.HasValue)
                    {
                        string vnTimeZoneKey = "SE Asia Standard Time";
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateTo.Value, vnTimeZone);

                        orders = orders.Where(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Create_Date <= VnTime);
                    }
                }
                orders = orders.OrderByDescending(o => _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Create_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)orders.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    orders = orders.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                if (orders.Count() > 0)
                {
                    List<OrderModelView> list = new List<OrderModelView>();
                    foreach (var order in orders.ToList())
                    {
                        OrderModelView model = new OrderModelView
                        {
                            OrderID = order.OrderID,
                            Create_Date = order.Create_Date,
                            UserID = order.UserID,
                            PriceItem = GetPriceItemOrder(order.OrderID),
                            Pick_Address = order.Pick_Address,
                            Pick_Province = order.Pick_Province,
                            Pick_District = order.Pick_District,
                            Pick_Ward = order.Pick_Ward,
                            Pick_Name = order.Pick_Name,
                            Pick_Tel = order.Pick_Tel,
                            Address = order.Address,
                            District = order.District,
                            Province = order.Province,
                            Ward = order.Ward,
                            Name = order.Name,
                            Tel = order.Tel,
                            FeeShip = order.FeeShip,
                            OrderShip = GetShipOrder(order.OrderID),
                            OrderStatus = _statusReposity.Value.GetOrderStatus(order.OrderStatusID),
                            StoreView = GetStoreViewModel(order.OrderID),
                            Details = GetOrderDetailModels(order.OrderID, order.OrderStatusID),
                            Reason = order.Reason,
                            Pick_Time= order.Pick_Time,
                            FirebaseID=order.User.FirebaseID,
                        };
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = new ArrayList();
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

        public bool CancelOrder(int orderID)
        {
            try
            {
                var lisrdetail = _context.OrderDetails.Where(od => od.OrderID == orderID).ToList();
                foreach (var item in lisrdetail)
                {
                    var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID);
                    subItem.Amount = subItem.Amount + item.Amount;
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ShipViewModel GetShipOrder(int orderID)
        {
            var ship = _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.OrderID == orderID);
            if (ship == null)
                return null;
            else
            {
                ShipViewModel model = new ShipViewModel
                {
                    Create_Date = ship.Create_Date,
                    LabelID = ship.LabelID,
                    Reason = ship.Reason,
                    Reason_code = ship.Reason_code,
                    status = ship.ShipStatus.Status_Name,
                    ShipStatusID = ship.Status_ID,
                };
                return model;
            }
        }

        public List<Image> GetListImageFB(int orderdetailID)
        {
            var listIF = _context.Feedback_Images.Where(fi => fi.OrderDetailID == orderdetailID).ToList();
            var list = new List<Image>();
            if (listIF.Count > 0)
            {
                foreach (var image in listIF)
                {
                    list.Add(image.Image);
                }
                return list;
            }
            return new List<Image>();
        }

        public Result GetlistFeedback(int? page, bool isFeedback, int? userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderDetail = _context.OrderDetails.AsQueryable();
                orderDetail = orderDetail.Where(od => _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID && _context.ShipOrders.SingleOrDefault(so => so.OrderID == o.OrderID) != null) != null);
                if (userID.HasValue)
                {
                    orderDetail = orderDetail.Where(od => _context.Orders.SingleOrDefault(o => o.UserID == userID && o.OrderID == od.OrderID) != null);
                }
                if (isFeedback)
                {
                    orderDetail = orderDetail.Where(od => od.Feedback_StatusID != null);
                }
                else
                {
                    orderDetail = orderDetail.Where(od => od.Feedback_StatusID == null && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID && _context.ShipOrders.SingleOrDefault(so => so.OrderID == o.OrderID && so.Status_ID == "5") != null) != null);
                }

                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)orderDetail.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    orderDetail = orderDetail.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                List<FeedbackViewModel> list = new List<FeedbackViewModel>();

                if (orderDetail != null)
                {
                    foreach (var detail in orderDetail.ToList())
                    {
                        var orderstatus = _context.ShipOrders.SingleOrDefault(so => so.OrderID == detail.OrderID && so.Status_ID == "5");
                        if (detail.Feedback_StatusID != null)
                        {
                            FeedbackViewModel model = new FeedbackViewModel
                            {
                                Comment = detail.Feedback_Title,
                                Create_Date = detail.FeedBack_Date,
                                Feedback_Status = _statusReposity.Value.GetfeedbackStatus(detail.Feedback_StatusID.Value),
                                orderDetaiID = detail.OrderDetailID,
                                Rate = detail.Feedback_Rate,
                                subItemImage = detail.Sub_Item.Image.Path,
                                Sub_itemName = detail.Sub_Item.Sub_ItemName,
                                ImagesFB = GetListImageFB(detail.OrderDetailID),
                                Delivery_Date = orderstatus.Create_Date,

                            };
                            list.Add(model);
                        }
                        else
                        {
                            FeedbackViewModel model = new FeedbackViewModel
                            {
                                Comment = detail.Feedback_Title,
                                Create_Date = detail.FeedBack_Date,
                                Feedback_Status = null,
                                orderDetaiID = detail.OrderDetailID,
                                Rate = detail.Feedback_Rate,
                                subItemImage = detail.Sub_Item.Image.Path,
                                Sub_itemName = detail.Sub_Item.Sub_ItemName,
                                ImagesFB = GetListImageFB(detail.OrderDetailID),
                                Delivery_Date = orderstatus.Create_Date,

                            };
                            list.Add(model);
                        }
                    }
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

        public Result GetFeedbackDetail(int orderDetailID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID && od.Feedback_StatusID != null);
                if (orderDetail != null)
                {
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderDetail.OrderID);
                    if (orderDetail.Feedback_StatusID != null)
                    {
                        FeedbackDetailModel mode = new FeedbackDetailModel
                        {
                            UserID = order.UserID,
                            ItemID=orderDetail.Sub_Item.ItemID,
                            UserName = order.User.UserName,
                            Useravatar = order.User.Image.Path,
                            orderDetaiID = orderDetail.OrderDetailID,
                            Comment = orderDetail.Feedback_Title,
                            Create_Date = orderDetail.FeedBack_Date,
                            Feedback_Status = _statusReposity.Value.GetfeedbackStatus(orderDetail.Feedback_StatusID.Value),
                            Rate = orderDetail.Feedback_Rate,
                            subItemImage = orderDetail.Sub_Item.Image.Path,
                            Sub_itemName = orderDetail.Sub_Item.Sub_ItemName,
                            ImagesFB = GetListImageFB(orderDetailID),
                        };
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = mode;
                        result.TotalPage = numpage;
                        return result;
                    }
                    else
                    {
                        FeedbackDetailModel mode = new FeedbackDetailModel
                        {
                            UserID = order.UserID,
                            UserName = order.User.UserName,
                            Useravatar = order.User.Image.Path,
                            orderDetaiID = orderDetail.OrderDetailID,
                            Comment = orderDetail.Feedback_Title,
                            Create_Date = orderDetail.FeedBack_Date,
                            Feedback_Status = null,
                            Rate = orderDetail.Feedback_Rate,
                            subItemImage = orderDetail.Sub_Item.Image.Path,
                            Sub_itemName = orderDetail.Sub_Item.Sub_ItemName,
                            ImagesFB = GetListImageFB(orderDetailID),
                        };
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = mode;
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Đơn hàng chưa được đánhh giá";
                result.Data = "";
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

        public Result HiddenFeedback(int orderDetailID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID && od.Feedback_StatusID != null && od.Feedback_StatusID != 2);
                if (orderDetail != null)
                {
                    orderDetail.Feedback_StatusID = 3;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn đánh giá thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Đơn hàng chưa được đánhh giá";
                result.Data = "";
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

        public Result BlockFeedback(int orderDetailID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID && od.Feedback_StatusID != null);
                if (orderDetail != null)
                {
                    orderDetail.Feedback_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn đánh giá thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Đơn hàng chưa được đánhh giá";
                result.Data = "";
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

        public Result CheckPay(int orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 1);
                if (order != null)
                {
                    result.Success = true;
                    result.Message = "Thanh toán thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Thanh toán thất bại";
                result.Data = "";
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

        public int GetUserIDByOrderID(int orderID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                return order.UserID;
            }
            catch
            {
                return -1;
            }
        }

        public int GetSuppilerIDByOrderID(int orderID)
        {
            try
            {
                var orderdetail = _context.OrderDetails.FirstOrDefault(od => od.OrderID == orderID);
                var store = GetStoreBySubItemID(orderdetail.Sub_ItemID);
                return store.User.UserID;
            }
            catch
            {
                return -1;
            }
        }

        public int GetUserIDByOrderDetailID(int orderDetailID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => _context.OrderDetails.FirstOrDefault(od=>od.OrderDetailID==orderDetailID).OrderID==o.OrderID);
                return order.UserID;
            }
            catch
            {
                return -1;
            }
        }

        public int GetSuppilerIDByOrderDetailID(int orderDetailID)
        {
            var orderdetail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailID == orderDetailID);
            var store = GetStoreBySubItemID(orderdetail.Sub_ItemID);
            return store.User.UserID;
        }
    }
}
