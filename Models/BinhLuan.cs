// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Bình luận tương tác bài viết]
// Tên Tệp: BinhLuan.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("BinhLuan")]
    public class BinhLuan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã bình luận")]
        public int MaBinhLuan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận!")]
        [StringLength(1000, ErrorMessage = "Nội dung bình luận không vượt quá 1000 ký tự!")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; } = string.Empty;

        [Display(Name = "Thời gian gửi")]
        public DateTime NgayBinhLuan { get; set; } = DateTime.Now;

        // Khóa ngoại tới TaiKhoan
        [Required]
        [Display(Name = "Tài khoản")]
        public int MaTaiKhoan { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }

        // Khóa ngoại tới KhoaHoc (tùy chọn)
        [Display(Name = "Khóa học")]
        public int? MaKhoaHoc { get; set; }
        public virtual KhoaHoc? KhoaHoc { get; set; }

        // Khóa ngoại tới BaiViet (tùy chọn)
        [Display(Name = "Bài viết")]
        public int? MaBaiViet { get; set; }
        public virtual BaiViet? BaiViet { get; set; }
    }
}
