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
        private readonly int PAGE_SIZE=25;

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
                if(order == null)
                    return false;
                var store=GetStore(order.OrderID);
                var orderPrice = _orderReposity.GetPriceItemOrder(order.OrderID);
                var orderFee = order.FeeShip;
                var afterprice = orderPrice * (1 - esmpSystem.Commission_Precent) +orderFee;
                OrderStore_Transaction transaction = new OrderStore_Transaction();
                transaction.OrderID = order.OrderID;
                transaction.Create_Date = DateTime.UtcNow;
                transaction.Price=afterprice;
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
        public Image GetImage(int imageID)
        {
            return _context.Images.SingleOrDefault(i => i.ImageID == imageID);
        }
        public Result GetALlSystemWitdrawl(int? page, DateTime? From, DateTime? To)
        {
            Result result = new Result();
            try
            {
                var listWithdrawal=_context.System_Withdrawals.AsQueryable();
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
                    listWithdrawal= listWithdrawal.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                listWithdrawal = listWithdrawal.Where(sw => sw.SystemID == 1);
                List< SystemWithdrawalView > list = new List< SystemWithdrawalView >();
                if (listWithdrawal.Count() > 0)
                {
                    foreach (var item in listWithdrawal.ToList())
                    {
                        SystemWithdrawalView model = new SystemWithdrawalView
                        {
                            System_WithdrawalID = item.System_WithdrawalID,
                            Context = item.Context,
                            Create_Date = item.Create_Date,
                            Image=GetImage(item.ImageID),
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
                            orderID=GetOrderStore_Transaction(item.OrderStore_TransactionID).OrderID,
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
    }
}
