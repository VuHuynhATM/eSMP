using eSMP.Models;
using eSMP.Services.FileRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.Services.StatusRepo;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using System.Net;
using User = eSMP.Models.User;

namespace eSMP.Services.StoreRepo
{
    public class StoreRepository : IStoreReposity
    {
        private readonly WebContext _context;
        private readonly Lazy<IShipReposity> _shipReposity;
        private readonly Lazy<IOrderReposity> _orderReposity;
        private readonly Lazy<IFileReposity> _fileReposity;
        private readonly Lazy<IStatusReposity> _statusReposity;

        public static int PAGE_SIZE { get; set; } = 10;

        public StoreRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity, Lazy<IFileReposity> fileReposity, Lazy<IStatusReposity> statusReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
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
        public User GetUser(int userID)
        {
            return _context.Users.SingleOrDefault(u => u.UserID == userID);
        }
        public Result CteateStore(StoreRegister store)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (CheckStore(store.UserID))
                {
                    result.Success = false;
                    result.Message = "Nhà cung cấp đã có cửa hàng";
                    result.Data = GetStore(store.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                if (store != null)
                {
                    var user = GetUser(store.UserID);
                    Address address = new Address
                    {
                        UserName = user.UserName,
                        Phone = user.Phone,
                        Context = store.contextAddress,
                        Province = store.Province,
                        District = store.District,
                        Ward = store.Ward,
                        Latitude = store.latitude,
                        Longitude = store.longitude,
                        IsActive = true
                    };

                    var file = store.File;
                    var date = GetVnTime();
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    string filename = store.UserID + "-" + myuuidAsString;
                    string path = _fileReposity.Value.UploadFile(file, filename).Result;
                    Image image = new Image
                    {
                        Crete_date = GetVnTime(),
                        FileName = filename,
                        IsActive = true,
                        Path = path
                    };
                    Store storeRegister = new Store
                    {
                        StoreName = store.StoreName,
                        Create_date = GetVnTime(),
                        Email = store.Email,
                        Phone = store.Phone,
                        Store_StatusID = 3,
                        Pick_date = 1,
                        Address = address,
                        UserID = store.UserID,
                        Image = image,
                    };
                    _context.Stores.Add(storeRegister);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Tạo cửa hàng thành công";
                    result.Data = GetStore(store.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Kiểm tra lại thông tin cửa hàng";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thông";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public StoreModel GetStore(int UserID)
        {
            var store = _context.Stores.SingleOrDefault(s => s.UserID == UserID);
            if (store != null)
            {
                StoreModel model = new StoreModel
                {
                    StoreID = store.StoreID,
                    StoreName = store.StoreName,
                    Create_date = store.Create_date,
                    Email = store.Email,
                    Phone = store.Phone,
                    Pick_date = store.Pick_date,
                    Address = store.Address,
                    Image = GetImage(store.ImageID),
                    Store_Status = _statusReposity.Value.GetStoreStatus(store.Store_StatusID),
                    UserID = store.UserID,
                    Asset = store.Asset,
                    Actice_Date = store.Actice_Date,
                    MomoTransactionID = store.MomoTransactionID,
                    Actice_Amount = store.AmountActive,
                    FirebaseID = store.User.FirebaseID,
                    FCM_Firebase = store.User.FCM_Firebase,
                    TotalActiveItem = GetTotalItemActive(store.StoreID),
                    TotalBlockItem = GetTotalItemBlock(store.StoreID),
                    TotalWatingItem = GetTotalItemWatting(store.StoreID),
                    TotalOrder = GetTotalOrder(store.StoreID),
                    TotalCancelOrder = GetTotalCancelOrder(store.StoreID),
                    TotalRating = GetTotalRating(store.StoreID),
                };
                return model;
            }
            return null;
        }

        public Address GetAddressByStoreID(int StoreID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == StoreID);

                if (store != null)
                    return store.Address;
                return null;
            }
            catch
            {
                return null;
            }
        }
        public Image GetImage(int imageID)
        {
            Image image = _context.Images.SingleOrDefault(i => i.ImageID == imageID);
            if (image != null)
            {
                return image;
            }
            return null;
        }
        public bool CheckStore(int UserID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.UserID == UserID);
                if (store != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public Result GetAllStore(string? search, int? page, int? statusID)
        {
            Result result = new Result();
            int numpage = 1;
            List<StoreModel> list = new List<StoreModel>();
            try
            {
                var listStore = _context.Stores.AsQueryable();
                if (search != null)
                {
                    listStore = listStore.Where(s => EF.Functions.Collate(s.StoreName, "SQL_Latin1_General_CP1_CI_AI").Contains(search));
                }
                if (statusID.HasValue)
                {
                    listStore = listStore.Where(s => s.Store_StatusID == statusID);
                }
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listStore.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listStore = listStore.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                foreach (var store in listStore.ToList())
                {
                    StoreModel model = new StoreModel
                    {
                        StoreID = store.StoreID,
                        StoreName = store.StoreName,
                        Create_date = store.Create_date,
                        Email = store.Email,
                        Phone = store.Phone,
                        Pick_date = store.Pick_date,
                        Address = store.Address,
                        Image = GetImage(store.ImageID),
                        Store_Status = _statusReposity.Value.GetStoreStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset = store.Asset,
                        Actice_Date = store.Actice_Date,
                        MomoTransactionID = store.MomoTransactionID,
                        Actice_Amount = store.AmountActive,
                        FCM_Firebase = store.User.FCM_Firebase,
                        FirebaseID = store.User.FirebaseID,
                    };
                    list.Add(model);
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

        public Result StoreDetail(int storeID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    StoreModel model = new StoreModel
                    {
                        StoreID = store.StoreID,
                        StoreName = store.StoreName,
                        Create_date = store.Create_date,
                        Email = store.Email,
                        Phone = store.Phone,
                        Pick_date = store.Pick_date,
                        Address = store.Address,
                        Image = GetImage(store.ImageID),
                        Store_Status = _statusReposity.Value.GetStoreStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset = store.Asset,
                        Actice_Date = store.Actice_Date,
                        MomoTransactionID = store.MomoTransactionID,
                        Actice_Amount = store.AmountActive,
                        FirebaseID = store.User.FirebaseID,
                        FCM_Firebase = store.User.FCM_Firebase,
                        TotalActiveItem = GetTotalItemActive(storeID),
                        TotalBlockItem = GetTotalItemBlock(storeID),
                        TotalWatingItem = GetTotalItemWatting(storeID),
                        TotalOrder = GetTotalOrder(storeID),
                        TotalCancelOrder = GetTotalCancelOrder(storeID),
                        TotalRating = GetTotalRating(storeID),
                    };
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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

        public Result StoreUpdateInfo(StoreUpdateInfo info)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == info.StoreID);
                if (store != null)
                {
                    if (info.StoreName != null)
                    {
                        store.StoreName = info.StoreName;
                    }
                    if (info.Email != null)
                    {
                        store.Email = info.Email;
                    }
                    if (info.Phone != null)
                    {
                        store.Phone = info.Phone;
                    }
                    if (info.Pick_date != null)
                    {
                        store.Pick_date = (int)info.Pick_date;
                    }
                    if (info.File != null)
                    {
                        var img = _context.Images.SingleOrDefault(i => i.ImageID == store.ImageID);
                        if (img != null)
                        {
                            var date = GetVnTime();
                            Guid myuuid = Guid.NewGuid();
                            string myuuidAsString = myuuid.ToString();
                            string filename = store.StoreID + "-" + myuuidAsString;
                            string path = _fileReposity.Value.UploadFile(info.File, filename).Result;
                            string pathDelete = img.Path;
                            string imageDelete = img.FileName;
                            if (_fileReposity.Value.DeleteFileASYNC(imageDelete).Result)
                            {
                                img.Crete_date = GetVnTime();
                                img.FileName = filename;
                                img.Path = path;
                            }
                        }
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Chỉnh sửa cửa hàng thành công";
                        result.Data = GetStore(store.UserID);
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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

        public StoreViewModel GetStoreModel(int storeID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    StoreViewModel model = new StoreViewModel
                    {
                        StoreID = store.StoreID,
                        StoreName = store.StoreName,
                        Imagepath = GetImage(store.ImageID).Path,
                        FirebaseID = store.User.FirebaseID,
                        FCM_Firebase = store.User.FCM_Firebase,
                        storeStatusID = store.Store_StatusID,
                    };
                    return model;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public Result ActiveStore(int storeID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    store.Store_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Kích hoạt cửa hàng thành công";
                    result.Data = store;
                    result.TotalPage = numpage;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result HiddenStore(int storeID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    if (store.Store_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "Cửa hàng hiện bị khoá";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                    store.Store_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn của hàng thành công";
                    result.Data = store;
                    result.TotalPage = numpage;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result BlockStore(int storeID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    store.Store_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Khoá cửa hàng thành công";
                    result.Data = store;
                    result.TotalPage = numpage;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result UnHiddenStore(int storeID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {
                    if (store.Store_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "Cửa hàng hiện bị khoá";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                    store.Store_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn cửa hàng thành công";
                    result.Data = store;
                    result.TotalPage = numpage;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result UpdateAddress(int storeID, Address address)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var addressUpdate = _context.Addresss.SingleOrDefault(a => a.AddressID == address.AddressID);
                if (addressUpdate != null)
                {
                    addressUpdate.UserName = address.UserName;
                    addressUpdate.Phone = address.Phone;
                    addressUpdate.Context = address.Context;
                    addressUpdate.Province = address.Province;
                    addressUpdate.District = address.District;
                    addressUpdate.Ward = address.Ward;
                    addressUpdate.Latitude = address.Latitude;
                    addressUpdate.Longitude = address.Longitude;
                    _context.SaveChanges();
                    UpdateOrderaddress(storeID, address.Province, address.District, address.Ward, address.Context, address.UserName, address.Phone);
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = addressUpdate;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Không tìm thấy địa chỉ";
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

        public void UpdateOrderaddress(int storeID, string provine, string district, string ward, string address, string name, string tel)
        {
            try
            {
                var listOrder = _context.Orders.Where(o => o.OrderID == _context.OrderDetails.FirstOrDefault(od => od.Sub_ItemID == _context.Sub_Items.FirstOrDefault(si => si.ItemID == _context.Items.FirstOrDefault(i => i.StoreID == storeID).ItemID).Sub_ItemID).OrderID && o.OrderStatusID == 2).ToList();
                if (listOrder.Count > 0)
                {
                    foreach (var item in listOrder)
                    {
                        item.Pick_Province = provine;
                        item.Pick_District = district;
                        item.Pick_Ward = ward;
                        item.Pick_Address = address;
                        item.Pick_Name = name;
                        item.Pick_Tel = tel;
                        item.FeeShip = _shipReposity.Value.GetFeeAsync(item.Province, item.District, provine, district, _orderReposity.Value.GetWeightOrder(item.OrderID)).fee.fee;
                    }
                }
                _context.SaveChanges();
            }
            catch
            {
                return;
            }
        }

        public Result GetStorebyuserID(int userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.UserID == userID);
                if (store != null)
                {
                    StoreModel model = new StoreModel
                    {
                        StoreID = store.StoreID,
                        StoreName = store.StoreName,
                        Create_date = store.Create_date,
                        Email = store.Email,
                        Phone = store.Phone,
                        Pick_date = store.Pick_date,
                        Address = store.Address,
                        Image = GetImage(store.ImageID),
                        Store_Status = _statusReposity.Value.GetStoreStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset = store.Asset,
                        MomoTransactionID = store.MomoTransactionID,
                        Actice_Date = store.Actice_Date,
                        Actice_Amount = store.AmountActive,
                        FCM_Firebase = store.User.FCM_Firebase,
                        FirebaseID = store.User.FirebaseID,
                    };
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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

        public bool CheckStoreFirebase(string firebaseID)
        {
            Result result = new Result();
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.User.FirebaseID == firebaseID && s.Store_StatusID == 1);
                if (store != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckStoreActive(int userID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.User.isActive && s.Store_StatusID == 1 && s.UserID == userID);
                if (store != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public double GetTotalRating(int storeID)
        {
            try
            {
                double present = 0;
                var listFeedback = _context.OrderDetails.Where(od => od.Feedback_StatusID != null && od.Sub_Item.Item.StoreID == storeID);
                if (listFeedback.Count() > 0)
                {
                    double rate = 0;
                    foreach (var feedback in listFeedback.ToList())
                    {
                        rate = rate + feedback.Feedback_Rate.Value;
                    }
                    present = rate / listFeedback.Count();
                }
                return present;
            }
            catch
            {
                return 0;
            }
        }
        public int GetTotalOrder(int storeID)
        {
            try
            {
                double num = 0;
                var listorder = _context.Orders.Where(o => _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && od.Sub_Item.Item.StoreID == storeID) != null && o.OrderStatusID != 2);
                return listorder.Count();
            }
            catch
            {
                return 0;
            }
        }
        public int GetTotalCancelOrder(int storeID)
        {
            try
            {
                double num = 0;
                var listorder = _context.Orders.Where(o => _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && _context.Sub_Items.FirstOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID && i.StoreID == storeID) != null) != null) != null && o.OrderStatusID != 3); ;
                return listorder.Count();
            }
            catch
            {
                return 0;
            }
        }
        public int GetTotalItemActive(int storeID)
        {
            try
            {
                int num = 0;
                var listItem = _context.Items.Where(i => i.StoreID == storeID && i.Item_StatusID != 2 && i.Item_StatusID != 3);
                num = listItem.Count();
                return num;
            }
            catch
            {
                return 0;
            }
        }
        public int GetTotalItemWatting(int storeID)
        {
            try
            {
                int num = 0;
                var listItem = _context.Items.Where(i => i.StoreID == storeID && i.Item_StatusID == 3);
                num = listItem.Count();
                return num;
            }
            catch
            {
                return 0;
            }
        }
        public int GetTotalItemBlock(int storeID)
        {
            try
            {
                int num = 0;
                var listItem = _context.Items.Where(i => i.StoreID == storeID && i.Item_StatusID == 2);
                num = listItem.Count();
                return num;
            }
            catch
            {
                return 0;
            }
        }

        public Result GetPriceActive()
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                if (system != null)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = system.AmountActive;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Không tìm thấy địa chỉ";
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
    }
}
