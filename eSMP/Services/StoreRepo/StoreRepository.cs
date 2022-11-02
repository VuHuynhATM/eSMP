using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.VModels;

namespace eSMP.Services.StoreRepo
{
    public class StoreRepository : IStoreReposity
    {
        private readonly WebContext _context;
        private readonly Lazy<IShipReposity> _shipReposity;
        private readonly Lazy<IOrderReposity> _orderReposity;

        public StoreRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
        }
        public User GetUser(int userID)
        {
            return _context.Users.SingleOrDefault(u => u.UserID == userID);
        }
        public Result CteateStore(StoreRegister store)
        {
            Result result = new Result();
            try
            {
                if (CheckStore(store.UserID))
                {
                    result.Success = false;
                    result.Message = "Nhà cung cấp đã có cửa hàng";
                    result.Data = GetStore(store.UserID);
                    return result;
                }
                if (store != null)
                {
                    Address address = new Address
                    {
                        UserName = GetUser(store.UserID).UserName,
                        Phone = GetUser(store.UserID).Phone,
                        Context = store.contextAddress,
                        Province = store.Province,
                        District = store.District,
                        Ward = store.Ward,
                        Latitude = store.latitude,
                        Longitude = store.longitude,
                        IsActive = true
                    };
                    Image image = new Image
                    {
                        Crete_date = DateTime.UtcNow,
                        FileName = store.ImageName,
                        IsActive = true,
                        Path = store.ImagePath
                    };
                    Store storeRegister = new Store
                    {
                        StoreName = store.StoreName,
                        Create_date = DateTime.UtcNow,
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
                    return result;
                }
                result.Success = false;
                result.Message = "Kiểm tra lại thông tin cửa hàng";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thông";
                result.Data = "";
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
                    Address =store.Address,
                    Image = GetImage(store.ImageID),
                    Store_Status = GetStatus(store.Store_StatusID),
                    UserID = store.UserID,
                    Asset=store.Asset,
                    Actice_Date = store.Actice_Date,
                    MomoTransactionID=store.MomoTransactionID,
                    Actice_Amount=store.AmountActive,
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
        public Store_Status GetStatus(int statusID)
        {
            try
            {
                var status = _context.Store_Statuses.SingleOrDefault(s => s.Store_StatusID == statusID);
                if (status != null)
                    return status;
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
        public Result GetAllStore()
        {
            Result result = new Result();
            List<StoreModel> list = new List<StoreModel>();
            try
            {
                var listStore = _context.Stores.ToList();
                foreach (var store in listStore)
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
                        Store_Status = GetStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset=store.Asset,
                        Actice_Date = store.Actice_Date,    
                        MomoTransactionID = store.MomoTransactionID,
                        Actice_Amount=store.AmountActive,
                    };
                    list.Add(model);
                }
                result.Success = true;
                result.Message = "Thành công";
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

        public Result StoreDetail(int storeID)
        {
            Result result = new Result();
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
                        Store_Status = GetStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset= store.Asset,
                        Actice_Date = store.Actice_Date,
                        MomoTransactionID = store.MomoTransactionID,
                        Actice_Amount = store.AmountActive,
                    };
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    return result;
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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

        public Result StoreUpdateInfo(StoreUpdateInfo info)
        {
            Result result = new Result();
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == info.StoreID);
                if (store != null)
                {
                    store.StoreName = info.StoreName;
                    store.Email = info.Email;
                    store.Phone = info.Phone;
                    store.Pick_date = info.Pick_date;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Chỉnh sửa cửa hàng thành công";
                    result.Data = GetStore(store.UserID);
                    return result;
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result HiddenStore(int storeID)
        {
            Result result = new Result();
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
                        return result;
                    }
                    store.Store_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn của hàng thành công";
                    result.Data = store;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result BlockStore(int storeID)
        {
            Result result = new Result();
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
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result UnHiddenStore(int storeID)
        {
            Result result = new Result();
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
                        return result;
                    }
                    store.Store_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn cửa hàng thành công";
                    result.Data = store;
                    return result;

                }
                result.Success = false;
                result.Message = "cửa hàng không tồn tại";
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

        public Result UpdateAddress(int storeID, Address address)
        {
            Result result=new Result();
            try
            {
                var addressUpdate = _context.Addresss.SingleOrDefault(a => a.AddressID == address.AddressID);
                if (addressUpdate != null)
                {
                    addressUpdate.UserName = address.UserName;
                    addressUpdate.Phone= address.Phone;
                    addressUpdate.Context = address.Context;
                    addressUpdate.Province=address.Province;
                    addressUpdate.District=address.District;
                    addressUpdate.Ward=address.Ward;
                    addressUpdate.Latitude = address.Latitude;
                    addressUpdate.Longitude = address.Longitude;
                    _context.SaveChanges();
                    UpdateOrderaddress(storeID,address.Province, address.District, address.Ward, address.Context, address.UserName, address.Phone);
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = addressUpdate;
                    return result;
                }
                result.Success = false;
                result.Message = "Không tìm thấy địa chỉ";
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

        public void UpdateOrderaddress(int storeID,string provine, string district, string ward,string address, string name, string tel)
        {
            try
            {
                var listOrder = _context.Orders.Where(o => o.OrderID == _context.OrderDetails.FirstOrDefault(od => od.Sub_ItemID == _context.Sub_Items.FirstOrDefault(si => si.ItemID == _context.Items.FirstOrDefault(i => i.StoreID == storeID).ItemID).Sub_ItemID).OrderID && o.OrderStatusID==2).ToList();
                if(listOrder.Count > 0)
                {
                    foreach (var item in listOrder)
                    {
                        item.Pick_Province = provine;
                        item.Pick_District= district;
                        item.Pick_Ward = ward;
                        item.Pick_Address = address;
                        item.Pick_Name = name;
                        item.Pick_Tel= tel;
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
                        Store_Status = GetStatus(store.Store_StatusID),
                        UserID = store.UserID,
                        Asset=store.Asset,
                        MomoTransactionID=store.MomoTransactionID,
                        Actice_Date = store.Actice_Date,
                        Actice_Amount=store.AmountActive,
                    };
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    return result;
                }
                result.Success = false;
                result.Message = "Cửa hàng không tồn tại";
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
