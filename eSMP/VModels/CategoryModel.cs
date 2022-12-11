using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eSMP.VModels
{
    public class CategoryModel
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
        public List<Sub_CategoryModel>? listSub { get; set; }

    }
    public class Sub_CategoryModel
    {
        [Key]
        public int Sub_CategoryID { get; set; }
        public string Sub_categoryName { get; set; }
        public Boolean IsActive { get; set; }
        public int CategoryID { get; set; }
    }
}
