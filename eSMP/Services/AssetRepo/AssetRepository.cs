using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.VModels;
using System.Linq.Expressions;

namespace eSMP.Services.StoreAssetRepo
{
    public class AssetRepository : IAssetReposity
    {
        private readonly WebContext _context;
        private readonly IOrderReposity _orderReposity;
        private readonly int PAGE_SIZE = 25;

        public AssetRepository(WebContext context, IOrderReposity orderReposity)
        {
            _context = context;
            _orderReposity = orderReposity;
        }
        public Store GetStore(int orderID)
        {
            return _context.Stores.SingleOrDefault(s => _context.Orders.SingleOrDefault(o => o.OrderID == orderID && _context.OrderDetails.FirstOrDefault(od => od.OrderID == o.OrderID && _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID).StoreID == s.StoreID) != null) != null) != null);
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
                transaction.Create_Date = DateTime.UtcNow;
                transaction.Price = afterprice;
                transaction.StoreID = store.StoreID;
                transaction.IsActive = true;

                OrderSystem_Transaction system_Transaction = new OrderSystem_Transaction();
                system_Transaction.Create_Date = DateTime.UtcNow;
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
                Image image = new Image();
                image.Crete_date = DateTime.UtcNow;
                image.FileName = request.FileName;
                image.Path = request.FilePath;
                image.IsActive = true;

                System_Withdrawal system_Withdrawal = new System_Withdrawal();
                system_Withdrawal.SystemID = 1;
                system_Withdrawal.Price = request.Price;
                system_Withdrawal.Context = request.Context;
                system_Withdrawal.Create_Date = DateTime.UtcNow;
                system_Withdrawal.IsActive = true;
                system_Withdrawal.Image = image;
                _context.System_Withdrawals.Add(system_Withdrawal);

                var systemeSMP = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);

                if (systemeSMP.Asset < request.Price)
                {
                    result.Success = false;
                    result.Message = "Tài khoản hhienej khhongo đủ số dư";
                    result.Data = "";
                    return result;
                }
                systemeSMP.Asset = systemeSMP.Asset - request.Price;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành công";
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
        public Result GetALlSystemWitdrawl(int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
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
        public OrderStore_Transaction GetOrderStore_Transaction(int orderStore_TransactionID)
        {
            return _context.OrderStore_Transactions.SingleOrDefault(ost => ost.OrderStore_TransactionID == orderStore_TransactionID);
        }
        public Result GetALlReveneu(int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
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
                listReveneu = listReveneu.OrderByDescending(ost => ost.Create_Date);
                if (page.HasValue)
                {
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
                            OrderStore_TransactionID = item.OrderStore_TransactionID,
                            Price = item.Price,
                            orderID = GetOrderStore_Transaction(item.OrderStore_TransactionID).OrderID,
                        };
                        list.Add(model);
                    }
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
        public Result GetSystemInfo()
        {
            Result result = new Result();
            try
            {
                var eSMp = _context.eSMP_Systems.SingleOrDefault(es => es.SystemID == 1);
                result.Success = true;
                result.Message = "Thành công";
                result.Data = eSMp;
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

        public Result GetStoreReveneu(int storeID, int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
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
                listReveneu = listReveneu.OrderByDescending(ost => ost.Create_Date);
                if (page.HasValue)
                {
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
                        };
                        list.Add(model);
                    }
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

        public Result CreateStoreWithdrawal(StoreWithdrawalRequest request)
        {
            Result result = new Result();
            try
            {
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == request.StoreID);
                if ( store!= null)
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
                    withdrawal.Create_Date = DateTime.UtcNow;
                    withdrawal.StoreID = request.StoreID;

                    _context.Store_Withdrawals.Add(withdrawal);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = withdrawal;
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

        public Result ProcessStoreWithdrawal(int storeWithhdrawalID)
        {
            Result result = new Result();
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

        public Result CancelStoreWithdrawal(int storeWithhdrawalID, string reason)
        {
            Result result = new Result();
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.SingleOrDefault(stw => stw.Store_WithdrawalID == storeWithhdrawalID && stw.Store_WithdrawalID==1);
                if (storeWitdrawal != null)
                {
                    storeWitdrawal.Withdrawal_StatusID = 3;
                    storeWitdrawal.Reason = reason;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = storeWitdrawal;
                    return result;
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Result SuccessStoreWithdrawal(StoreWithdrawalSuccessRequest request)
        {
            Result result = new Result();
            try
            {

                var storeWitdrawal = _context.Store_Withdrawals.SingleOrDefault(stw => stw.Store_WithdrawalID == request.Store_WithdrawalID && stw.Store_WithdrawalID != 3);
                if (storeWitdrawal != null)
                {
                    storeWitdrawal.Withdrawal_StatusID = 4;
                    Image image = new Image();
                    image.IsActive = true;
                    image.FileName = request.Filename;
                    image.Path = request.Path;
                    image.Crete_date = DateTime.UtcNow;

                    storeWitdrawal.Image=image;

                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = storeWitdrawal;
                    return result;
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Withdrawal_Status GetWithdrawal_Status(int statusID)
        {
            return _context.Withdrawal_Statuses.SingleOrDefault(ws => ws.Withdrawal_StatusID == statusID);
        }
        public Result GetStoreWithdrawal(int? storeID, int? page, int? statusID)
        {
            Result result = new Result();
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
                if (page.HasValue)
                {
                    storeWitdrawal = storeWitdrawal.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                List<Store_WithdrawalModel> list = new List<Store_WithdrawalModel>();
                if (storeWitdrawal.Count() > 0)
                {
                    foreach(var item in storeWitdrawal.ToList())
                    {
                        Store_WithdrawalModel model = new Store_WithdrawalModel
                        {
                            Bank = item.BankSupport,
                            NumBankCart=item.NumBankCart,
                            OwnerBankCart=item.OwnerBankCart,
                            Create_Date = item.Create_Date,
                            Image = item.Image,
                            Price = item.Price,
                            Reason = item.Reason,
                            StoreID = item.StoreID,
                            Store_WithdrawalID = item.StoreID,
                            Withdrawal_Status = GetWithdrawal_Status(item.Withdrawal_StatusID),
                        };
                        list.Add(model);
                    }
                }
                result.Success = false;
                result.Message = "Yêu cầu rút tiền không tồn tại";
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

        public Result GetBankSupport()
        {
            Result result = new Result();
            try
            {

                var bankSupport = _context.BankSupports.Where(b=>b.IsActive);
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
