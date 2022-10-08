using eSMP.Models;
using eSMP.VModels;
using System.Xml.Linq;

namespace eSMP.Services
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
                var listBrand = _context.Brands.ToList();
                result.Success = true;
                result.Message = "Thành công";
                result.Data = listBrand;
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
                        if (list.Count == 0)
                        {
                            BrandModel model = new BrandModel();
                            model.BrandID = item.Brand_Model.Brand.BrandID;
                            model.Name = item.Brand_Model.Brand.Name;
                            List<BrandItemModel> listmodel = new List<BrandItemModel>();
                            BrandItemModel brand_Model = new BrandItemModel();
                            brand_Model.Brand_ModelID = item.Brand_Model.BrandID;
                            brand_Model.Name = item.Brand_Model.Name;
                            brand_Model.IsActive = item.Brand_Model.IsActive;
                            listmodel.Add(brand_Model);
                            model.listModel = listmodel;
                            list.Add(model);
                        }
                        else
                        {
                            BrandModel model = new BrandModel();
                            model.BrandID = item.Brand_Model.Brand.BrandID;
                            model.Name = item.Brand_Model.Brand.Name;
                            foreach (var i in list)
                            {
                                Boolean checkinList = false;
                                if (i.BrandID == model.BrandID)
                                {
                                    checkinList = true;
                                    List<BrandItemModel> listmodel = model.listModel;
                                    BrandItemModel brand_Model = new BrandItemModel();
                                    brand_Model.Brand_ModelID = item.Brand_Model.BrandID;
                                    brand_Model.Name = item.Brand_Model.Name;
                                    brand_Model.IsActive = item.Brand_Model.IsActive;
                                    listmodel.Add(brand_Model);
                                    model.listModel = listmodel;
                                    list.Add(model);
                                }
                                if (!checkinList)
                                {
                                    List<BrandItemModel> listmodel = new List<BrandItemModel>();
                                    BrandItemModel brand_Model = new BrandItemModel();
                                    brand_Model.Brand_ModelID = item.Brand_Model.BrandID;
                                    brand_Model.Name = item.Brand_Model.Name;
                                    brand_Model.IsActive = item.Brand_Model.IsActive;
                                    listmodel.Add(brand_Model);
                                    model.listModel = listmodel;
                                    list.Add(model);
                                }
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
                result.Data ="";
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
