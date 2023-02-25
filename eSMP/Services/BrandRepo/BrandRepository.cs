using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;

namespace eSMP.Services.BrandRepo
{
    public class BrandRepository : IBrandReposity
    {
        private readonly WebContext _context;

        public BrandRepository(WebContext context)
        {
            _context = context;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }
        public Result GetAllBrand(string? role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                List<BrandModel> list = new List<BrandModel>();
                var listBrand = _context.Brands.AsQueryable();
                if (string.IsNullOrEmpty(role) || role == "2" || role == "3")
                {
                    listBrand = listBrand.Where(b => b.IsActive);
                }
                foreach (var brand in listBrand.ToList())
                {
                    BrandModel brandModel = new BrandModel
                    {
                        BrandID = brand.BrandID,
                        Name = brand.Name,
                        IsActive = brand.IsActive,
                        listModel = GetBrandModelList(brand.BrandID),
                    };
                    list.Add(brandModel);
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
        public List<BrandItemModel> GetBrandModelList(int BrandID)
        {
            try
            {
                var listBrandModel = _context.Brand_Models.Where(b => b.BrandID == BrandID).ToList();
                List<BrandItemModel> list = new List<BrandItemModel>();
                if (listBrandModel.Count > 0)
                {
                    foreach (var item in listBrandModel)
                    {
                        BrandItemModel itemBrand = new BrandItemModel
                        {
                            Brand_ModelID = item.Brand_ModelID,
                            Name = item.Name,
                            IsActive = item.IsActive,
                        };
                        list.Add(itemBrand);
                    }
                    return list;
                }
                return list;

            }
            catch
            {
                return new List<BrandItemModel>();
            }
        }

        public Result GetBrandModel(int BrandID, string? role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listBrandModel = _context.Brand_Models.AsQueryable();
                listBrandModel = listBrandModel.Where(b => b.BrandID == BrandID);
                if (string.IsNullOrEmpty(role) || role == "2" || role == "3")
                {
                    listBrandModel = listBrandModel.Where(b => b.IsActive);
                }
                List<BrandItemModel> list = new List<BrandItemModel>();
                if (listBrandModel.Count() > 0)
                {
                    foreach (var item in listBrandModel.ToList())
                    {
                        BrandItemModel itemBrand = new BrandItemModel
                        {
                            Brand_ModelID = item.Brand_ModelID,
                            Name = item.Name,
                            IsActive = item.IsActive,
                        };
                        list.Add(itemBrand);
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

        public Brand_Model GetBrandModelByID(int BrandModelID)
        {
            try
            {
                return _context.Brand_Models.SingleOrDefault(e => e.Brand_ModelID == BrandModelID);
            }
            catch
            {
                return null;
            }
        }
        public Brand GetBrandByID(int BrandID)
        {
            try
            {
                return _context.Brands.SingleOrDefault(e => e.BrandID == BrandID);
            }
            catch
            {
                return null;
            }
        }

        public List<BrandModel> GetModelForItem(int ItemID)
        {
            try
            {
                List<BrandModel> list = new List<BrandModel>();
                var listModelitem = _context.Model_Items.Where(mi => mi.ItemID == ItemID).ToList();
                var listBrand = _context.Brands.Where(b => _context.Brand_Models.FirstOrDefault(bm => bm.BrandID == b.BrandID && _context.Model_Items.FirstOrDefault(mi => mi.Brand_ModelID == bm.Brand_ModelID && mi.ItemID == ItemID) != null) != null).ToList();
                foreach (var brand in listBrand)
                {
                    BrandModel brandModel = new BrandModel
                    {
                        BrandID = brand.BrandID,
                        Name = brand.Name,
                        IsActive = brand.IsActive,
                        listModel = GetBrandModelList(brand.BrandID),
                    };
                    list.Add(brandModel);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public Result GetBrandModelForItem(int ItemID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {

                List<BrandModel> list = GetModelForItem(ItemID);
                if (list != null)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Không có dữ liệu";
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

        public Result CreateBrand(string brand_Name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var brandcheck = _context.Brands.SingleOrDefault(b => b.Name == brand_Name);
                if (brandcheck == null)
                {
                    Brand brand = new Brand();
                    brand.Name = brand_Name;
                    brand.IsActive = true;
                    _context.Brands.AddAsync(brand);
                    _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = brand;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Đã tồn tại hãng xe";
                    result.Data = "";
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

        public Result CreateMotorcycle(int brandID, string moto_Name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var motor = _context.Brand_Models.SingleOrDefault(m => m.BrandID == brandID && m.Name == moto_Name);
                if(motor == null)
                {
                    Brand_Model moto = new Brand_Model();
                    moto.Name = moto_Name;
                    moto.BrandID = brandID;
                    moto.IsActive = true;
                    _context.Brand_Models.AddAsync(moto);
                    _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = moto;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Đã tồn tại phương tiện";
                    result.Data = "";
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

        public Result RemoveMotorcycle(int motorcycleID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var moto =_context.Brand_Models.SingleOrDefault(m=>m.Brand_ModelID == motorcycleID);
                if (moto != null)
                {
                    moto.IsActive = false;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = moto;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Tên phương tiện không tồn tại";
                    result.Data = "";
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

        public Result RemoveBrand(int brandID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var brand = _context.Brands.SingleOrDefault(b => b.BrandID == brandID);
                if (brand != null)
                {
                    brand.IsActive = false;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = brand;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Hãng phương tiện không tồn tại";
                    result.Data = "";
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

        public Result ActiveMotorcycle(int motorcycleID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var moto = _context.Brand_Models.SingleOrDefault(m => m.Brand_ModelID == motorcycleID);
                if (moto != null)
                {
                    moto.IsActive = true;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = moto;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Tên phương tiện không tồn tại";
                    result.Data = "";
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

        public Result ActiveBrand(int brandID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var brand = _context.Brands.SingleOrDefault(b => b.BrandID == brandID);
                if (brand != null)
                {
                    brand.IsActive = true;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = brand;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Hãng phương tiện không tồn tại";
                    result.Data = "";
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
    }
}
