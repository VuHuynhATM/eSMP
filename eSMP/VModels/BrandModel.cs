
namespace eSMP.VModels
{
    public class BrandModel
    {
        public int BrandID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
        public List<BrandItemModel>? listModel { get; set; }
    }
    public class BrandItemModel
    {
        public int Brand_ModelID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}
