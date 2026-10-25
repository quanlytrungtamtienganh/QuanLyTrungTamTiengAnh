// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Lượt thích (Like) bài viết]
// Tên Tệp: ThichBaiViet.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("ThichBaiViet")]
    public class ThichBaiViet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã thích")]
        public int MaThich { get; set; }

        [Required]
        [Display(Name = "Bài viết")]
        public int MaBaiViet { get; set; }
        public virtual BaiViet? BaiViet { get; set; }

        [Required]
        [Display(Name = "Người thích")]
        public int MaTaiKhoan { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Display(Name = "Thời gian thích")]
        public DateTime NgayThich { get; set; } = DateTime.Now;
    }
}
