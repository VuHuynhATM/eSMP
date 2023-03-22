using Castle.Core.Internal;
using eSMP.Models;
using eSMP.Services.ShipRepo;
using eSMP.Services.StatusRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using User = eSMP.Models.User;

namespace eSMP.Services.AfterBuyServiceRepo
{
    public class AfterBuyServiceRepository : IAfterBuyServiceReposity
    {
        private readonly WebContext _context;
        private readonly IShipReposity _shipReposity;
        private readonly IStatusReposity _statusReposity;
        private readonly IStoreReposity _storeReposity;

        public static int PAGE_SIZE { get; set; } = 6;
        public AfterBuyServiceRepository(WebContext context, IShipReposity shipReposity, IStatusReposity statusReposity, IStoreReposity storeReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _statusReposity = statusReposity;
            _storeReposity = storeReposity;
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
                if (!CheckDetailService(orderDetails, (List<ServiceDetaiAdd>)listServiceDetail))
                {
                    result.Success = false;
                    result.Message = "Mã đơn chi tiết không đúng";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                double price = 0;
                foreach (var serviceDetail in listServiceDetail.ToList())
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        if (orderDetail.OrderDetailID == serviceDetail.DetailID)
                        {
                            price = price + orderDetail.PricePurchase * (1 - orderDetail.DiscountPurchase) * 100 * serviceDetail.Amount;
                            //kiem tra han tra hang
                            if (currentDate > ordership.Create_Date.AddDays(orderDetail.ReturnAndExchange))
                            {
                                result.Success = false;
                                result.Message = "Đơn hàng " + orderDetail.Sub_Item.Sub_ItemName + "(Mã:" + orderDetail.OrderDetailID + " )" + "Hết hạn trả hàng";
                                result.Data = "";
                                result.TotalPage = 1;
                                return result;
                            }
                            var afterservicelist = _context.ServiceDetails.Where(afs => afs.OrderDetailID == serviceDetail.DetailID);
                            int NumOrderWasProcess = afterservicelist.Sum(afs => afs.Amount);
                            if (orderDetail.Amount < NumOrderWasProcess + serviceDetail.Amount)
                            {
                                result.Success = false;
                                result.Message = "Đơn hàng " + orderDetail.Sub_Item.Sub_ItemName + "(Mã:" + orderDetail.OrderDetailID + " ) chỉ được phép đổi " + (orderDetail.Amount - NumOrderWasProcess) + "sản phẩm";
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
                    Store_Name = store.Address.UserName,
                    Store_District = store.Address.District,
                    Store_Province = store.Address.Province,
                    Store_Ward = store.Address.Ward,
                    Store_Tel = store.Address.Phone,
                    User_Address = Cusaddress.Context,
                    User_District = Cusaddress.District,
                    User_Province = Cusaddress.Province,
                    User_Name = Cusaddress.UserName,
                    User_Tel = Cusaddress.Phone,
                    User_Ward = Cusaddress.Ward,
                    ServicestatusID = 3,
                    Text=serviceCreate.Text,
                    PackingLinkCus=serviceCreate.PackingLinkCus,
                };
                foreach (var serviceDetail in listServiceDetail.ToList())
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
                if (order != null)
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
        public bool CheckDetailService(List<OrderDetail> listdetail, List<ServiceDetaiAdd> listservicedetail)
        {
            int num = 0;
            foreach (var serviceDetail in listservicedetail)
            {
                foreach (var orderDetail in listdetail)
                {
                    if (orderDetail.OrderDetailID == serviceDetail.DetailID)
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
                if (afterBuyService != null)
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
                    afterBuyService.FeeShipFisrt = shipReponse.order.fee;
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
                if (afterBuyService == null)
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

        public Result GetServices(int? serviceID, int? storeID, int? orderID, int? userID, DateTime? from, DateTime? to, int? serviceType, int? servicestatusID, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var afterbuyservices = _context.AfterBuyServices.AsQueryable();
                if (serviceID.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(af => af.AfterBuyServiceID == serviceID);
                }
                if (storeID.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(afs => _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afs.AfterBuyServiceID && _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sd.OrderDetail.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID).StoreID == storeID) != null) != null);
                }
                if (orderID != null)
                {
                    afterbuyservices = afterbuyservices.Where(afs => _context.ServiceDetails.FirstOrDefault(sd => sd.OrderDetail.OrderID == orderID && afs.AfterBuyServiceID == sd.AfterBuyServiceID) != null);
                }
                if (userID != null)
                {
                    afterbuyservices = afterbuyservices.Where(afs => _context.ServiceDetails.FirstOrDefault(sd => sd.OrderDetail.Order.UserID == userID && afs.AfterBuyServiceID == sd.AfterBuyServiceID) != null);
                }
                if (from.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(afs => afs.Create_Date >= from);
                }
                if (to.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(afs => afs.Create_Date <= to);
                }
                if (serviceType.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(afs => afs.ServiceType == serviceType);
                }
                if (servicestatusID.HasValue)
                {
                    afterbuyservices = afterbuyservices.Where(afs => afs.ServicestatusID == servicestatusID);
                }
                afterbuyservices = afterbuyservices.OrderByDescending(afs => afs.Create_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)afterbuyservices.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    afterbuyservices = afterbuyservices.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                if (afterbuyservices.Count() > 0)
                {
                    List<AfterBuyServiceModelView> list = new List<AfterBuyServiceModelView>();
                    foreach (var service in afterbuyservices.ToList())
                    {
                        var user=GetUser(service.AfterBuyServiceID);
                        bool hasStoreDatachange = false;
                        bool hasUsersDatachange = false;
                        if (_context.DataExchangeStores.FirstOrDefault(ds => ds.AfterBuyServiceID == service.AfterBuyServiceID) != null)
                        {
                            hasStoreDatachange = true;
                        }
                        if (_context.DataExchangeUsers.FirstOrDefault(ds => ds.AfterBuyServiceID == service.AfterBuyServiceID) != null)
                        {
                            hasUsersDatachange = true;
                        }
                        var order = _context.Orders.FirstOrDefault(o => _context.ServiceDetails.FirstOrDefault(sd => sd.OrderDetail.OrderID == o.OrderID) != null);
                        AfterBuyServiceModelView model = new AfterBuyServiceModelView
                        {
                            AfterBuyServiceID = service.AfterBuyServiceID,
                            Create_Date = service.Create_Date,
                            UserID = user.UserID,
                            Store_Address = service.Store_Address,
                            Store_Province = service.Store_Province,
                            Store_District = service.Store_District,
                            Store_Ward = service.Store_Ward,
                            Store_Name = service.Store_Name,
                            Store_Tel = service.Store_Tel,
                            User_Address = service.User_Address,
                            User_District = service.User_District,
                            User_Province = service.User_Province,
                            User_Ward = service.User_Ward,
                            User_Name = service.User_Name,
                            User_Tel = service.User_Tel,
                            FeeShipFisrt = service.FeeShipFisrt,
                            FeeShipSercond = service.FeeShipSercond,
                            OrderShip = GetShipOrder(service.AfterBuyServiceID),
                            Servicestatus = _statusReposity.GetServiceStatus(service.ServicestatusID),
                            ServiceType = _statusReposity.GetServiceType(service.ServiceType),
                            StoreView = GetStoreViewModel(service.AfterBuyServiceID),
                            Details = GetServiceDetailModels(service.AfterBuyServiceID),
                            Reason = service.Reason,
                            Pick_Time = service.estimated_pick_time,
                            Deliver_time = service.estimated_deliver_time,
                            FirebaseID = user.FirebaseID,
                            RefundPrice = service.RefundPrice,
                            HasStoreDataExchange = hasStoreDatachange,
                            HasUserDataExchange = hasUsersDatachange,
                            PackingLinkCus=service.PackingLinkCus,
                            PackingLink= order.PackingLink,
                            OrderID= order.OrderID,
                            Text=service.Text,
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
        public ShipViewModel GetShipOrder(int serviceID)
        {
            var ship = _context.ShipOrders.OrderBy(so => so.Create_Date).LastOrDefault(so => so.AfterBuyServiceID == serviceID);
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
        public StoreViewModel GetStoreViewModel(int serviceID)
        {
            try
            {
                var orderdetail = _context.OrderDetails.FirstOrDefault(od => _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == serviceID && od.OrderDetailID==sd.OrderDetailID) != null);
                var store = GetStoreBySubItemID(orderdetail.Sub_ItemID);
                StoreViewModel model = _storeReposity.GetStoreModel(store.StoreID);
                return model;
            }
            catch
            {
                return null;
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
        public List<AfterBuyServiceDetailModel> GetServiceDetailModels(int serviceID)
        {
            try
            {
                List<AfterBuyServiceDetailModel> list = new List<AfterBuyServiceDetailModel>();
                var listServiceDetails = _context.ServiceDetails.Where(sd => sd.AfterBuyServiceID == serviceID);
                if (listServiceDetails.Count() > 0)
                {
                    foreach (var serviceDetail in listServiceDetails.ToList())
                    {
                        AfterBuyServiceDetailModel model = new AfterBuyServiceDetailModel
                        {
                            AfterBuyServiceDetailID = serviceDetail.AfterBuyServiceID,
                            Amount = serviceDetail.Amount,
                            DiscountPurchase =serviceDetail.OrderDetail.DiscountPurchase,
                            PricePurchase =serviceDetail.OrderDetail.PricePurchase,
                            Sub_ItemID=serviceDetail.OrderDetail.Sub_ItemID,
                            sub_ItemImage=serviceDetail.OrderDetail.Sub_Item.Image.Path,
                            Sub_ItemName= serviceDetail.OrderDetail.Sub_Item.Sub_ItemName,
                            ItemID=serviceDetail.OrderDetail.Sub_Item.ItemID
                        };
                        list.Add(model);
                    }
                }
                return list;
            }
            catch
            {
                return new List<AfterBuyServiceDetailModel>();
            }
        }
        public User GetUser(int serciceID)
        {
            try
            {
                return _context.Users.FirstOrDefault(u=>_context.ServiceDetails.FirstOrDefault(sd=>sd.AfterBuyServiceID==serciceID && sd.OrderDetail.Order.UserID==u.UserID)!=null);
            }
            catch
            {
                return null;
            }
        }
    }
}
