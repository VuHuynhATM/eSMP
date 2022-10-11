using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public class StoreRepository : IStoreReposity
    {
        private readonly WebContext _context;

        public StoreRepository(WebContext context)
        {
            _context = context;
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
                        Store_StatusID = 1,
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
                    Address = GetAddress(store.AddressID),
                    Image = GetImage(store.ImageID),
                    Store_Status = GetStatus(store.Store_StatusID),
                    UserID=store.UserID,
                };
                return model;
            }
            return null;
        }

        public Address GetAddress(int AddressID)
        {
            try
            {
                var address = _context.Addresss.SingleOrDefault(s => s.AddressID == AddressID);
                if (address != null)
                    return address;
                return null;
            }
            catch
            {
                return null;
            }
        }
        public Address GetAddressByStoreID(int StoreID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == StoreID);

                if (store != null)
                    return GetAddress(store.AddressID);
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
        public Boolean CheckStore(int UserID)
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
                        Address = GetAddress(store.AddressID),
                        Image = GetImage(store.ImageID),
                        Store_Status = GetStatus(store.Store_StatusID),
                        UserID=store.UserID,
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
                        Address = GetAddress(store.AddressID),
                        Image = GetImage(store.ImageID),
                        Store_Status = GetStatus(store.Store_StatusID),
                        UserID= store.UserID,
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
                var store=_context.Stores.SingleOrDefault(s => s.StoreID == info.StoreID);
                if (store != null)
                {
                    store.StoreName=info.StoreName;
                    store.Email=info.Email;
                    store.Phone=info.Phone;
                    store.Pick_date=info.Pick_date;
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
                        StoreName=store.StoreName,
                        Imagepath=GetImage(store.ImageID).Path,
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
                var store=_context.Stores.SingleOrDefault(s => s.StoreID == storeID);
                if (store != null)
                {

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
            throw new NotImplementedException();
        }

        public Result BlockStore(int storeID)
        {
            throw new NotImplementedException();
        }

        public Result UnHiddenStore(int storeID)
        {
            throw new NotImplementedException();
        }
    }
}
