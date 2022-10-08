using eSMP.VModels;

namespace eSMP.Services
{
    public interface ICategoryReposity
    {
        public Result GetAllCategory();
        public Result GetSubCategory(int categoryID);
    }
}
