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
                var list = _context.Specification.ToList();
                List<Specification> speList = new List<Specification>();
                if (list.Count > 0)
                {
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = list;
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
        public Specification GetSpecificationByID(int specificationID)
        {
            try
            {
                var spec = _context.Specification.SingleOrDefault(s => s.SpecificationID == specificationID);
                if (spec != null)
                {
                    return spec;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public Result GetSpecificationsBySubCate(int subCategoryID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listSpe = _context.SubCate_Specifications.Where(c => c.Sub_CategoryID == subCategoryID).ToList();
                List<Specification> specifications = new List<Specification>();
                if (listSpe.Count > 0)
                {
                    foreach (var item in listSpe)
                    {
                        var spe = GetSpecificationByID(item.SpecificationID);
                        specifications.Add(spe);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = specifications;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Chưa có thông số kĩ thuật";
                result.Data = new List<Specification>();
                result.TotalPage = numpage;
                return result;
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
                var listSpe = _context.Specification_Values.Where(c => c.ItemID == itemID).ToList();
                List<SpecificationTagModel> result = new List<SpecificationTagModel>();
                if (listSpe.Count > 0)
                {
                    foreach (var item in listSpe)
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
                _context.Specification.Add(specification);
                _context.SaveChanges();
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

        public Result AddSpecification(int sub_CategoryID, int[] specificationIDs)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                foreach (int item in specificationIDs)
                {
                    if (CheckSpec(item, sub_CategoryID))
                    {
                        var subCate_spe = _context.SubCate_Specifications.SingleOrDefault(ss => ss.SpecificationID == item && ss.Sub_CategoryID == sub_CategoryID && !ss.IsActive);
                        if (subCate_spe != null)
                        {
                            subCate_spe.IsActive = true;
                        }
                        else
                        {
                            SubCate_Specification specification = new SubCate_Specification();
                            specification.SpecificationID = item;
                            specification.Sub_CategoryID = sub_CategoryID;
                            specification.IsActive = true;
                            _context.SubCate_Specifications.Add(specification);
                        }
                    }
                }
                _context.SaveChanges();
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
                    var spe = _context.Specification.SingleOrDefault(s=>s.SpecificationID==specificationID);
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
    }
}
