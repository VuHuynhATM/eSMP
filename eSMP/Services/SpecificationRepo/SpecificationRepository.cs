using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.SpecificationRepo
{
    public class SpecificationRepository : ISpecificationReposity
    {
        private readonly WebContext _context;

        public SpecificationRepository(WebContext context)
        {
            _context = context;
        }
        public bool SpecificationIsExist(string name)
        {
            var specification = _context.Specification.SingleOrDefault(s => s.SpecificationName == name);
            if (specification == null)
                return false;
            return true;
        }
        public Result GetAllSpecification()
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var list = _context.Specification.Where(s=>s.IsSystem).ToList();
                List<SpecificationModel> speList = new List<SpecificationModel>();
                if (list.Count > 0)
                {
                    foreach (var spec in list)
                    {
                        SpecificationModel model = new SpecificationModel();
                        model.SpecificationID = spec.SpecificationID;
                        model.SpecificationName = spec.SpecificationName;
                        model.IsActive = spec.IsActive;
                        if (spec.SpecificationSuggests != null)
                        {
                            var listvalue = spec.SpecificationSuggests.Split(";");
                            model.SuggestValues = listvalue;
                        }
                        speList.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = speList;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa có thông số";
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
        public Result GetSpecificationsBySubCate(int subCategoryID, string? role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                CateSpecification_Reponse reponse = new CateSpecification_Reponse();
                if (role == "1")
                {
                    var listIsSpe = _context.Specification.Where(s => _context.SubCate_Specifications.SingleOrDefault(ss => ss.IsActive && ss.Sub_CategoryID == subCategoryID && ss.SpecificationID == s.SpecificationID && s.IsSystem) != null && s.IsActive && s.SpecificationID != 2).ToList();
                    var listSpe = _context.Specification.Where(s => s.IsActive && s.SpecificationID != 2 && s.IsSystem).ToList();
                    reponse.ispecs = listIsSpe;
                    reponse.nonpecs = listSpe;
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = reponse;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    var listSpe = _context.Specification.Where(s => s.IsActive && s.IsSystem).ToList();
                    List<SpecificationModel> list = new List<SpecificationModel>();
                    foreach (var spec in listSpe)
                    {
                        SpecificationModel model = new SpecificationModel();
                        model.SpecificationID = spec.SpecificationID;
                        model.SpecificationName = spec.SpecificationName;
                        model.IsActive = spec.IsActive;
                        if (spec.SpecificationSuggests != null)
                        {
                            var listvalue = spec.SpecificationSuggests.Split(";");
                            model.SuggestValues = listvalue;
                        }
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi Hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public List<SpecificationTagModel> GetSpecificationsForItem(int itemID)
        {
            try
            {
                var listSpe = _context.Specification_Values.Where(c => c.ItemID == itemID && c.Specification.IsActive).ToList();
                List<SpecificationTagModel> result = new List<SpecificationTagModel>();
                if (listSpe.Count > 0)
                {
                    foreach (var item in listSpe)
                    {
                        if (item.IsActive)
                        {
                            SpecificationTagModel tag = new SpecificationTagModel
                            {
                                ItemID = item.ItemID,
                                IsActive = item.IsActive,
                                SpecificationID = item.SpecificationID,
                                Specification_ValueID = item.Specification_ValueID,
                                Value = item.Value,
                                SpecificationName = item.Specification.SpecificationName,
                            };
                            result.Add(tag);
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public Result CreateSpecification(string specification_Name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                Specification specification = new Specification();
                specification.SpecificationName = specification_Name;
                specification.IsActive = true;
                specification.IsSystem = true;
                _context.Specification.AddAsync(specification);
                _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Chưa có thông số";
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

        public Result AddSpecification(CateSpecification_Request request)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                foreach (int item in request.specificationIDsaAdd)
                {
                    var subCate_spe = _context.SubCate_Specifications.SingleOrDefault(ss => ss.SpecificationID == item && ss.Sub_CategoryID == request.sub_CategoryID && !ss.IsActive);
                    if (subCate_spe != null)
                    {
                        subCate_spe.IsActive = true;
                    }
                    else
                    {
                        SubCate_Specification specification = new SubCate_Specification();
                        specification.SpecificationID = item;
                        specification.Sub_CategoryID = request.sub_CategoryID;
                        specification.IsActive = true;
                        _context.SubCate_Specifications.AddAsync(specification);
                    }
                }
                foreach (int itemremove in request.specificationIDsRemove)
                {
                    var subCate_speremove = _context.SubCate_Specifications.SingleOrDefault(ss => ss.SpecificationID == itemremove && ss.Sub_CategoryID == request.sub_CategoryID && ss.IsActive);
                    if (subCate_speremove != null)
                    {
                        subCate_speremove.IsActive = false;
                    }
                }
                _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Thêm thông số thành công";
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
        public Boolean CheckSpec(int specificationID, int sub_CategoryID)
        {
            try
            {
                var spe = _context.Specification.SingleOrDefault(s => s.SpecificationID == specificationID && s.IsActive == true && _context.SubCate_Specifications.SingleOrDefault(ss => ss.SpecificationID == specificationID && ss.Sub_CategoryID == sub_CategoryID && !ss.IsActive) == null);
                if (spe != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Result ReomoveCateSpecification(int sub_CategoryID, int[] specificationIDs)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                foreach (int item in specificationIDs)
                {
                    var subCate_spe = _context.SubCate_Specifications.SingleOrDefault(ss => ss.SpecificationID == item && ss.Sub_CategoryID == sub_CategoryID && ss.IsActive);
                    if (subCate_spe != null)
                    {
                        subCate_spe.IsActive = false;
                    }
                }
                _context.SaveChanges();
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

        public Result ReomoveSpecification(int specificationID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var spe = _context.Specification.SingleOrDefault(s => s.SpecificationID == specificationID);
                if (spe != null)
                {
                    spe.IsActive = false;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
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

        public Result ActiveSpecification(int specificationID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var spe = _context.Specification.SingleOrDefault(s => s.SpecificationID == specificationID);
                if (spe != null)
                {
                    spe.IsActive = true;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
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

        public Result AddSuggesTSpecification(int SpecificationID, string suggestValues)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var specification= _context.Specification.SingleOrDefault(s => s.SpecificationID == SpecificationID);
                    specification.SpecificationSuggests=suggestValues;
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = suggestValues;
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

    }
}
