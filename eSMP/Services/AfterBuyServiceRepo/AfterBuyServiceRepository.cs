using Castle.Core.Internal;
using eSMP.Models;
using eSMP.Services.ShipRepo;
using eSMP.VModels;
using Newtonsoft.Json;

namespace eSMP.Services.AfterBuyServiceRepo
{
    public class AfterBuyServiceRepository : IAfterBuyServiceReposity
    {
        private readonly WebContext _context;
        private readonly IShipReposity _shipReposity;

        public AfterBuyServiceRepository(WebContext context, IShipReposity shipReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }
        public Result CreateChangeService(AfterBuyServiceModel serviceCreate)
        {
            Result result = new Result();
            try
            {
                DateTime currentDate = GetVnTime();
                //kt order cos ton taij ko
                IEnumerable<ServiceDetaiAdd> listServiceDetail = JsonConvert.DeserializeObject<IEnumerable<ServiceDetaiAdd>>(serviceCreate.List_ServiceDetail);
                if (listServiceDetail.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "Danh sách sản phẩm trả trống";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                var orderDetails = _context.OrderDetails.Where(od => od.OrderID == serviceCreate.OrderID).ToList();
                if (orderDetails.Count() < 0)
                {
                    result.Success = false;
                    result.Message = "Mã đơn hàng không đúng";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                var ordership = _context.ShipOrders.FirstOrDefault(so => so.OrderID == serviceCreate.OrderID && so.Status_ID == "5");
                //lay thoi gian nhan hang
                if (ordership == null)
                {
                    result.Success = false;
                    result.Message = "Không thể trả hàng lúc này";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                //kt detail cos hopwj le hay khong
                if (!CheckDetailService(orderDetails, (List<ServiceDetaiAdd>)listServiceDetail, ordership.Create_Date))
                {
                    result.Success = false;
                    result.Message = "Mã đơn chi tiết không đúng";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                double price = 0;
                foreach (var serviceDetail in listServiceDetail)
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        if (orderDetail.OrderDetailID == serviceDetail.DetailID)
                        {
                            price=price+orderDetail.PricePurchase*(1-orderDetail.DiscountPurchase)*100* serviceDetail.Amount;
                            //kiem tra han tra hang
                            if (currentDate < ordership.Create_Date.AddDays(orderDetail.ReturnAndExchange))
                            {
                                result.Success = false;
                                result.Message = "Đơn hàng " + orderDetail.Sub_Item.Sub_ItemName + "(Mã:" + orderDetail.OrderDetailID + " )" + "Hết hạn trả hàng";
                                result.Data = "";
                                result.TotalPage = 1;
                                return result;
                            }
                            var afterservicelist = _context.ServiceDetails.Where(afs => afs.OrderDetailID == serviceDetail.DetailID);
                            int NumOrderWasProcess = afterservicelist.Sum(afs => afs.Amount);
                            if(orderDetail.Amount< NumOrderWasProcess + serviceDetail.Amount)
                            {
                                result.Success = false;
                                result.Message = "Đơn hàng " + orderDetail.Sub_Item.Sub_ItemName + "(Mã:" + orderDetail.OrderDetailID + " ) chỉ được phép đổi "+ (orderDetail.Amount- NumOrderWasProcess) +"sản phẩm";
                                result.Data = "";
                                result.TotalPage = 1;
                                return result;
                            }
                        }
                    }
                }
                //tao du lieu
                Address Cusaddress = _context.Addresss.SingleOrDefault(a => a.AddressID == serviceCreate.AddressID);
                if (Cusaddress == null)
                {
                    result.Success = false;
                    result.Message = "địa chỉ lấy hàng không tồn tại";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                Store store = _context.Stores.SingleOrDefault(s => _context.OrderDetails.FirstOrDefault(od => od.OrderID == serviceCreate.OrderID && od.Sub_Item.Item.StoreID == s.StoreID) != null);
                if (store == null)
                {
                    result.Success = false;
                    result.Message = "Không lấy được địa chỉ store";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                AfterBuyService afterBuyService = new AfterBuyService
                {
                    Create_Date = currentDate,
                    ServiceType = serviceCreate.ServiceType,
                    RefundPrice = 0,
                    Store_Address = store.Address.Context,
                    Store_Name=store.Address.UserName,
                    Store_District=store.Address.District,
                    Store_Province=store.Address.Province,
                    Store_Ward=store.Address.Ward,
                    Store_Tel=store.Address.Phone,
                    User_Address= Cusaddress.Context,
                    User_District=Cusaddress.District,
                    User_Province=Cusaddress.Province,
                    User_Name=Cusaddress.UserName,
                    User_Tel=Cusaddress.Phone,
                    User_Ward=Cusaddress.Ward,
                    ServicestatusID= 3,
                };
                foreach (var serviceDetail in listServiceDetail)
                {
                    ServiceDetail newserviceDetail = new ServiceDetail
                    {
                        AfterBuyService = afterBuyService,
                        Amount = serviceDetail.Amount,
                        OrderDetailID = serviceDetail.DetailID
                    };
                    _context.ServiceDetails.Add(newserviceDetail);
                }
                //treo giai ngan giao dich
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == serviceCreate.OrderID);
                if(order != null)
                {
                    order.OrderStatusID = 6;
                }
                _context.SaveChanges();
                //taoj donwd ship
                result.Success = true;
                result.Message = "Thành công";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
        }
        public bool CheckDetailService(List<OrderDetail> listdetail, List<ServiceDetaiAdd> listservicedetail, DateTime rereceiveDate)
        {
            int num = 0;
            DateTime currentDate = GetVnTime();
            foreach (var serviceDetail in listservicedetail)
            {
                foreach (var orderDetail in listdetail)
                {
                    if (orderDetail.OrderDetailID == serviceDetail.DetailID && currentDate > rereceiveDate.AddDays(orderDetail.ReturnAndExchange))
                    {
                        num++;
                    }
                }
            }
            if (num == listservicedetail.Count())
            {
                return true;
            }
            return false;
        }

        public Result AcceptService(int serviceID)
        {
            Result result = new Result();
            try
            {
                AfterBuyService afterBuyService = _context.AfterBuyServices.SingleOrDefault(afs => afs.AfterBuyServiceID == serviceID);
                if(afterBuyService != null)
                {
                    result.Success = false;
                    result.Message = "Dịch vụ không tồn tại";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                ShipReponse shipReponse = _shipReposity.CreateOrderService(afterBuyService.ServicestatusID, "user");
                if (shipReponse != null)
                {
                    result.Success = false;
                    result.Message = "Lỗi hệ thống";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                if (shipReponse.success)
                {
                    afterBuyService.ServicestatusID = 5;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = shipReponse.message;
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
        }

        public Result CancelService(int serviceID, string reason)
        {
            Result result = new Result();
            try
            {
                AfterBuyService afterBuyService = _context.AfterBuyServices.SingleOrDefault(afs => afs.AfterBuyServiceID == serviceID);
                if(afterBuyService == null)
                {
                    result.Success = false;
                    result.Message = "Mã dịch vụ không tồn tại";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                if (afterBuyService.ServicestatusID == 2)
                {
                    result.Success = false;
                    result.Message = "Dịch vụ này đã hủy";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                if (afterBuyService.ServicestatusID != 3 && afterBuyService.ServicestatusID != 6)
                {
                    result.Success = false;
                    result.Message = "Dịch vụ này không thể hủy";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                afterBuyService.ServicestatusID = 2;
                afterBuyService.Reason = reason;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
        }

        public Result WarningService(int serviceID)
        {
            Result result = new Result();
            try
            {
                AfterBuyService afterBuyService = _context.AfterBuyServices.SingleOrDefault(afs => afs.AfterBuyServiceID == serviceID);
                if (afterBuyService == null)
                {
                    result.Success = false;
                    result.Message = "Mã dịch vụ không tồn tại";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                if (afterBuyService.ServicestatusID != 2)
                {
                    result.Success = false;
                    result.Message = "Dịch vụ này không thể báo cáo";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                afterBuyService.ServicestatusID = 6;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
        }
    }
}
