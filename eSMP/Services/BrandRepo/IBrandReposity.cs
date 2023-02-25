using eSMP.VModels;

namespace eSMP.Services.BrandRepo
{
    public interface IBrandReposity
    {
        public Result GetAllBrand(string? role);
        public Result GetBrandModel(int BrandID, string? role);
        public Result GetBrandModelForItem(int ItemID);
        public Result CreateBrand(string brand_Name);
        public Result CreateMotorcycle(int brandID, string moto_Name);
        public Result RemoveMotorcycle(int motorcycleID);
        public Result RemoveBrand(int brandID);
        public Result ActiveMotorcycle(int motorcycleID);
        public Result ActiveBrand(int brandID);
        public List<BrandModel> GetModelForItem(int ItemID);
    }
}
