using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public int AddressID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Context { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Boolean IsActive { get; set; }
    }
}
