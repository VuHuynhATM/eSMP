using eSMP.VModels;

namespace eSMP.Services
{
    public interface IBrandReposity
    {
        public Result GetAllBrand();
        public Result GetBrandModel(int BrandID);
        public Result GetBrandModelForItem(int ItemID);
        public List<BrandModel> GetModelForItem(int ItemID);
    }
}
