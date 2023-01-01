using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.SpecificationRepo
{
    public interface ISpecificationReposity
    {
        public Result GetAllSpecification();
        public Specification GetSpecificationByID(int specificationID);
        public Result GetSpecificationsBySubCate(int subCategoryID);
        public Result CreateSpecification(string specification_Name);
        public Result AddSpecification(int sub_CategoryID, int[] specificationIDs);
        public Result ReomoveCateSpecification(int sub_CategoryID, int[] specificationIDs);
        public Result ReomoveSpecification(int specificationID);
        public List<SpecificationTagModel> GetSpecificationsForItem(int itemID);
    }
}
