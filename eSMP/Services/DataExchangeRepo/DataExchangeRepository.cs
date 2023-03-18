using eSMP.Models;
using eSMP.Services.FileRepo;
using eSMP.Services.StatusRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Image = eSMP.Models.Image;

namespace eSMP.Services.DataExchangeRepo
{
    public class DataExchangeRepository : IDataExchangeReposity
    {
        private readonly WebContext _context;
        private readonly IFileReposity _fileReposity;
        private readonly IStatusReposity _statusReposity;
        private int PAGE_SIZE = 10;

        public DataExchangeRepository(WebContext context, IFileReposity fileReposity, IStatusReposity statusReposity)
        {
            _context = context;
            _fileReposity = fileReposity;
            _statusReposity = statusReposity;
        }
        public Result FinishStoreDataExchange(DataExchangeStoreFinish DataExchange)
        {
            Result result = new Result();
            try
            {
                var Exchange = _context.DataExchangeStores.FirstOrDefault(os => os.ExchangeStoreID == DataExchange.ExchangeStoreID);
                if (Exchange != null)
                {
                    var file = DataExchange.File;
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    string filename = Exchange.ExchangeStoreID + "-" + myuuidAsString;
                    string path = _fileReposity.UploadFile(file, filename).Result;
                    Exchange.ExchangeStatusID = 1;
                    Image img = new Image();
                    img.Crete_date = GetVnTime();
                    img.FileName = filename;
                    img.Path = path;
                    img.IsActive = true;
                    Exchange.Image = img;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                result.Success = false;
                result.Message = "Đối soát không tồn tại";
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
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }

        public Result GetStoreDataExchanges(int? storeID, int? orderID, DateTime? from, DateTime? to, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listExchange = _context.DataExchangeStores.AsQueryable();
                if (storeID.HasValue)
                {
                    listExchange = listExchange.Where(de =>_context.OrderDetails.FirstOrDefault(od=>od.OrderID==de.OrderID && od.Sub_Item.Item.StoreID==storeID)!=null );
                }
                if (orderID.HasValue)
                {
                    listExchange = listExchange.Where(de => de.OrderID == orderID);
                }
                if (from.HasValue)
                {
                    listExchange = listExchange.Where(de => de.Create_date >= from);
                }
                if (to.HasValue)
                {
                    listExchange = listExchange.Where(de => de.Create_date <= to);
                }
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listExchange.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listExchange = listExchange.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                var r = new List<DataExchangeStoreModel>();
                if (listExchange.Count() > 0)
                    foreach (var exchange in listExchange.ToList())
                    {
                        DataExchangeStoreModel model = new DataExchangeStoreModel
                        {
                            Create_date = exchange.Create_date,
                            ExchangePrice = exchange.ExchangePrice,
                            ExchangeStatus = _statusReposity.GetStoreDataExchangeStatus(exchange.ExchangeStatusID),
                            ExchangeStoreID = exchange.ExchangeStoreID,
                            ExchangeStoreName = exchange.ExchangeStoreName,
                            Image = exchange.Image,
                            OrderID = exchange.OrderID,
                            AfterBuyServiceID=exchange.AfterBuyServiceID,
                        };
                        r.Add(model);
                    }
                result.Success = true;
                result.Message = "thành công";
                result.Data = r;
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

        public Result GetUserDataExchanges(int? UserID, int? orderID, DateTime? from, DateTime? to, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listExchange = _context.DataExchangeUsers.AsQueryable();
                if (UserID.HasValue)
                {
                    listExchange = listExchange.Where(de => de.Order.UserID==UserID);
                }
                if (orderID.HasValue)
                {
                    listExchange = listExchange.Where(de => de.OrderID == orderID);
                }
                if (from.HasValue)
                {
                    listExchange = listExchange.Where(de => de.Create_date >= from);
                }
                if (to.HasValue)
                {
                    listExchange = listExchange.Where(de => de.Create_date <= to);
                }
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listExchange.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listExchange = listExchange.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                var r = new List<DataExchangeUserModel>();
                if (listExchange.Count() > 0)
                    foreach (var exchange in listExchange.ToList())
                    {
                        DataExchangeUserModel model = new DataExchangeUserModel
                        {
                            Create_date = exchange.Create_date,
                            ExchangePrice = exchange.ExchangePrice,
                            ExchangeStatus = _statusReposity.GetUserDataExchangeStatus(exchange.ExchangeStatusID),
                            ExchangeUserID = exchange.ExchangeUserID,
                            ExchangeUserName = exchange.ExchangeUserName,
                            Image = exchange.Image,
                            OrderID = exchange.OrderID,
                            BankName = exchange.BankName,
                            CardHoldName = exchange.CardHoldName,
                            CardNum = exchange.CardNum,
                            AfterBuyServiceID= exchange.AfterBuyServiceID,
                        };
                        r.Add(model);
                    }
                result.Success = true;
                result.Message = "thành công";
                result.Data = r;
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

        public Result FinishUserDataExchange(DataExchangeUserFinish DataExchange)
        {
            Result result = new Result();
            try
            {
                var Exchange = _context.DataExchangeUsers.FirstOrDefault(os => os.ExchangeUserID == DataExchange.ExchangeUserID);
                if (Exchange != null)
                {
                    var file = DataExchange.File;
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    string filename = Exchange.ExchangeUserID + "-" + myuuidAsString;
                    string path = _fileReposity.UploadFile(file, filename).Result;
                    Exchange.ExchangeStatusID = 1;
                    Image img = new Image();
                    img.Crete_date = GetVnTime();
                    img.FileName = filename;
                    img.Path = path;
                    img.IsActive = true;
                    Exchange.Image = img;
                    Exchange.ExchangePrice= DataExchange.Price;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                result.Success = false;
                result.Message = "Đối soát không tồn tại";
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

        public Result AddCardUserDataExchange(DataExchangeUserAddCard DataExchange)
        {
            Result result = new Result();
            try
            {
                var Exchange = _context.DataExchangeUsers.FirstOrDefault(os => os.ExchangeUserID == DataExchange.ExchangeUserID);
                if (Exchange != null)
                {
                    Exchange.CardNum = DataExchange.CardNum;
                    Exchange.CardHoldName = DataExchange.CardHoldName;
                    Exchange.BankName=DataExchange.BankName;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                result.Success = false;
                result.Message = "Đối soát không tồn tại";
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
