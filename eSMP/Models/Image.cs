using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Image")]
    public class Image
    {
        [Key]
        public int ImageID { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public DateTime Crete_date { get; set; }
        public Boolean IsActive { get; set; }
    }
}
