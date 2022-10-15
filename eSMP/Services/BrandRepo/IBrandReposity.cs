using eSMP.VModels;

namespace eSMP.Services.BrandRepo
{
    public interface IBrandReposity
    {
        public Result GetAllBrand();
        public Result GetBrandModel(int BrandID);
        public Result GetBrandModelForItem(int ItemID);
        public List<BrandModel> GetModelForItem(int ItemID);
    }
}
