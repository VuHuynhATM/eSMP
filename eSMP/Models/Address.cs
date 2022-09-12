using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public int AddressID { get; set; }
        public string context { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Boolean IsActive { get; set; }
    }
}
