using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ServiceImage")]
    public class ServiceImage
    {
        [Key]
        public int ServiceImageID { get; set; }
        public int ImageID { get; set; }
        [ForeignKey("ImageID")]
        public virtual Image Image { get; set; }
        public int AfterBuyServiceID { get; set; }
        [ForeignKey("AfterBuyServiceID")]
        public virtual AfterBuyService AfterBuyService { get; set; }
    }
}
