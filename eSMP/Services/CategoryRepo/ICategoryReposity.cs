﻿using eSMP.VModels;

namespace eSMP.Services.CategoryRepo
{
    public interface ICategoryReposity
    {
        public Result GetAllCategory(string? role);
        public Result GetSubCategory(int categoryID, string? role);
        public Result RemoveCategory(int categoryID);
        public Result RemoveSubCategory(int subCategoryID);
        public Result ActiveCategory(int categoryID);
        public Result ActiveSubCategory(int subCategoryID);
        public Result CreateCategory(string category_Name);
        public Result CreateSubCategory(int categoryID, string subCategory_Name);

    }
}
