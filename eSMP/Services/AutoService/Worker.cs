using eSMP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eSMP.Services.AutoService
{
    public class Worker : IWorker
    {
        private readonly WebContext _context;

        public Worker(WebContext context)
        {
            _context = context;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            //1000*60*15
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000 * 5* 60);

                    var role = _context.Roles.SingleOrDefault(r => r.RoleID == 5);
                    role.RoleName = DateTime.Now.ToString();
                    _context.SaveChanges();
                    ControlOfRevenues();
                    UpdateOrderstatus();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

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
        public void ControlOfRevenues()
        {
            try
            {
                var listShipFinish = _context.ShipOrders.Where(so => so.Status_ID == "6" && so.Order.OrderStatusID == 1).AsQueryable();
                if (listShipFinish.Count() > 0)
                {
                    foreach (var ship in listShipFinish.ToList())
                    {
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == ship.OrderID);
                        order.OrderStatusID = 5;
                        var confirmReponse = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == ship.OrderID);
                        //layas thoi gian giai ngan
                        var maxReturnTime = _context.OrderDetails.Where(od => od.OrderID == order.OrderID).Max(od => od.ReturnAndExchange);
                        DateTime now = GetVnTime();
                        DateTime timeCanReturn = order.Create_Date.AddDays(maxReturnTime);
                        if (confirmReponse.ResultCode == 0 && timeCanReturn <= now)
                        {
                            //ghi nhan doanh thu
                            //store
                            var store = GetStoreByorderID(ship.OrderID.Value);
                            var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                            var orderStore_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderID == order.OrderID);
                            store.Asset = store.Asset + orderStore_Transaction.Price;
                            //sys
                            var orderSystem_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == orderStore_Transaction.OrderStore_TransactionID);
                            system.Asset = system.Asset + orderSystem_Transaction.Price;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public Store GetStoreByorderID(int orderID)
        {
            var subitemID = _context.OrderDetails.FirstOrDefault(od => od.OrderID == orderID).Sub_ItemID;
            return GetStoreBySubItemID(subitemID);
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
        public double GetPriceItemOrder(int orderID)
        {
            try
            {
                double total = 0;
                var listorderdetail = _context.OrderDetails.Where(od => od.OrderID == orderID);

                if (_context.Orders.SingleOrDefault(o => o.OrderID == orderID).OrderStatusID == 1)
                {
                    if (listorderdetail.Count() > 0)
                    {
                        foreach (var item in listorderdetail.ToList())
                        {
                            total = total + item.PricePurchase * item.Amount * (1 - item.DiscountPurchase);
                        }
                    }
                }
                return total;
            }
            catch
            {
                return 0;
            }
        }

        public void UpdateOrderstatus()
        {
            try
            {
                var listorderServicepending = _context.Orders.Where(o => o.OrderStatusID == 6 && _context.ServiceDetails.FirstOrDefault(sd => sd.OrderDetail.OrderID == o.OrderID && sd.AfterBuyService.ServicestatusID == 1|| sd.AfterBuyService.ServicestatusID == 2) == null).AsQueryable();
                foreach (var item in listorderServicepending.ToList())
                {
                    item.OrderStatusID = 1;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
