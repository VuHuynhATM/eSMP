using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.SpecificationRepo
{
    public interface ISpecificationReposity
    {
        public Result GetAllSpecification();
        public Result GetSpecificationsBySubCate(int subCategoryID, string? role);
        public Result CreateSpecification(string specification_Name);
        public Result AddSpecification(CateSpecification_Request request);
        public Result ReomoveCateSpecification(int sub_CategoryID, int[] specificationIDs);
        public Result ReomoveSpecification(int specificationID);
        public Result ActiveSpecification(int specificationID);
        public Result AddSuggesTSpecification(int SpecificationID, string suggestValues);
        public List<SpecificationTagModel> GetSpecificationsForItem(int itemID);
    }
}
