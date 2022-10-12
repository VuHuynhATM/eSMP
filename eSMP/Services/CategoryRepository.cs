using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.X86;

namespace eSMP.Services
{
    public class CategoryRepository : ICategoryReposity
    {
        private readonly WebContext _context;

        public CategoryRepository(WebContext context)
        {
            _context = context;
        }
        public List<Sub_CategoryModel> GetSub_Categories(int categoryID)
        {
            try
            {
                List<Sub_Category> sub_categories = _context.SubCategories.Where(c => c.CategoryID == categoryID && c.IsActive).ToList();
                List<Sub_CategoryModel> list = new List<Sub_CategoryModel>();
                if (sub_categories.Count > 0)
                {
                    foreach (Sub_Category sub_category in sub_categories)
                    {
                        Sub_CategoryModel mode = new Sub_CategoryModel
                        {
                            CategoryID = sub_category.CategoryID,
                            Sub_CategoryID = sub_category.Sub_CategoryID,
                            Sub_categoryName = sub_category.Sub_categoryName,
                            IsActive=sub_category.IsActive,
                        };
                        list.Add(mode);
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
        public Result GetAllCategory()
        {
            Result result = new Result();
            try
            {
                var list = _context.Categorys.Where(c=>c.IsActive).ToList();
                var r = new List<CategoryModel>();
                if (list.Count > 0)
                {
                    foreach (var c in list)
                    {
                        CategoryModel model = new CategoryModel
                        {
                            CategoryID = c.CategoryID,
                            Name = c.Name,
                            IsActive = c.IsActive,
                            listSub = GetSub_Categories(c.CategoryID),
                        };
                        r.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = r;
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = r;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi Hệ thống";
                result.Data = "";
                return result;
            }
        }
        public Boolean CheckCategory(string name)
        {
            var category = _context.Categorys.SingleOrDefault(c => c.Name == name);
            if (category == null)
                return false;
            return true;
        }
        public Boolean CheckSubCategory(string name)
        {
            var category = _context.SubCategories.SingleOrDefault(c => c.Sub_categoryName == name);
            if (category == null)
                return false;
            return true;
        }
        public Boolean CategoryisExist(int CategoryID)
        {
            var category = _context.Categorys.SingleOrDefault(c => c.CategoryID == CategoryID);
            if (category == null)
                return false;
            return true;
        }
        public Result GetSubCategory(int categoryID)
        {
            Result result = new Result();
            try
            {
                if (!CategoryisExist(categoryID))
                {
                    result.Success = false;
                    result.Message = "Category không tồn tại";
                    result.Data = "";
                    return result;

                }
                var sub = _context.SubCategories.FirstOrDefault(s=>s.CategoryID==categoryID);
                var r = new List<Sub_CategoryModel>();
                if (sub!=null)
                {
                    r = GetSub_Categories(categoryID);
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = r;
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = r;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi Hệ thống";
                result.Data = "";
                return result;
            }
        }

    }
}
