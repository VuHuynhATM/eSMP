using eSMP.Models;
using eSMP.Services.StoreRepo;
using eSMP.VModels;

namespace eSMP.Services.ReportSaleRepo
{
    public class SaleReportRepository:ISaleReportReposity
    {
        private readonly WebContext _context;
        private readonly IStoreReposity _storeReposity;

        public SaleReportRepository(WebContext context, IStoreReposity storeReposity)
        {
            _context = context;
            _storeReposity = storeReposity;
        }
        public Result GetListHotItem(int? storeID, int? month, int? year, bool hot)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listItem = _context.Items.AsQueryable();
                if (storeID.HasValue)
                {
                    listItem = listItem.Where(i => i.StoreID == storeID);
                }
                if (year.HasValue)
                {
                    listItem = listItem.Where(i => _context.OrderDetails.Where(od => od.Sub_ItemID == _context.Sub_Items.SingleOrDefault(si => si.ItemID == i.ItemID).Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID && _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == o.OrderID && obt.Create_Date.Year == year) != null) != null)!=null);
                    if (month.HasValue)
                    {
                        listItem = listItem.Where(i => _context.OrderDetails.Where(od => od.Sub_ItemID == _context.Sub_Items.SingleOrDefault(si => si.ItemID == i.ItemID).Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID && _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == o.OrderID && obt.Create_Date.Year == year && obt.Create_Date.Month == month) != null) != null) != null);
                    }
                }
                if (hot)
                {
                    listItem = listItem.OrderByDescending(i => _context.OrderDetails.Where(od => od.Sub_ItemID == _context.Sub_Items.SingleOrDefault(si => si.ItemID == i.ItemID).Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID) != null).Sum(od => od.Amount));
                }
                else
                {
                    listItem = listItem.OrderBy(i => _context.OrderDetails.Where(od => od.Sub_ItemID == _context.Sub_Items.SingleOrDefault(si => si.ItemID == i.ItemID).Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID) != null).Sum(od => od.Amount));

                }
                listItem = listItem.Take(30);
                List<ItemViewModel> list = new List<ItemViewModel>();
                if (listItem.Count() > 0)
                {
                    foreach (var item in listItem.ToList())
                    {
                        ItemViewModel model = new ItemViewModel
                        {
                            Description = item.Description,
                            Discount = item.Discount,
                            ItemID = item.ItemID,
                            Item_Image = GetItemImage(item.ItemID)[0].Path,
                            Name = item.Name,
                            Price = GetMinPriceForItem(item.ItemID),
                            Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                            Rate = item.Rate,
                            Num_Sold = GetNumSold(item.ItemID),
                        };
                        list.Add(model);
                    }
                }
                result.Success = true;
                result.Message = "Thhành công";
                result.Data =list;
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hhệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public int GetNumSold(int itemID)
        {
            int result = 0;
            try
            {
                return _context.OrderDetails.Where(od => od.Sub_ItemID == _context.Sub_Items.SingleOrDefault(si => si.ItemID == itemID).Sub_ItemID && _context.Orders.SingleOrDefault(o => o.OrderStatusID == 1 && o.OrderID == od.OrderID) != null).Sum(od => od.Amount);
            }
            catch
            {
                return result;
            }
        }
        public List<Image> GetItemImage(int itemID)
        {
            try
            {
                var images = _context.Item_Images.Where(i => i.ItemID == itemID).ToList();
                if (images.Count > 0)
                {
                    List<Image> result = new List<Image>();
                    foreach (var image in images)
                    {
                        result.Add(image.Image);
                    }
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public double GetMinPriceForItem(int itemID)
        {
            return _context.Sub_Items.Where(i => i.ItemID == itemID).Min(i => i.Price);
        }
    }
}
