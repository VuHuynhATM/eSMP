using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Policy;

namespace eSMP.Services
{
    public class ItemRepository : IItemReposity
    {
        private readonly WebContext _context;
        private readonly IStoreReposity _storeReposity;
        private readonly ISpecificationReposity _specificationReposity;
        private readonly IBrandReposity _brandReposity;

        public static int PAGE_SIZE { get; set; } = 5;

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
                newItem.Item_StatusID = 3;

                var listsub = item.List_SubItem;
                foreach (var itemsub in listsub)
                {
                    Sub_Item sub = new Sub_Item();
                    sub.Sub_ItemName = itemsub.Sub_ItemName;
                    sub.Amount = itemsub.Amount;
                    sub.Sub_ItemID = 3;
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
            return _context.Sub_Items.Where(i => i.ItemID == itemID).Min(i => i.Price);
        }

        public double GetMaxPriceForItem(int itemID)
        {
            return _context.Sub_Items.Where(i => i.ItemID == itemID).Max(i => i.Price);
        }

        public Item_Status GetItemStatus(int statusID)
        {
            var result = _context.Item_Statuses.SingleOrDefault(s => s.Item_StatusID == statusID);
            return result;
        }

        public Result GetAllItem(int? statusID, int page)
        {
            Result result = new Result();
            try
            {
                var listItem= _context.Items.AsQueryable();
                if (statusID.HasValue)
                {
                    listItem = listItem.Where(i => i.Item_StatusID == statusID);
                }

                listItem = listItem.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                if (listItem != null)
                {
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
                        SubItem_StatusID = item.SubItem_StatusID,
                        Price = item.Price,
                        Sub_ItemName = item.Sub_ItemName,
                        Image = GetImage(item.ImageID),
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
                        var subimage=_context.Images.SingleOrDefault(i=>i.ImageID==subItem.ItemID);
                        if (subimage != null)
                        {
                            _context.Images.Remove(subimage);
                        }
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

        public Result SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? lat, double? lot, int? storeID, int page)
        {
            Result result = new Result();
            try
            {
                var listItem = _context.Items.AsQueryable();


                //Fillter
                if (!string.IsNullOrEmpty(search))
                {
                    listItem = listItem.Where(i => i.Name.Contains(search));
                }
                else
                {
                    listItem = listItem.Where(i => i.Name.Contains(""));
                }
                if (min.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price) > min);
                }
                if (max.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price) < max);
                }
                if (rate.HasValue)
                {
                    listItem = listItem.Where(i => i.Rate >= rate);
                }
                if (cateID.HasValue)
                {
                    listItem = listItem.Where(i => _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == i.Sub_CategoryID).CategoryID == cateID);
                }
                if (subCateID.HasValue)
                {
                    listItem = listItem.Where(i => _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == i.Sub_CategoryID).Sub_CategoryID == subCateID);
                }
                if (brandID.HasValue)
                {
                    listItem = listItem.Where(i => _context.Model_Items.SingleOrDefault(mi => mi.ItemID == i.ItemID && _context.Brand_Models.SingleOrDefault(bm => bm.Brand_ModelID == mi.Brand_ModelID && bm.BrandID == brandID) != null) != null);
                }
                if (brandModelID.HasValue)
                {
                    listItem = listItem.Where(i => _context.Model_Items.SingleOrDefault(mi => mi.ItemID == i.ItemID && mi.Brand_ModelID == brandModelID) != null);
                }
                if (storeID.HasValue)
                {
                    listItem = listItem.Where(i => i.StoreID == storeID);
                }
                // item active
                listItem = listItem.Where(i => i.Item_StatusID == 1);
                //store active
                listItem = listItem.Where(i =>_context.Stores.SingleOrDefault(s=>s.StoreID==i.StoreID&&s.Store_StatusID==1)!=null);

                //Sort i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Longitude
                
                //listItem = listItem.OrderByDescending(i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID==a.AddressID).Longitude);

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "price_desc":
                            listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            break;
                        case "price_asc":
                            listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            break;
                        case "discount":
                            listItem = listItem.OrderByDescending(i => i.Discount);
                            break;
                    }
                }
                if (lot.HasValue && lat.HasValue)
                {
                    double lo = lot.Value;
                    double la = lat.Value;
                    listItem = listItem.OrderBy(i =>
                                    6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                    * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                    * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                    );
                }

                //Paging

               listItem=listItem.Skip((page-1)* PAGE_SIZE).Take(PAGE_SIZE);

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
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        
        public Result UpdatesubItem(SubItemUpdate subItem)
        {
            Result result = new Result();
            try
            {
                var si = _context.Sub_Items.SingleOrDefault(i => i.Sub_ItemID == subItem.SubItemID);
                if (si != null)
                {
                    si.Amount = subItem.Amount;
                    si.Price = subItem.Price;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Cập nhập thành công";
                    result.Data = si;
                    return result;
                }
                result.Success = false;
                result.Message = "subItemID không hợp lệ";
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

        public Result AddsubItem(int itemID, Sub_ItemRegister subItem)
        {
            Result result=new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                if (item != null)
                {
                    Sub_Item si=new Sub_Item();
                    si.Sub_ItemName = subItem.Sub_ItemName;
                    si.Amount=subItem.Amount;
                    si.Price = subItem.Price;
                    si.SubItem_StatusID = 3;
                    si.ItemID = item.ItemID;
                    _context.Sub_Items.Add(si);
                    _context.SaveChanges();
                    result.Success = false;
                    result.Message = "Thêm thành công";
                    result.Data = si;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không hợp lệ";
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

        public Result ActivesubItem(int subitemID)
        {
            Result result =new Result();
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if(subitem != null)
                {
                    subitem.SubItem_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Active thành công";
                    result.Data = subitem;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result ActiveItem(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    item.Item_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Active thành công";
                    result.Data = item;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result BlocksubItem(int subitemID)
        {
            Result result = new Result();
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    subitem.SubItem_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Khoá thành công";
                    result.Data = subitem;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result BlockItem(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    item.Item_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Khoá thành công";
                    result.Data = item;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result HiddensubItem(int subitemID)
        {
            Result result = new Result();
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    var numactive = _context.Sub_Items.Where(si => si.SubItem_StatusID == 1 && si.ItemID==subitem.ItemID).Count();
                    if(numactive == 1)
                    {
                        result.Success = false;
                        result.Message = "Không thể ẩn sản phẩm cuối cùng";
                        result.Data = "";
                        return result;
                    }
                    if (subitem.SubItem_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "Sản phẩm hiện bị khoá";
                        result.Data = "";
                        return result;
                    }
                    subitem.SubItem_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn thành công";
                    result.Data = subitem;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result HiddenItem(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    if (item.Item_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "Sản phẩm hiện bị khoá";
                        result.Data = "";
                        return result;
                    }
                    item.Item_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn thành công";
                    result.Data = item;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result UnHiddensubItem(int subitemID)
        {
            Result result = new Result();
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    if (subitem.SubItem_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "SubItem hiện bị khoá";
                        result.Data = "";
                        return result;
                    }
                    subitem.SubItem_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn thành công";
                    result.Data = subitem;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result UnHiddenItem(int itemID)
        {
            Result result = new Result();
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    if (item.Item_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "SubItem hiện bị khoá";
                        result.Data = "";
                        return result;
                    }
                    item.Item_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn thành công";
                    result.Data = item;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result SearchItemForSupplier(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? lat, double? lot, int? storeID, int page)
        {
            Result result = new Result();
            try
            {
                var listItem = _context.Items.AsQueryable();


                //Fillter
                if (!string.IsNullOrEmpty(search))
                {
                    listItem = listItem.Where(i => i.Name.Contains(search));
                }
                else
                {
                    listItem = listItem.Where(i => i.Name.Contains(""));
                }
                if (min.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price) > min);
                }
                if (max.HasValue)
                {
                    listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price) < max);
                }
                if (rate.HasValue)
                {
                    listItem = listItem.Where(i => i.Rate >= rate);
                }
                if (cateID.HasValue)
                {
                    listItem = listItem.Where(i => _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == i.Sub_CategoryID).CategoryID == cateID);
                }
                if (subCateID.HasValue)
                {
                    listItem = listItem.Where(i => _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == i.Sub_CategoryID).Sub_CategoryID == subCateID);
                }
                if (brandID.HasValue)
                {
                    listItem = listItem.Where(i => _context.Model_Items.SingleOrDefault(mi => mi.ItemID == i.ItemID && _context.Brand_Models.SingleOrDefault(bm => bm.Brand_ModelID == mi.Brand_ModelID && bm.BrandID == brandID) != null) != null);
                }
                if (brandModelID.HasValue)
                {
                    listItem = listItem.Where(i => _context.Model_Items.SingleOrDefault(mi => mi.ItemID == i.ItemID && mi.Brand_ModelID == brandModelID) != null);
                }
                if (storeID.HasValue)
                {
                    listItem = listItem.Where(i => i.StoreID == storeID);
                }
                // item khong bi block
                listItem = listItem.Where(i => i.Item_StatusID != 2);
                //store active

                //Sort i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Longitude

                //listItem = listItem.OrderByDescending(i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID==a.AddressID).Longitude);

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "price_desc":
                            listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            break;
                        case "price_asc":
                            listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            break;
                        case "discount":
                            listItem = listItem.OrderByDescending(i => i.Discount);
                            break;
                    }
                }
                if (lot.HasValue && lat.HasValue)
                {
                    double lo = lot.Value;
                    double la = lat.Value;
                    listItem = listItem.OrderBy(i =>
                                    6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                    * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                    * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                    );
                }

                //Paging

                listItem = listItem.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

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
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
    }
}
