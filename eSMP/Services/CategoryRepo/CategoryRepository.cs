using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.X86;

namespace eSMP.Services.CategoryRepo
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
                List<Sub_Category> sub_categories = _context.SubCategories.Where(c => c.CategoryID == categoryID).ToList();
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
                            IsActive = sub_category.IsActive,
                        };
                        list.Add(mode);
                    }
                }
                return list;
            }
            catch
            {
                return new List<Sub_CategoryModel>(); ;
            }
        }
        public Result GetAllCategory(string role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var list = _context.Categorys.AsQueryable();
                if (string.IsNullOrEmpty(role)||role=="2"||role=="3")
                {
                    list = list.Where(c => c.IsActive == true);
                }
                var r = new List<CategoryModel>();
                if (list.Count() > 0)
                {
                    foreach (var c in list.ToList())
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
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = r;
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
        public bool CheckCategory(string name)
        {
            var category = _context.Categorys.SingleOrDefault(c => c.Name == name);
            if (category == null)
                return false;
            return true;
        }
        public bool CheckSubCategory(string name)
        {
            var category = _context.SubCategories.SingleOrDefault(c => c.Sub_categoryName == name);
            if (category == null)
                return false;
            return true;
        }
        public bool CategoryisExist(int CategoryID)
        {
            var category = _context.Categorys.SingleOrDefault(c => c.CategoryID == CategoryID);
            if (category == null)
                return false;
            return true;
        }
        public Result GetSubCategory(int categoryID, string role)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (!CategoryisExist(categoryID))
                {
                    result.Success = false;
                    result.Message = "Category không tồn tại";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;

                }
                var listsub = _context.SubCategories.AsQueryable();
                listsub=listsub.Where(sc => sc.CategoryID == categoryID);
                if (string.IsNullOrEmpty(role) || role == "2" || role == "3")
                {
                    listsub = listsub.Where(sc => sc.IsActive);
                }
                List<Sub_CategoryModel> list = new List<Sub_CategoryModel>();
                foreach (var sub in listsub.ToList())
                {
                    Sub_CategoryModel mode = new Sub_CategoryModel
                    {
                        CategoryID = sub.CategoryID,
                        Sub_CategoryID = sub.Sub_CategoryID,
                        Sub_categoryName = sub.Sub_categoryName,
                        IsActive = sub.IsActive,
                    };
                    list.Add(mode);
                }
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = list;
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

        public Result RemoveCategory(int categoryID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var cate = _context.Categorys.SingleOrDefault(s => s.CategoryID == categoryID);
                if(cate != null)
                {
                    cate.IsActive = false;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = cate;
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

        public Result RemoveSubCategory(int subCategoryID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subcate = _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == subCategoryID);
                if (subcate != null)
                {
                    subcate.IsActive = false;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = subcate;
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

        public Result ActiveCategory(int categoryID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var cate = _context.Categorys.SingleOrDefault(s => s.CategoryID == categoryID);
                if (cate != null)
                {
                    cate.IsActive = true;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = cate;
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

        public Result ActiveSubCategory(int subCategoryID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subcate = _context.SubCategories.SingleOrDefault(s => s.Sub_CategoryID == subCategoryID);
                if (subcate != null)
                {
                    subcate.IsActive = true;
                }
                _context.SaveChanges();
                result.Success = true;
                result.Message = "Thành Công";
                result.Data = subcate;
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

        public Result CreateCategory(string category_Name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var cate = _context.Categorys.SingleOrDefault(c => c.Name == category_Name);
                if (cate == null)
                {
                    Category category = new Category();
                    category.Name = category_Name;
                    category.IsActive = true;
                    _context.Categorys.AddAsync(category);
                    _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = category;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Loại phụ tùng đã tồn tại";
                    result.Data = "";
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

        public Result CreateSubCategory(int categoryID, string subCategory_Name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var subcate=_context.SubCategories.SingleOrDefault(c => c.CategoryID == categoryID&& c.Sub_categoryName==subCategory_Name);
                if (subcate == null)
                {
                    Sub_Category category = new Sub_Category();
                    category.Sub_categoryName = subCategory_Name;
                    category.CategoryID = categoryID;
                    category.IsActive = true;
                    _context.SubCategories.AddAsync(category);
                    _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = category;
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "đã tồn tại phân loại này trong loại phụ tùng";
                    result.Data = "";
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
    }
}
