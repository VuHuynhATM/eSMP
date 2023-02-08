using eSMP.Models;
using eSMP.Services.FileRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.StatusRepo;
using eSMP.VModels;
using System;
using System.Linq.Expressions;

namespace eSMP.Services.StoreAssetRepo
{
    public class AssetRepository : IAssetReposity
    {
        private readonly WebContext _context;
        private readonly IOrderReposity _orderReposity;
        private readonly IFileReposity _fileReposity;
        private readonly IStatusReposity _statusReposity;
        private readonly int PAGE_SIZE = 10;
        public AssetRepository(WebContext context, IOrderReposity orderReposity, IFileReposity fileReposity, IStatusReposity statusReposity)
        {
            _context = context;
            _orderReposity = orderReposity;
            _fileReposity = fileReposity;
            _statusReposity = statusReposity;
        }
        public Store GetStore(int orderID)
        {
            return _context.Stores.SingleOrDefault(s => _context.Orders.SingleOrDefault(o => o.OrderID == orderID && _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID).StoreID == s.StoreID) != null) != null) != null);
        }
        public long GetMomoTransaction(int orderID)
        {
            return _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID).MomoTransactionID;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }
        public bool SendPriceTostore(int OrderID)
        {
            try
            {


                var esmpSystem = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == OrderID && o.OrderStatusID == 1 && _context.ShipOrders.OrderByDescending(so => so.Create_Date).LastOrDefault(so => so.OrderID == o.OrderID).Status_ID.Equals("5"));
                if (order == null)
                    return false;
                var store = GetStore(order.OrderID);
                var orderPrice = _orderReposity.GetPriceItemOrder(order.OrderID);
                var orderFee = order.FeeShip;
                var afterprice = orderPrice * (1 - esmpSystem.Commission_Precent) + orderFee;
                OrderStore_Transaction transaction = new OrderStore_Transaction();
                transaction.OrderID = order.OrderID;
                transaction.Create_Date = GetVnTime();
                transaction.Price = afterprice;
                transaction.StoreID = store.StoreID;
                transaction.IsActive = true;

                OrderSystem_Transaction system_Transaction = new OrderSystem_Transaction();
                system_Transaction.Create_Date = GetVnTime();
                system_Transaction.OrderStore_Transaction = transaction;
                system_Transaction.Price = orderPrice * esmpSystem.Commission_Precent;
                system_Transaction.IsActive = true;
                system_Transaction.SystemID = 1;

                var storeupdate = _context.Stores.SingleOrDefault(s => s.StoreID == store.StoreID);
                storeupdate.Asset = storeupdate.Asset + transaction.Price;
                _context.OrderSystem_Transactions.Add(system_Transaction);
                esmpSystem.Asset = esmpSystem.Asset + system_Transaction.Price;
                _context.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public Result SystemWithdrawal(SystemWithdrawalM request)
        {
            Result result = new Result();
            try
            {
                Guid myuuid = Guid.NewGuid();
                string myuuidAsString = myuuid.ToString();
                var filename = "eSMP1" + myuuidAsString;
                string path = _fileReposity.UploadFile(request.File, filename).Result;

                Image image = new Image();
                image.Crete_date = GetVnTime();
                image.FileName = filename;
                image.Path = path;
                image.IsActive = true;

                System_Withdrawal system_Withdrawal = new System_Withdrawal();
                system_Withdrawal.SystemID = 1;
                system_Withdrawal.Price = request.Price;
                system_Withdrawal.Context = request.Context;
                system_Withdrawal.Create_Date = GetVnTime();
                system_Withdrawal.IsActive = true;
                system_Withdrawal.Image = image;
                _context.System_Withdrawals.Add(system_Withdrawal);

                var systemeSMP = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);

                if (systemeSMP.Asset < request.Price)
                {
                    result.Success = false;
                    result.Message = "Tài khoản hhienej khhongo đủ số dư";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                systemeSMP.Asset = systemeSMP.Asset - request.Price;
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
        public Result GetALlSystemWitdrawl(int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listWithdrawal = _context.System_Withdrawals.AsQueryable();
                if (From.HasValue)
                {
                    listWithdrawal = listWithdrawal.Where(sw => sw.Create_Date >= From);
                }
                if (To.HasValue)
                {
                    listWithdrawal = listWithdrawal.Where(sw => sw.Create_Date <= To);
                }
                listWithdrawal = listWithdrawal.OrderByDescending(sw => sw.Create_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listWithdrawal.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listWithdrawal = listWithdrawal.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                listWithdrawal = listWithdrawal.Where(sw => sw.SystemID == 1);
                List<SystemWithdrawalView> list = new List<SystemWithdrawalView>();
                if (listWithdrawal.Count() > 0)
                {
                    foreach (var item in listWithdrawal.ToList())
                    {
                        SystemWithdrawalView model = new SystemWithdrawalView
                        {
                            System_WithdrawalID = item.System_WithdrawalID,
                            Context = item.Context,
                            Create_Date = item.Create_Date,
                            Image = item.Image,
                            IsActive = item.IsActive,
                            Price = item.Price
                        };
                        list.Add(model);
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
                result.TotalPage = 1;
                return result;
            }
        }
        public OrderStore_Transaction GetOrderStore_Transaction(int orderStore_TransactionID)
        {
            return _context.OrderStore_Transactions.SingleOrDefault(ost => ost.OrderStore_TransactionID == orderStore_TransactionID);
        }
        public Result GetALlReveneu(int? page, DateTime? From, DateTime? To, int? orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listReveneu = _context.OrderSystem_Transactions.AsQueryable();
                if (From.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Create_Date >= From);
                }
                if (To.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Create_Date <= To);
                }
                if (orderID.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.OrderStore_Transaction.OrderID == orderID);
                }
                listReveneu = listReveneu.OrderByDescending(ost => ost.Create_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listReveneu.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listReveneu = listReveneu.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                listReveneu = listReveneu.Where(ot => ot.SystemID == 1);
                List<OrderSystemTransactionView> list = new List<OrderSystemTransactionView>();
                if (listReveneu.Count() > 0)
                {
                    foreach (var item in listReveneu.ToList())
                    {
                        OrderSystemTransactionView model = new OrderSystemTransactionView
                        {
                            Create_Date = item.Create_Date,
                            IsActive = item.IsActive,
                            OrderSystem_TransactionID = item.OrderSystem_TransactionID,
                            OrderStore_TransactionModel = new OrderStore_TransactionModel
                            {
                                Create_Date = item.OrderStore_Transaction.Create_Date,
                                IsActive = item.OrderStore_Transaction.IsActive,
                                OrderID = item.OrderStore_Transaction.OrderID,
                                OrderStore_TransactionID = item.OrderStore_TransactionID,
                                Price = item.OrderStore_Transaction.Price,
                                MomoTransaction = GetMomoTransaction(item.OrderStore_Transaction.OrderID)
                            },
                            Price = item.Price,
                            StoreID = GetStore(item.OrderStore_Transaction.OrderID).StoreID,
                        };
                        list.Add(model);
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
                result.TotalPage = 1;
                return result;
            }
        }
        public Result GetSystemInfo()
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                Systeminfo systeminfo = new Systeminfo();
                var eSMp = _context.eSMP_Systems.SingleOrDefault(es => es.SystemID == 1);
                var cus = _context.Users.Where(u => u.RoleID == 2);
                var sup = _context.Users.Where(u => u.RoleID == 3);
                var item = _context.Items.Where(i => i.Item_StatusID == 1 && i.Store.Store_StatusID == 1);
                var order = _context.Orders.Where(o => o.OrderStatusID == 1);
                if (eSMp != null)
                {
                    systeminfo.SystemID = eSMp.SystemID;
                    systeminfo.AmountActive = eSMp.AmountActive;
                    systeminfo.Refund_Precent = eSMp.Refund_Precent;
                    systeminfo.Commission_Precent = eSMp.Commission_Precent;
                    systeminfo.IsActive = eSMp.IsActive;
                    systeminfo.Asset = eSMp.Asset;
                }
                if (cus != null)
                {
                    systeminfo.TotalCustomer = cus.Count();
                }
                if (sup != null)
                {
                    systeminfo.TotalSupplier = sup.Count();
                }
                if (item != null)
                {
                    systeminfo.TotalItem = item.Count();
                }
                if (order != null)
                {
                    systeminfo.TotalOrder = order.Count();
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = systeminfo;
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

        public Result GetStoreReveneu(int storeID, int? page, DateTime? From, DateTime? To, int? orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listReveneu = _context.OrderStore_Transactions.AsQueryable();
                listReveneu = listReveneu.Where(ot => ot.StoreID == storeID);
                if (From.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Create_Date >= From);
                }
                if (To.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Create_Date <= To);
                }
                if (orderID.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.OrderID == orderID);
                }
                listReveneu = listReveneu.OrderByDescending(ost => ost.Create_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listReveneu.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listReveneu = listReveneu.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }

                List<OrderStore_TransactionModel> list = new List<OrderStore_TransactionModel>();
                if (listReveneu.Count() > 0)
                {
                    foreach (var item in listReveneu.ToList())
                    {
                        OrderStore_TransactionModel model = new OrderStore_TransactionModel
                        {
                            Create_Date = item.Create_Date,
                            IsActive = item.IsActive,
                            OrderStore_TransactionID = item.OrderStore_TransactionID,
                            Price = item.Price,
                            OrderID = item.OrderID,
                            MomoTransaction = GetMomoTransaction(item.OrderID)
                        };
                        list.Add(model);
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

        public Result CreateStoreWithdrawal(StoreWithdrawalRequest request)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == request.StoreID);
                if (store != null)
                {
                    if (store.Asset < request.Price)
                    {
                        result.Success = false;
                        result.Message = "Số dư tài khoản hiện không đủ";
                        result.Data = "";
                        return result;
                    }
                    Store_Withdrawal withdrawal = new Store_Withdrawal();
                    withdrawal.Withdrawal_StatusID = 1;
                    withdrawal.Price = request.Price;
                    withdrawal.BankID = request.BankID;
                    withdrawal.NumBankCart = request.NumBankCart;
                    withdrawal.OwnerBankCart = request.OwnerBankCart;
                    withdrawal.Reason = "";
                    withdrawal.Create_Date = GetVnTime();
                    withdrawal.StoreID = request.StoreID;

                    _context.Store_Withdrawals.Add(withdrawal);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = withdrawal;
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

        public Result ProcessStoreWithdrawal(int storeWithhdrawalID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.SingleOrDefault(stw => stw.Store_WithdrawalID == storeWithhdrawalID);
                if (storeWitdrawal != null)
                {
                    storeWitdrawal.Withdrawal_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = storeWitdrawal;
                    return result;
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Result CancelStoreWithdrawal(int storeWithhdrawalID, string reason)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.SingleOrDefault(stw => stw.Store_WithdrawalID == storeWithhdrawalID && stw.Withdrawal_StatusID != 4);
                if (storeWitdrawal != null)
                {
                    storeWitdrawal.Withdrawal_StatusID = 3;
                    storeWitdrawal.Reason = reason;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = storeWitdrawal;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Result SuccessStoreWithdrawal(StoreWithdrawalSuccessRequest request)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.SingleOrDefault(stw => stw.Store_WithdrawalID == request.Store_WithdrawalID && stw.Withdrawal_StatusID != 3);
                if (storeWitdrawal != null)
                {
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    var filename = "eSMP" + storeWitdrawal.Store_WithdrawalID + myuuidAsString;
                    string path = _fileReposity.UploadFile(request.File, filename).Result;

                    storeWitdrawal.Withdrawal_StatusID = 4;
                    Image image = new Image();
                    image.IsActive = true;
                    image.FileName = filename;
                    image.Path = path;
                    image.Crete_date = GetVnTime();

                    storeWitdrawal.Image = image;

                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.TotalPage = numpage;
                    result.Data = storeWitdrawal;
                    return result;
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Result GetStoreWithdrawal(int? storeID, int? page, int? statusID, DateTime? from, DateTime? to)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.AsQueryable();
                if (storeID.HasValue)
                {
                    storeWitdrawal = storeWitdrawal.Where(sw => sw.StoreID == storeID);
                }
                if (statusID.HasValue)
                {
                    storeWitdrawal = storeWitdrawal.Where(sw => sw.Withdrawal_StatusID == statusID);
                }
                if (from.HasValue)
                {
                    storeWitdrawal = storeWitdrawal.Where(sw => sw.Create_Date>=from);
                }
                if (to.HasValue)
                {
                    storeWitdrawal = storeWitdrawal.Where(sw => sw.Create_Date <= to);
                }
                if (page.HasValue)
                {

                    numpage = (int)Math.Ceiling((double)storeWitdrawal.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    storeWitdrawal = storeWitdrawal.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                List<Store_WithdrawalModel> list = new List<Store_WithdrawalModel>();
                if (storeWitdrawal.Count() > 0)
                {
                    foreach (var item in storeWitdrawal.ToList())
                    {
                        Store_WithdrawalModel model = new Store_WithdrawalModel
                        {
                            Bank = item.BankSupport,
                            NumBankCart = item.NumBankCart,
                            OwnerBankCart = item.OwnerBankCart,
                            Create_Date = item.Create_Date,
                            Image = item.Image,
                            Price = item.Price,
                            Reason = item.Reason,
                            StoreID = item.StoreID,
                            Store_WithdrawalID = item.Store_WithdrawalID,
                            Withdrawal_Status = _statusReposity.GetWithdrawalStatus(item.Withdrawal_StatusID),
                        };
                        list.Add(model);
                    }
                }
                result.Success = true;
                result.Message = "Thành Công";
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

        public Result GetBankSupport()
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                var bankSupport = _context.BankSupports.Where(b => b.IsActive);
                List<BankSupport> list = new List<BankSupport>();
                if (bankSupport.Count() > 0)
                {
                    foreach (var item in bankSupport.ToList())
                    {

                        list.Add(item);
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

        public Result GetStoreReveneuForChart(int storeID, int? year)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                List<ChartStoreModel> list = new List<ChartStoreModel>();
                if (year.HasValue)
                {
                    for (int i = 1; i < 13; i++)
                    {
                        var sumreveneu = _context.OrderStore_Transactions.Where(ost => ost.StoreID == storeID && ost.Create_Date.Year == year.Value && ost.Create_Date.Month == i && ost.IsActive).Sum(ost => ost.Price);
                        ChartStoreModel model = new ChartStoreModel
                        {
                            amount = sumreveneu,
                            time = i,
                        };
                        list.Add(model);
                    }
                }
                else
                {
                    var yearStart = _context.OrderStore_Transactions.OrderByDescending(ost => ost.Create_Date.Year).First().Create_Date.Year;
                    var yearCrrent = GetVnTime().Year;
                    for (int i = yearStart; i < yearCrrent + 1; i++)
                    {
                        var sumreveneu = _context.OrderStore_Transactions.Where(ost => ost.StoreID == storeID && ost.Create_Date.Year == i && ost.IsActive).Sum(ost => ost.Price);
                        ChartStoreModel model = new ChartStoreModel
                        {
                            amount = sumreveneu,
                            time = i,
                        };
                        list.Add(model);
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
        public Result GetSystemReveneuForChart(int? year)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                List<ChartModel> list = new List<ChartModel>();
                if (year.HasValue)
                {
                    for (int i = 1; i < 13; i++)
                    {
                        var sumreveneuod = _context.OrderSystem_Transactions.Where(ost => ost.SystemID == 1 && ost.Create_Date.Year == year.Value && ost.Create_Date.Month == i && ost.IsActive).Sum(ost => ost.Price);
                        var sumreveneust = _context.Stores.Where(ost => ost.Actice_Date.Value.Year == year.Value && ost.Actice_Date.Value.Month == i && ost.Store_StatusID == 1).Sum(ost => ost.AmountActive);
                        ChartModel model = new ChartModel
                        {
                            amountstore = sumreveneust.Value,
                            amountOrder = sumreveneuod,
                            time = i,
                        };
                        list.Add(model);
                    }
                }
                else
                {
                    var yearStart = _context.Stores.OrderByDescending(ost => ost.Actice_Date.Value.Year).First().Actice_Date.Value.Year;
                    var yearCrrent = GetVnTime().Year;
                    for (int i = yearStart; i < yearCrrent + 1; i++)
                    {
                        var sumreveneuod = _context.OrderSystem_Transactions.Where(ost => ost.SystemID == 1 && ost.Create_Date.Year == i && ost.IsActive).Sum(ost => ost.Price);
                        var sumreveneust = _context.Stores.Where(ost => ost.Actice_Date.Value.Year == i && ost.Store_StatusID == 1).Sum(ost => ost.AmountActive);
                        ChartModel model = new ChartModel
                        {
                            amountstore = sumreveneust.Value,
                            amountOrder = sumreveneuod,
                            time = i,
                        };
                        list.Add(model);
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

        public Result GetStoreSystemReveneu(int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listReveneu = _context.Stores.AsQueryable();
                listReveneu = listReveneu.Where(ot => ot.Store_StatusID == 1);
                if (From.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Actice_Date.Value >= From);
                }
                if (To.HasValue)
                {
                    listReveneu = listReveneu.Where(ost => ost.Actice_Date.Value <= To);
                }
                listReveneu = listReveneu.OrderByDescending(ost => ost.Actice_Date.Value);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listReveneu.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listReveneu = listReveneu.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                List<StoreSystemReveneuView> list = new List<StoreSystemReveneuView>();
                if (listReveneu.Count() > 0)
                {
                    foreach (var item in listReveneu.ToList())
                    {
                        StoreSystemReveneuView model = new StoreSystemReveneuView
                        {
                            StoreName=item.StoreName,
                            ActiveDate = item.Actice_Date.Value,
                            Amount = item.AmountActive.Value,
                            MomoTransaction = item.MomoTransactionID.Value,
                            StoreID = item.StoreID,
                        };
                        list.Add(model);
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

        public Result UpdateCommission_Precent(double commission_Precent)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var eSMP = _context.eSMP_Systems.SingleOrDefault(es => es.SystemID == 1);
                eSMP.Commission_Precent = commission_Precent;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = eSMP;
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

        public Result UpdateAmountActive(double amount_Active)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var eSMP = _context.eSMP_Systems.SingleOrDefault(es => es.SystemID == 1);
                eSMP.AmountActive = amount_Active;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = eSMP;
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

        public Result UpdateRefund_Precent(double refund_Precent)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var eSMP = _context.eSMP_Systems.SingleOrDefault(es => es.SystemID == 1);
                eSMP.Refund_Precent = refund_Precent;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = eSMP;
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
