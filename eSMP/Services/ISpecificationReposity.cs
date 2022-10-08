using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public interface ISpecificationReposity
    {
        public Result GetAllSpecification();
        public Specification GetSpecificationByID(int specificationID);
        public Result GetSpecificationsBySubCate(int subCategoryID);
        public List<SpecificationTagModel> GetSpecificationsForItem(int itemID);
    }
}
