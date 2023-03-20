using Castle.Core.Internal;
using eSMP.Models;
using eSMP.Services.BrandRepo;
using eSMP.Services.FileRepo;
using eSMP.Services.SpecificationRepo;
using eSMP.Services.StatusRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eSMP.Services.ItemRepo
{
    public class ItemRepository : IItemReposity
    {
        private readonly WebContext _context;
        private readonly IStoreReposity _storeReposity;
        private readonly ISpecificationReposity _specificationReposity;
        private readonly IUserReposity _userReposity;
        private readonly IBrandReposity _brandReposity;
        private readonly IFileReposity _fileReposity;
        private readonly IStatusReposity _statusReposity;

        public static int PAGE_SIZE { get; set; } = 12;

        public ItemRepository(WebContext context, IStoreReposity storeReposity, ISpecificationReposity specificationReposity, IUserReposity userReposity, IBrandReposity brandReposity, IFileReposity fileReposity, IStatusReposity statusReposity)
        {
            _context = context;
            _storeReposity = storeReposity;
            _specificationReposity = specificationReposity;
            _userReposity = userReposity;
            _brandReposity = brandReposity;
            _fileReposity = fileReposity;
            _statusReposity = statusReposity;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }

        public Result CreateItem(ItemRegister item)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                Item newItem = new Item();
                newItem.Name = item.Name;
                newItem.Description = item.Description;
                newItem.Create_date = GetVnTime();
                newItem.StoreID = item.StoreID;
                newItem.Sub_CategoryID = item.Sub_CategoryID;
                newItem.Rate = 0;
                newItem.Item_StatusID = 3;

                //var listsub = JsonConvert.DeserializeObject<ItemRegister_Sub>(item.List_SubItem);
                IEnumerable<ItemRegister_Sub> listsub = JsonConvert.DeserializeObject<IEnumerable<ItemRegister_Sub>>(item.List_SubItem);
                IEnumerable<SpecificationTagRegister> listSpec = JsonConvert.DeserializeObject<IEnumerable<SpecificationTagRegister>>(item.List_Specitication);
                IEnumerable<int> listModel = JsonConvert.DeserializeObject<IEnumerable<int>>(item.ListModel);
                int index = 0;
                var listSubImage = item.List_SubItem_Image;
                if (listSubImage.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "Hình ảnh các loại sản phẩm trống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                if (listsub.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "Các loại sản phẩm trống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                if (listsub.Count() != listSubImage.Count())
                {
                    result.Success = false;
                    result.Message = "Hình ảnh của loại sản phẩm không đủ";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                foreach (var itemsub in listsub)
                {
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    var filename = "eSMP" + myuuidAsString;
                    string path = _fileReposity.UploadFile(listSubImage[index], filename).Result;

                    Sub_Item sub = new Sub_Item();
                    sub.Sub_ItemName = itemsub.sub_ItemName;
                    sub.Amount = itemsub.amount;
                    sub.SubItem_StatusID = 3;
                    sub.Price = itemsub.price;
                    sub.Item = newItem;
                    sub.WarrantiesTime = itemsub.warrantiesTime;
                    sub.Discount = itemsub.discount;
                    sub.ReturnAndExchange = itemsub.returnAndExchange;

                    Image i = new Image();
                    i.Crete_date = GetVnTime();
                    i.FileName = filename;
                    i.Path = path;
                    i.IsActive = true;

                    sub.Image = i;
                    index++;
                    _context.Sub_Items.Add(sub);
                }

                var listImage = item.List_Image;
                if (listImage.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "Hình ảnh sản phẩm trống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                foreach (var image in listImage)
                {
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    var filename = "eSMP" + myuuidAsString;
                    string path = _fileReposity.UploadFile(image, filename).Result;

                    Image i = new Image();
                    i.Crete_date = GetVnTime();
                    i.FileName = filename;
                    i.Path = path;
                    i.IsActive = true;

                    Item_Image item_Image = new Item_Image();
                    item_Image.Image = i;
                    item_Image.Item = newItem;

                    _context.Item_Images.Add(item_Image);
                }
                if (listSpec.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "thông số sản phẩm trống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                var numspec = _context.Specification.Where(s => _context.SubCate_Specifications.SingleOrDefault(ss => ss.IsActive && ss.Sub_CategoryID == item.Sub_CategoryID && ss.SpecificationID == s.SpecificationID && s.IsSystem) != null && s.IsActive).Count();
                if(numspec != listSpec.Count())
                {
                    result.Success = false;
                    result.Message = "thông số sản phẩm không đủ";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                foreach (var specitication in listSpec)
                {
                    Specification_Value specification_Value = new Specification_Value();
                    specification_Value.SpecificationID = specitication.SpecificationID;
                    specification_Value.Value = specitication.Value;
                    specification_Value.Item = newItem;
                    specification_Value.IsActive = true;
                    _context.Specification_Values.Add(specification_Value);
                }
                if (!item.List_SpecificationCustom.IsNullOrEmpty())
                {
                    IEnumerable<NewSpecificationItem> listSpecCus = JsonConvert.DeserializeObject<IEnumerable<NewSpecificationItem>>(item.List_SpecificationCustom);
                    if (!listSpecCus.IsNullOrEmpty())
                    {
                        foreach (var newSpec in listSpecCus)
                        {
                            Specification specification = new Specification();
                            specification.SpecificationName = newSpec.specificationName;
                            specification.IsActive = true;
                            specification.IsSystem = false;

                            Specification_Value specValue = new Specification_Value();
                            specValue.Specification = specification;
                            specValue.Value = newSpec.specificationValue;
                            specValue.IsActive = true;
                            specValue.Item = newItem;
                            _context.Specification_Values.Add(specValue);
                        }
                    }
                }
                if (listModel.IsNullOrEmpty())
                {
                    result.Success = false;
                    result.Message = "Danh sách phương tiện sản phẩm trống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
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
                result.Data = newItem.ItemID;
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


        public Sub_Item GetMinPriceForItem(int itemID)
        {
            return _context.Sub_Items.OrderBy(i => i.Price).FirstOrDefault(i => i.ItemID == itemID);
        }

        public Sub_Item GetMaxPriceForItem(int itemID)
        {
            return _context.Sub_Items.OrderByDescending(i => i.Price).FirstOrDefault(i => i.ItemID == itemID);
        }


        public Result GetItemWithStatusID(int? statusID, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listItem = _context.Items.AsQueryable();
                if (statusID.HasValue)
                {
                    listItem = listItem.Where(i => i.Item_StatusID == statusID);
                }
                //sort

                listItem = listItem.OrderBy(i => i.Create_date);

                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listItem.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listItem = listItem.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                if (listItem != null)
                {
                    List<ItemViewModel> listmodel = new List<ItemViewModel>();
                    if (listItem.Count() > 0)
                    {
                        foreach (var item in listItem.ToList())
                        {
                            Sub_Item sub = GetMinPriceForItem(item.ItemID);
                            ItemViewModel model = new ItemViewModel
                            {
                                ItemID = item.ItemID,
                                Description = item.Description,
                                Rate = item.Rate,
                                Item_Image = GetItemImage(item.ItemID)[0].Path,
                                Name = item.Name,
                                Price = sub.Price,
                                Discount= sub.Discount,
                                Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                                Num_Sold = GetNumSold(item.ItemID),
                            };
                            listmodel.Add(model);
                        }
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = listmodel;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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

        public Result GetItemWithStatusIDS(int storeID, int? statusID, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listItem = _context.Items.AsQueryable();
                listItem = listItem.Where(i => i.StoreID == storeID);
                if (statusID.HasValue)
                {
                    listItem = listItem.Where(i => i.Item_StatusID == statusID);
                }
                //sort

                listItem = listItem.OrderBy(i => i.Create_date);

                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listItem.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listItem = listItem.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                if (listItem != null)
                {
                    List<ItemViewModel> listmodel = new List<ItemViewModel>();
                    foreach (var item in listItem.ToList())
                    {
                        Sub_Item sub = GetMinPriceForItem(item.ItemID);
                        ItemViewModel model = new ItemViewModel
                        {
                            ItemID = item.ItemID,
                            Description = item.Description,
                            Rate = item.Rate,
                            Item_Image = GetItemImage(item.ItemID)[0].Path,
                            Name = item.Name,
                            Price = sub.Price,
                            Discount = sub.Discount,
                            Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                            Num_Sold = GetNumSold(item.ItemID),
                        };
                        listmodel.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = listmodel;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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
                        SubItem_Status = _statusReposity.GetSubItemStatus(item.SubItem_StatusID),
                        Price = item.Price,
                        Sub_ItemName = item.Sub_ItemName,
                        Image = item.Image,
                        WarrantiesTime = item.WarrantiesTime,
                        Discount = item.Discount,
                        ReturnAndExchange = item.ReturnAndExchange,
                        StatusText = item.StatusText,
                    };
                    list.Add(model);
                }
                return list;
            }
            return null;
        }

        public List<Image> GetListImageFB(int orderdetailID)
        {
            var listIF = _context.Feedback_Images.Where(fi => fi.OrderDetailID == orderdetailID).ToList();
            var list = new List<Image>();
            if (listIF.Count > 0)
            {
                foreach (var image in listIF)
                {
                    list.Add(image.Image);
                }
                return list;
            }
            return null;
        }
        public List<FeedBackModel> GetListFeedBack(int itemID)
        {
            var listorderdetail = _context.OrderDetails.Where(od => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID).ItemID == itemID).Take(PAGE_SIZE).ToList();
            var list = new List<FeedBackModel>();
            if (listorderdetail.Count > 0)
            {
                foreach (var item in listorderdetail)
                {
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == item.OrderID);
                    var user = _userReposity.GetUserIFByID(order.UserID);
                    FeedBackModel model = new FeedBackModel();
                    model.UserName = user.UserName;
                    model.UserID = user.UserID;
                    model.UserAvatar = user.Image.Path;
                    model.orderDetaiID = item.OrderDetailID;
                    model.Create_Date = item.FeedBack_Date;
                    model.Sub_itemName = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID).Sub_ItemName;
                    if (item.FeedBack_Date.HasValue)
                    {
                        model.Rate = item.Feedback_Rate;
                        model.Comment = item.Feedback_Title;
                        model.ImagesFB = GetListImageFB(item.OrderDetailID);
                        list.Add(model);
                    }
                }
            }
            return list;
        }
        public int GetTotalFeedBack(int itemID)
        {
            var listorderdetail = _context.OrderDetails.Where(od => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID).ItemID == itemID && od.Feedback_StatusID != null);
            return listorderdetail.Count();
        }

        public Result GetItemDetail(int itemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                if (item != null)
                {
                    Sub_Item submin = GetMinPriceForItem(itemID);
                    Sub_Item submax = GetMaxPriceForItem(itemID);
                    ItemModel model = new ItemModel
                    {
                        ItemID = item.ItemID,
                        Name = item.Name,
                        Description = item.Description,
                        Create_date = item.Create_date,
                        MaxPrice = submin.Price,
                        MinPrice = submax.Price,
                        Discount = submin.Discount,
                        Rate = item.Rate,
                        Sub_Category = item.Sub_Category.Sub_categoryName,
                        Store = _storeReposity.GetStoreModel(item.StoreID),
                        Specification_Tag = _specificationReposity.GetSpecificationsForItem(item.ItemID),
                        List_Image = GetItemImage(item.ItemID),
                        Item_Status = _statusReposity.GetItemStatus(item.Item_StatusID),
                        ListSubItem = GetListSubItem(item.ItemID),
                        ListModel = _brandReposity.GetModelForItem(item.ItemID),
                        Num_Sold = GetNumSold(item.ItemID),
                        ListFeedBack = GetListFeedBack(itemID),
                        Num_Feedback = GetTotalFeedBack(itemID),
                        WarrantiesTime = submin.WarrantiesTime,
                        StatusText = item.StatusText,
                    };

                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
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

        public Result RemoveItem(int itemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                if (item != null)
                {
                    var listSub = _context.Sub_Items.Where(i => i.ItemID == itemID).ToList();
                    foreach (var subItem in listSub)
                    {
                        var subimage = _context.Images.SingleOrDefault(i => i.ImageID == subItem.ItemID);
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
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa tồn tại item";
                result.Data = "";
                result.TotalPage = numpage;
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

        public Result SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? lat, double? lot, int? storeID, int? page)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listItem = _context.Items.AsQueryable();


                //Fillter
                if (!string.IsNullOrEmpty(search))
                {
                    listItem = listItem.Where(i => EF.Functions.Collate(i.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search));
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
                listItem = listItem.Where(i => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID && s.Store_StatusID == 1) != null);
                //item count>0
                listItem = listItem.Where(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Sum(si => si.Amount) > 0);

                //Sort i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Longitude

                //listItem = listItem.OrderByDescending(i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID==a.AddressID).Longitude);

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "price_desc":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            }
                            break;
                        case "price_asc":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));

                            }
                            break;
                        case "discount":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si=>si.ItemID==i.ItemID).Min(si=>si.Discount)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Discount));

                            }
                            break;
                    }
                }
                else
                {
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
                }

                //Paging
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listItem.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listItem = listItem.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                }

                List<ItemViewModel> listmodel = new List<ItemViewModel>();
                foreach (var item in listItem.ToList())
                {
                    Sub_Item sub = GetMinPriceForItem(item.ItemID);
                    ItemViewModel model = new ItemViewModel
                    {
                        ItemID = item.ItemID,
                        Description = item.Description,
                        Rate = item.Rate,
                        Item_Image = GetItemImage(item.ItemID)[0].Path,
                        Name = item.Name,
                        Price = sub.Price,
                        Discount = sub.Discount,
                        Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                        Num_Sold = GetNumSold(item.ItemID),
                    };
                    listmodel.Add(model);
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = listmodel;
                result.TotalPage = numpage;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = ex;
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result UpdatesubItem(SubItemUpdate subItem)
        {
            Result result = new Result();
            int numpage = 1;
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
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "subItemID không hợp lệ";
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

        public Result AddsubItem(Sub_ItemRegister subItem)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = _context.Items.SingleOrDefault(i => i.ItemID == subItem.itemID);
                if (item != null)
                {
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    var filename = "eSMP" + item.ItemID + myuuidAsString;
                    string path = _fileReposity.UploadFile(subItem.File, filename).Result;

                    Image image = new Image();
                    image.Crete_date = GetVnTime();
                    image.FileName = filename;
                    image.Path = path;
                    image.IsActive = true;

                    Sub_Item si = new Sub_Item();
                    si.Sub_ItemName = subItem.Sub_ItemName;
                    si.Amount = subItem.Amount;
                    si.Price = subItem.Price;
                    si.SubItem_StatusID = 3;
                    si.ItemID = item.ItemID;
                    si.Image = image;
                    si.WarrantiesTime = subItem.WarrantiesTime;
                    si.Discount = subItem.Discount;
                    _context.Sub_Items.Add(si);
                    _context.SaveChanges();
                    result.Success = false;
                    result.Message = "Thêm thành công";
                    result.Data = si;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không hợp lệ";
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

        public Result ActivesubItem(int subitemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    subitem.StatusText = "";
                    subitem.SubItem_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Active thành công";
                    result.Data = _statusReposity.GetSubItemStatus(subitem.SubItem_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result ActiveItem(int itemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    item.StatusText = "";
                    item.Item_StatusID = 1;
                    _context.SaveChanges();
                    var listsub = _context.Sub_Items.Where(si => si.ItemID == item.ItemID).ToList();
                    foreach (var subitem in listsub)
                    {
                        var s = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitem.Sub_ItemID && subitem.SubItem_StatusID != 2);
                        if (s != null&& s.SubItem_StatusID==3)
                        {
                            s.SubItem_StatusID = 1;
                            _context.SaveChanges();
                        }
                    }
                    result.Success = true;
                    result.Message = "Active thành công";
                    result.Data = _statusReposity.GetItemStatus(item.Item_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result BlocksubItem(int subitemID, string? statusText)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    subitem.StatusText = statusText;
                    subitem.SubItem_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Khoá thành công";
                    result.Data = _statusReposity.GetSubItemStatus(subitem.SubItem_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result BlockItem(int itemID, string? statusText)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var item = _context.Items.SingleOrDefault(si => si.ItemID == itemID);
                if (item != null)
                {
                    item.StatusText = statusText;
                    item.Item_StatusID = 2;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Khoá thành công";
                    result.Data = _statusReposity.GetItemStatus(item.Item_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result HiddensubItem(int subitemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == subitemID);
                if (subitem != null)
                {
                    var numactive = _context.Sub_Items.Where(si => si.SubItem_StatusID == 1 && si.ItemID == subitem.ItemID).Count();
                    if (numactive == 1)
                    {
                        result.Success = false;
                        result.Message = "Không thể ẩn sản phẩm cuối cùng";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                    if (subitem.SubItem_StatusID == 2)
                    {
                        result.Success = false;
                        result.Message = "Sản phẩm hiện bị khoá";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                    subitem.SubItem_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn thành công";
                    result.Data = _statusReposity.GetSubItemStatus(subitem.SubItem_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result HiddenItem(int itemID)
        {
            Result result = new Result();
            int numpage = 1;
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
                        result.TotalPage = numpage;
                        return result;
                    }
                    item.Item_StatusID = 4;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Ẩn thành công";
                    result.Data = _statusReposity.GetItemStatus(item.Item_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result UnHiddensubItem(int subitemID)
        {
            Result result = new Result();
            int numpage = 1;
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
                        result.TotalPage = numpage;
                        return result;
                    }
                    subitem.SubItem_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn thành công";
                    result.Data = _statusReposity.GetSubItemStatus(subitem.SubItem_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "SubItemID không tồn tại";
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

        public Result UnHiddenItem(int itemID)
        {
            Result result = new Result();
            int numpage = 1;
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
                        result.TotalPage = numpage;
                        return result;
                    }
                    item.Item_StatusID = 1;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Huỷ ẩn thành công";
                    result.Data = _statusReposity.GetItemStatus(item.Item_StatusID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "ItemID không tồn tại";
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

        public Result SearchItemForAdmin(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? lat, double? lot, int? storeID, int? page, int? itemStatusID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listItem = _context.Items.AsQueryable();


                //Fillter
                if (!string.IsNullOrEmpty(search))
                {
                    listItem = listItem.Where(i => EF.Functions.Collate(i.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(search));
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
                if (itemStatusID.HasValue)
                {
                    listItem = listItem.Where(i => i.Item_StatusID == itemStatusID);

                }
                // item khong bi block
               /* if (isSupplier.HasValue)
                {
                    if (isSupplier.Value)
                        listItem = listItem.Where(i => i.Item_StatusID != 2);
                }*/
                //store active

                //Sort i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Longitude

                //listItem = listItem.OrderByDescending(i => _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID==a.AddressID).Longitude);

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "price_desc":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));
                            }
                            break;
                        case "price_asc":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderBy(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Price));

                            }
                            break;
                        case "discount":
                            if (lot.HasValue && lat.HasValue)
                            {
                                double lo = lot.Value;
                                double la = lat.Value;
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Discount)).ThenBy(i =>
                                                6371 * 2 * Math.Atan2(Math.Sqrt(Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID) != null).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)), Math.Sqrt(1 - Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) * Math.Sin((la - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude) * (Math.PI / 180) / 2) + Math.Cos(_context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Latitude * (Math.PI / 180))
                                                * Math.Cos(la * (Math.PI / 180)) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2) * Math.Sin((lo - _context.Addresss.SingleOrDefault(a => _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).AddressID == a.AddressID).Longitude) * (Math.PI / 180) / 2)))
                                );
                            }
                            else
                            {
                                listItem = listItem.OrderByDescending(i => _context.Sub_Items.Where(si => si.ItemID == i.ItemID).Min(si => si.Discount));

                            }
                            break;
                    }
                }
                else
                {
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
                }


                //Paging

                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listItem.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listItem = listItem.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }

                List<ItemViewModel> listmodel = new List<ItemViewModel>();
                foreach (var item in listItem.ToList())
                {
                    Sub_Item sub = GetMinPriceForItem(item.ItemID);
                    ItemViewModel model = new ItemViewModel
                    {
                        ItemID = item.ItemID,
                        Description = item.Description,
                        Rate = item.Rate,
                        Item_Image = GetItemImage(item.ItemID)[0].Path,
                        Name = item.Name,
                        Price = sub.Price,
                        Discount = sub.Discount,
                        Province = _storeReposity.GetAddressByStoreID(item.StoreID).Province,
                        Num_Sold = GetNumSold(item.ItemID),
                        StoreStatusID = item.Store.Store_StatusID,
                    };
                    listmodel.Add(model);
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = listmodel;
                result.TotalPage = numpage;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result UpdateBrandModel(int itemID, int[] brandmodelIDs)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                foreach (var branmodelid in brandmodelIDs)
                {
                    var itemmodel = _context.Model_Items.SingleOrDefault(mi => mi.ItemID == itemID && mi.Brand_ModelID == branmodelid);
                    if (itemmodel != null)
                    {
                        itemmodel.IsActive = true;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var item = _context.Items.SingleOrDefault(i => i.ItemID == itemID);
                        var model = _context.Brand_Models.SingleOrDefault(bm => bm.Brand_ModelID == branmodelid);
                        if (item != null && model != null)
                        {
                            Model_Item model_Item = new Model_Item();
                            model_Item.Brand_ModelID = branmodelid;
                            model_Item.ItemID = itemID;
                            model_Item.IsActive = true;
                            _context.Model_Items.Add(model_Item);
                            _context.SaveChanges();
                        }
                    }
                }
                result.Success = true;
                result.Message = "Thành công";
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

        public Result RemoveBrandModel(int itemID, int[] brandmodelIDs)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                foreach (var branmodelid in brandmodelIDs)
                {
                    var itemmodel = _context.Model_Items.SingleOrDefault(mi => mi.ItemID == itemID && mi.Brand_ModelID == branmodelid);
                    if (itemmodel != null)
                    {
                        itemmodel.IsActive = false;
                        _context.SaveChanges();
                    }
                }
                result.Success = true;
                result.Message = "Thành công";
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

        public Result UpdateDiscount(int Sub_ItemID, double discsount)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subitem = _context.Sub_Items.SingleOrDefault(i => i.Sub_ItemID == Sub_ItemID);
                if (subitem == null)
                {
                    result.Success = false;
                    result.Message = "sản phhẩm không tồn tại";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    subitem.Discount = discsount;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = discsount;
                    result.TotalPage = numpage;
                    return result;
                }
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

        public Result GetListFeedback(int itemID, int? page, int? role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listorderdetail = _context.OrderDetails.Where(od => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == od.Sub_ItemID && itemID==si.ItemID)!=null && od.Feedback_StatusID != null).AsQueryable();
                if (role == 3)
                {
                    listorderdetail = listorderdetail.Where(od=>od.Feedback_StatusID!=2);
                }
                else if(role == 1)
                {
                    
                }
                else
                {
                    listorderdetail = listorderdetail.Where(od => od.Feedback_StatusID == 1);
                }
                listorderdetail = listorderdetail.OrderByDescending(od => od.FeedBack_Date);
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listorderdetail.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listorderdetail = listorderdetail.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                var list = new List<FeedBackModel>();
                if (listorderdetail.Count() > 0)
                {
                    foreach (var item in listorderdetail.ToList())
                    {
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == item.OrderID);
                        var user = _userReposity.GetUserIFByID(order.UserID);
                        FeedBackModel model = new FeedBackModel();
                        model.UserName = user.UserName;
                        model.UserAvatar = user.Image.Path;
                        model.orderDetaiID = item.OrderDetailID;
                        model.Sub_itemName = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID).Sub_ItemName;
                        if (item.FeedBack_Date.HasValue)
                        {
                            model.Rate = item.Feedback_Rate;
                            model.Comment = item.Feedback_Title;
                            model.ImagesFB = GetListImageFB(item.OrderDetailID);
                            model.Create_Date = item.FeedBack_Date;
                            list.Add(model);
                        }
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

        public int GetSupplierIDByItemID(int itemID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => _context.Items.SingleOrDefault(i => i.ItemID == itemID).StoreID == s.StoreID);
                return store.UserID;
            }
            catch
            {
                return -1;
            }
        }

        public int GetSupplierIDBySubItemID(int SubItemID)
        {
            try
            {
                var store = _context.Stores.SingleOrDefault(s => _context.Items.SingleOrDefault(i => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == SubItemID).ItemID == i.ItemID).StoreID == s.StoreID);
                return store.UserID;
            }
            catch
            {
                return -1;
            }
        }
    }
}
