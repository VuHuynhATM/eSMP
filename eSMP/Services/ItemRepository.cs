using eSMP.Models;
using eSMP.VModels;
using System.Security.Policy;

namespace eSMP.Services
{
    public class ItemRepository : IItemReposity
    {
        private readonly WebContext _context;
        private readonly IStoreReposity _storeReposity;
        private readonly ISpecificationReposity _specificationReposity;
        private readonly IBrandReposity _brandReposity;

        public ItemRepository(WebContext context, IStoreReposity storeReposity, ISpecificationReposity specificationReposity, IBrandReposity brandReposity)
        {
            _context = context;
            _storeReposity = storeReposity;
            _specificationReposity = specificationReposity;
            _brandReposity = brandReposity;
        }

        public Result CreateItem(ItemRegister item)
        {
            Result result = new Result();
            try
            {
                Item newItem = new Item();
                newItem.Name = item.Name;
                newItem.Description = item.Description;
                newItem.Create_date = DateTime.UtcNow;
                newItem.StoreID = item.StoreID;
                newItem.Sub_CategoryID = item.Sub_CategoryID;
                newItem.Rate = 0;
                newItem.Discount = item.Discount;
                newItem.Item_StatusID = 1;

                var listsub = item.List_SubItem;
                foreach (var itemsub in listsub)
                {
                    Sub_Item sub = new Sub_Item();
                    sub.Sub_ItemName = itemsub.Sub_ItemName;
                    sub.Amount = itemsub.Amount;
                    sub.IsActive = true;
                    sub.Price = itemsub.Price;
                    sub.Item = newItem;
                    _context.Sub_Items.Add(sub);
                }

                var listImage = item.List_Image;
                foreach (var image in listImage)
                {
                    Image i = new Image();
                    i.Crete_date = DateTime.UtcNow;
                    i.FileName = image.FileName;
                    i.Path = image.Path;
                    i.IsActive = true;

                    Item_Image item_Image = new Item_Image();
                    item_Image.Image = i;
                    item_Image.Item = newItem;

                    _context.Item_Images.Add(item_Image);
                }
                var listSpec = item.List_Specitication;
                foreach (var specitication in listSpec)
                {
                    Specification_Value specification_Value = new Specification_Value();
                    specification_Value.SpecificationID = specitication.SpecificationID;
                    specification_Value.Value = specitication.Value;
                    specification_Value.Item = newItem;
                    specification_Value.IsActive = true;
                    _context.Specification_Values.Add(specification_Value);
                }
                var listModel = item.ListModel;
                foreach (var model in listModel)
                {
                    Model_Item model_Item = new Model_Item();
                    model_Item.Item = newItem;
                    model_Item.Brand_ModelID = model;
                    model_Item.IsActive = true;
                    _context.Model_Items.Add(model_Item);
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "thành Công";
                result.Data = GetItemDetail(newItem.ItemID);
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
                        Image r = GetImage(image.ImageID);
                        result.Add(r);
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
        public Image GetImage(int imageID)
        {
            Image image = _context.Images.SingleOrDefault(i => i.ImageID == imageID);
            if (image != null)
            {
                return image;
            }
            return null;
        }
        public double GetMinPriceForItem(int itemID)
        {
            var listsubItem = _context.Sub_Items.Where(i => i.ItemID == itemID).ToList();
            if (listsubItem.Count > 0)
            {
                double price = 0;
                foreach (var item in listsubItem)
                {
                    price = item.Price;
                    if (item.Price < price)
                    {
                        price = item.Price;
                    }
                }
                return price;
            }
            return 0;
        }
        public double GetMaxPriceForItem(int itemID)
        {
            var listsubItem = _context.Sub_Items.Where(i => i.ItemID == itemID).ToList();
            if (listsubItem.Count > 0)
            {
                double price = 0;
                foreach (var item in listsubItem)
                {
                    if (item.Price > price)
                    {
                        price = item.Price;
                    }
                }
                return price;
            }
            return 0;
        }
        public Item_Status GetItemStatus(int statusID)
        {
            var result = _context.Item_Statuses.SingleOrDefault(s => s.Item_StatusID == statusID);
            return result;
        }
        public Result GetAllItem()
        {
            Result result = new Result();
            try
            {
                var listItem = _context.Items.ToList();
                if (listItem != null)
                {
                    List<ItemViewModel> listmodel = new List<ItemViewModel>();
                    foreach (var item in listItem)
                    {
                        ItemViewModel model = new ItemViewModel
                        {
                            ItemID = item.ItemID,
                            Description = item.Description,
                            Rate = item.Rate,
                            Item_Image = GetItemImage(item.ItemID)[0].Path,
                            Name = item.Name,
                            Price = GetMinPriceForItem(item.ItemID),
                            Discount = item.Discount,
                            Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                        };
                        listmodel.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = listmodel;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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
        public int GetTotalAmount(int ItemID)
        {
            int amount = 0;
            var subItems = _context.Sub_Items.Where(x => x.ItemID == ItemID).ToList();
            if (subItems.Count > 0)
            {
                foreach (var item in subItems)
                {
                    amount = amount + item.Amount;
                }
            }
            return amount;
        }
        public List<SubItemModel> GetListSubItem(int itemID)
        {
            var listSub = _context.Sub_Items.Where(s => s.ItemID == itemID).ToList();
            if (listSub.Count > 0)
            {
                List<SubItemModel> list = new List<SubItemModel>();
                foreach (var item in listSub)
                {
                    SubItemModel model = new SubItemModel
                    {
                        Sub_ItemID = item.Sub_ItemID,
                        Amount = item.Amount,
                        IsActive = item.IsActive,
                        Price = item.Price,
                        Sub_ItemName = item.Sub_ItemName,
                    };
                    list.Add(model);
                }
                return list;
            }
            return null;
        }
        public Result GetItemDetail(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                if (item != null)
                {
                    ItemModel model = new ItemModel
                    {
                        ItemID = item.ItemID,
                        Name = item.Name,
                        Description = item.Description,
                        Create_date = item.Create_date,
                        MaxPrice = GetMaxPriceForItem(item.ItemID),
                        MinPrice = GetMinPriceForItem(item.ItemID),
                        Discount = item.Discount,
                        Rate = item.Rate,
                        Sub_CategoryID = item.Sub_CategoryID,
                        Store = _storeReposity.GetStoreModel(item.StoreID),
                        Specification_Tag = _specificationReposity.GetSpecificationsForItem(item.ItemID),
                        List_Image = GetItemImage(item.ItemID),
                        Item_Status = GetItemStatus(item.Item_StatusID),
                        ListSubItem = GetListSubItem(item.ItemID),
                        ListModel = _brandReposity.GetModelForItem(item.ItemID),
                    };

                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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
        public Item GetItem(int ItemID)
        {
            return _context.Items.SingleOrDefault(i => i.ItemID == ItemID);
        }
        public Result RemoveItem(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                if (item != null)
                {
                    var listSub = _context.Sub_Items.Where(i => i.ItemID == itemID).ToList();
                    foreach (var subItem in listSub)
                    {
                        _context.Sub_Items.Remove(subItem);
                    }
                    var listImageItem = GetItemImage(item.ItemID);
                    foreach (var imageItem in listImageItem)
                    {
                        _context.Images.Remove(imageItem);
                    }
                    _context.Items.Remove(item);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Xoá Thành Công";
                    result.Data = "";
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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
        // Tính khổng cách hai điểm khi biết kinh độ, vĩ độ
        public static double distanceBetween2Points(double la1, double lo1, double la2, double lo2)
        {
            double R = 6371;
            double dLat = (la2 - la1) * (Math.PI / 180);
            double dLon = (lo2 - lo1) * (Math.PI / 180);
            double la1ToRad = la1 * (Math.PI / 180);
            double la2ToRad = la2 * (Math.PI / 180);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(la1ToRad)
                        * Math.Cos(la2ToRad) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;
        }
        public Result SearchItem(string? search, double? min, double? max, int? cateID, int? subCateID, int? brandID, int? brandModelID)
        {
            Result result = new Result();
            try
            {
                var listItem = _context.Items.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    listItem = listItem.Where(i=>i.Name.Contains(search));
                }
                else
                {
                    listItem = listItem.Where(i => i.Name.Contains(""));
                }

                if (min.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si=>si.ItemID==i.ItemID).Min(si=>si.Price)>min);
                }

                if (max.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price) < max);
                }

                if (cateID.HasValue)
                {
                    listItem = listItem.Where(i =>_context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == i.Sub_CategoryID).CategoryID==cateID);
                }
                List<ItemViewModel> listmodel = new List<ItemViewModel>();
                foreach (var item in listItem.ToList())
                {
                    ItemViewModel model = new ItemViewModel
                    {
                        ItemID = item.ItemID,
                        Description = item.Description,
                        Rate = item.Rate,
                        Item_Image = GetItemImage(item.ItemID)[0].Path,
                        Name = item.Name,
                        Price = GetMinPriceForItem(item.ItemID),
                        Discount = item.Discount,
                        Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                    };
                    listmodel.Add(model);
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = listmodel;
                return result;
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
    }
}
