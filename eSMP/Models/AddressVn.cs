using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("AddressVn")]
    public class AddressVn
    {
        [Key]
        public string id { get; set; }
        public string ward { get; set; }
        public string levelVN { get; set; }
        public string qhid { get; set; }
        public string district { get; set; }
        public string tpid { get; set; }
        public string province { get; set; }
    }
}
