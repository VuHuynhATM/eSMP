using eSMP.Models;
using eSMP.Services.ShipRepo;
using eSMP.VModels;
using Firebase.Auth;

namespace eSMP.Services.OrderRepo
{
    public class OrderRepository:IOrderReposity
    {
        private readonly WebContext _context;
        private readonly IShipReposity _shipReposity;

        public OrderRepository(WebContext context, IShipReposity shipReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
        }

        public Result AddOrderDetail(OrderDetailAdd orderDetail)
        {
            Result result = new Result();
            try
            {
                if (CheckSubItemInOrder(orderDetail.Sub_ItemID, orderDetail.UserID)) {
                    var odexist = _context.OrderDetails.SingleOrDefault(od => od.Sub_ItemID == orderDetail.Sub_ItemID && !_context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID && o.UserID==orderDetail.UserID).IsPay);
                    odexist.Amount= odexist.Amount+orderDetail.Amount;
                    odexist.DiscountPurchase = GetDiscount(orderDetail.Sub_ItemID);
                    odexist.PricePurchase = GetSub_Item(orderDetail.Sub_ItemID).Price;
                    //ship
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == odexist.OrderID);
                    int weight = GetWeightOfSubItem(orderDetail.Sub_ItemID)*orderDetail.Amount;
                    int weightOrder = GetWeightOrder(order.OrderID);
                    order.FeeShip = _shipReposity.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, weight + weightOrder).fee;

                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thêm vào giỏ hàng thành công";
                    result.Data = odexist;
                    return result;
                }
                else
                {
                    if (CheckStoreOrder(orderDetail.Sub_ItemID, orderDetail.UserID))
                    {
                        int storeID = GetStoreBySubItemID(orderDetail.Sub_ItemID).StoreID;
                        var order = _context.Orders.SingleOrDefault(o => !o.IsPay && o.UserID == orderDetail.UserID && _context.OrderDetails.SingleOrDefault(od => _context.Items.SingleOrDefault(i => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID).ItemID == i.ItemID).StoreID == storeID).OrderID==o.OrderID);
                        //ship
                        int weight = GetWeightOfSubItem(orderDetail.Sub_ItemID) * orderDetail.Amount;
                        int weightOrder=GetWeightOrder(order.OrderID);
                        order.FeeShip = _shipReposity.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, weight+weightOrder).fee;

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
                        result.Data = newod;
                        return result;
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
                        o.Province= userAddress.Province;
                        o.District= userAddress.District;
                        o.Ward= userAddress.Ward;
                        o.Address= userAddress.Context;
                        o.Tel= userAddress.Phone;

                        o.Create_Date = DateTime.UtcNow;
                        o.IsPay = false;
                        o.UserID = orderDetail.UserID;
                        //ship
                        int weight = GetWeightOfSubItem(orderDetail.Sub_ItemID) * orderDetail.Amount;
                        o.FeeShip = _shipReposity.GetFeeAsync(o.Province, o.District, o.Pick_Province, o.Pick_District, weight).fee;

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
                        result.Data = od;
                        return result;
                    }
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Thêm vào giỏ hàng thất bại";
                result.Data = "";
                return result;
            }
        }
        public int GetWeightOrder(int OrderID)
        {
            var orderDetail=_context.OrderDetails.Where(x => x.OrderID == OrderID).ToList();
            int weight = 0;
            foreach (var detail in orderDetail)
            {
                weight=weight+GetWeightOfSubItem(detail.Sub_ItemID)*detail.Amount;
            }
            return weight;
        }
        public Address GetStoreAddress (int subItemID)
        {
            var storeaddressID=GetStoreBySubItemID(subItemID).AddressID;
            return _context.Addresss.SingleOrDefault(s => s.AddressID == storeaddressID);
        }
        public int GetWeightOfSubItem(int subItemID)
        {
            var weight = _context.Specification_Values.SingleOrDefault(sv => sv.ItemID == subItemID && sv.SpecificationID == 2).Value;
            return int.Parse(weight);
        }
        public bool CheckStoreOrder(int sub_ItemID, int userID)
        {
            try
            {
                int storeID = GetStoreBySubItemID(sub_ItemID).StoreID;
                var order = _context.Orders.SingleOrDefault(o => !o.IsPay && o.UserID==userID && _context.OrderDetails.SingleOrDefault(od=> _context.Items.SingleOrDefault(i=> _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID).ItemID==i.ItemID).StoreID==storeID).OrderID==o.OrderID);
                if (order == null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
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
                return _context.Items.SingleOrDefault(i=>i.ItemID==_context.Sub_Items.SingleOrDefault(si=>si.Sub_ItemID==sub_ItemID).ItemID).Discount;
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
                return _context.Sub_Items.SingleOrDefault(si=>si.Sub_ItemID==sub_ItemID);
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
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.Sub_ItemID == sub_ItemID && !_context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID && o.UserID==userID).IsPay);
                if(orderDetail == null)
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
            try
            {
                var listOrderDetail = _context.OrderDetails.Where(od => od.OrderID == orderID && !_context.Orders.SingleOrDefault(o => o.OrderID == orderID).IsPay).ToList();
                if (listOrderDetail.Count > 0)
                {
                    foreach (var item in listOrderDetail)
                    {
                        _context.OrderDetails.Remove(item);
                    }
                }
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && !o.IsPay);
                if(order == null)
                {
                    result.Success = false;
                    result.Message = "Xoá không thành công";
                    result.Data = "";
                    return result;
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Xoá thành công";
                result.Data = order;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result DeleteOrderDetail(int orderDetailID)
        {
            Result result = new Result();
            try
            {
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID && !_context.Orders.SingleOrDefault(o => o.OrderID == od.OrderID).IsPay);
                if( orderDetail == null)
                {
                    result.Success = false;
                    result.Message = "Xoá không thành công";
                    result.Data = "";
                    return result;
                }
                _context.OrderDetails.Remove(orderDetail);
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Xoá thành công";
                result.Data = orderDetail;
                return result;
            }
            catch
            {
                result.Success=false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Address GetAddressByID(int addressID)
        {
            return _context.Addresss.SingleOrDefault(a => a.AddressID == addressID);
        }
        public Feedback_Status GetFeedback_Status(int? feedback_StatusID)
        {
            return _context.Feedback_Statuses.SingleOrDefault(fb => fb.Feedback_StatusID == feedback_StatusID);
        }
        public Item GetItem(int subItemID)
        {
            return _context.Items.SingleOrDefault(i => i.ItemID == _context.Sub_Items.FirstOrDefault(si=>si.Sub_ItemID==subItemID).ItemID);
        }
        public Image GetImage(int imageID)
        {
            return _context.Images.SingleOrDefault(i => i.ImageID == imageID);
        }
        public List<OrderDetailModel> GetOrderDetailModels(int orderID, bool isPay)
        {
            List<OrderDetailModel> list=new List<OrderDetailModel>();
            var listOrderDetails=_context.OrderDetails.Where(o => o.OrderID == orderID);
            if(listOrderDetails.Count() > 0)
            {
                foreach(var orderDetail in listOrderDetails.ToList())
                {
                    if (isPay)
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
                            Feedback_Status = GetFeedback_Status(orderDetail.Feedback_StatusID),
                            Sub_ItemID = GetSub_Item(orderDetail.Sub_ItemID).Sub_ItemID,
                            Sub_ItemName = GetSub_Item(orderDetail.Sub_ItemID).Sub_ItemName,
                            sub_ItemImage = GetImage(GetSub_Item(orderDetail.Sub_ItemID).ImageID).Path,
                            ItemID= GetSub_Item(orderDetail.Sub_ItemID).ItemID,
                        };
                        list.Add(model);
                    }
                    else
                    {
                        OrderDetailModel model = new OrderDetailModel
                        {
                            Amount = orderDetail.Amount,
                            DiscountPurchase = GetItem(orderDetail.Sub_ItemID).Discount,
                            OrderDetailID = orderDetail.OrderDetailID,
                            PricePurchase = GetSub_Item(orderDetail.Sub_ItemID).Price,
                            Sub_ItemID = GetSub_Item(orderDetail.Sub_ItemID).Sub_ItemID,
                            Sub_ItemName = GetSub_Item(orderDetail.Sub_ItemID).Sub_ItemName,
                            sub_ItemImage = GetImage(GetSub_Item(orderDetail.Sub_ItemID).ImageID).Path,
                            ItemID = GetSub_Item(orderDetail.Sub_ItemID).ItemID,
                        };
                        list.Add(model);
                    }
                    
                }
            }
            return list;
        }

        public Result GetAllOrder(int userID, bool? isPay)
        {
            Result result = new Result();
            try
            {
                List<OrderModel> list = new List<OrderModel>();
                var listOrder=_context.Orders.Where(o=>o.UserID ==userID).AsQueryable();
                if (isPay.HasValue)
                {
                    listOrder = listOrder.Where(o => o.IsPay == isPay);
                }
                if (listOrder.Count() > 0)
                {
                    foreach (var order in listOrder.ToList())
                    {
                        OrderModel model = new OrderModel
                        {
                            OrderID = order.OrderID,
                            Create_Date = order.Create_Date,
                            UserID = order.UserID,
                            IsPay = order.IsPay,
                            Pick_Address=order.Pick_Address,
                            Pick_Province=order.Pick_Province,
                            Pick_District=order.Pick_District,
                            Pick_Ward=order.Pick_Ward,
                            Pick_Name=order.Pick_Name,
                            Pick_Tel=order.Pick_Tel,
                            Address=order.Address,
                            District=order.District,
                            Province=order.Province,
                            Ward=order.Ward,
                            Name=order.Name,
                            Tel=order.Tel,
                            Details=GetOrderDetailModels(order.OrderID,order.IsPay),
                            FeeShip=order.FeeShip,
                        };
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = list;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa có sản phảm trong giỏ hàng";
                result.Data = list;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result GetOrderInfo(int orderID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    OrderModel model = new OrderModel
                    {
                        OrderID = order.OrderID,
                        Create_Date = order.Create_Date,
                        UserID = order.UserID,
                        IsPay = order.IsPay,
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
                        Details = GetOrderDetailModels(order.OrderID, order.IsPay),
                        FeeShip= order.FeeShip,
                    };
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = model;
                    return result;
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result UpdateOrderAddress(int orderID, int AddressID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    Address address=GetAddressByID(AddressID);
                    order.Address = address.Context;
                    order.Province = address.Province;
                    order.District = address.District;
                    order.Ward = address.Ward;
                    order.Name = address.UserName;
                    order.Tel = address.Phone;
                    order.FeeShip = _shipReposity.GetFeeAsync(order.Province, order.District, address.Province, address.District, GetWeightOrder(orderID)).fee;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = order;
                    return result;
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result UpdateAmountOrderDetail(int orderDetailID, int amount)
        {
            Result result = new Result();
            try
            {
                var orderdetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == orderDetailID);
                if (orderdetail != null)
                {
                    orderdetail.Amount = orderdetail.Amount + amount;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = orderdetail;
                    return result;
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
    }
}
