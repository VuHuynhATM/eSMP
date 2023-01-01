using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class SpecificationModel
    {
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class SpecificationValueModel
    {
        public int Specification_ValueID { get; set; }
        public string Value { get; set; }
        public int SpecificationID { get; set; }
        public int ItemID { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class SpecificationTagModel
    {
        public int Specification_ValueID { get; set; }
        public string Value { get; set; }
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public int ItemID { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class SpecificationTagRegister
    {
        public int SpecificationID { get; set; }
        public string Value { get; set; }
    }
    public class CateSpecification_Request
    {
        public int sub_CategoryID { get; set; }
        public int[] specificationIDs { get; set; }
    }
}
