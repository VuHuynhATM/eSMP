using eSMP.VModels;

namespace eSMP.Services.CategoryRepo
{
    public interface ICategoryReposity
    {
        public Result GetAllCategory();
        public Result GetSubCategory(int categoryID);
    }
}
