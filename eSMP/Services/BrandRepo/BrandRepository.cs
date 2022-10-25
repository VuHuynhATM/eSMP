using eSMP.Models;
using eSMP.VModels;
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

        public Result GetAllBrand()
        {
            Result result = new Result();
            try
            {
                List<BrandModel> list = new List<BrandModel>();
                var listBrand = _context.Brands.ToList();
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

        public Result GetBrandModel(int BrandID)
        {
            Result result = new Result();
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
                var listModelitem = _context.Model_Items.Where(mi => mi.ItemID == ItemID).ToList();
                List<BrandModel> list = new List<BrandModel>();
                if (listModelitem.Count > 0)
                {
                    foreach (var item in listModelitem)
                    {
                        var brandmodel = GetBrandModelByID(item.Brand_ModelID);
                        var brand = GetBrandByID(brandmodel.BrandID);
                        if (list.Count == 0)
                        {
                            BrandModel model = new BrandModel();
                            model.BrandID = brand.BrandID;
                            model.Name = brand.Name;
                            model.IsActive = brandmodel.IsActive;
                            model.listModel = new List<BrandItemModel>();
                            List<BrandItemModel> listmodel = new List<BrandItemModel>();
                            BrandItemModel brand_Model = new BrandItemModel();
                            brand_Model.Brand_ModelID = item.Brand_ModelID;
                            brand_Model.Name = brandmodel.Name;
                            brand_Model.IsActive = brandmodel.IsActive;
                            listmodel.Add(brand_Model);
                            model.listModel = listmodel;
                            list.Add(model);
                        }
                        else
                        {
                            BrandModel model = new BrandModel();
                            model.BrandID = brand.BrandID;
                            model.Name = brand.Name;
                            bool checkinList = false;
                            foreach (var i in list)
                            {
                                if (i.BrandID == model.BrandID)
                                {
                                    checkinList = true;
                                    List<BrandItemModel> listmodel = i.listModel;
                                    BrandItemModel brand_Model = new BrandItemModel();
                                    brand_Model.Brand_ModelID = item.Brand_ModelID;
                                    brand_Model.Name = brandmodel.Name;
                                    brand_Model.IsActive = brandmodel.IsActive;
                                    listmodel.Add(brand_Model);
                                    model.listModel = listmodel;
                                }
                            }
                            if (!checkinList)
                            {
                                List<BrandItemModel> listmodel = new List<BrandItemModel>();
                                BrandItemModel brand_Model = new BrandItemModel();
                                brand_Model.Brand_ModelID = item.Brand_ModelID;
                                brand_Model.Name = brandmodel.Name;
                                brand_Model.IsActive = brandmodel.IsActive;
                                listmodel.Add(brand_Model);
                                model.listModel = listmodel;
                                list.Add(model);
                            }
                        }
                    }
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
            try
            {

                List<BrandModel> list = GetModelForItem(ItemID);
                if (list != null)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = list;
                    return result;
                }
                result.Success = true;
                result.Message = "Không có dữ liệu";
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
    }
}
