// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Báo cáo vi phạm bài viết]
// Tên Tệp: BaoCaoBaiViet.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("BaoCaoBaiViet")]
    public class BaoCaoBaiViet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã báo cáo")]
        public int MaBaoCao { get; set; }

        [Required]
        [Display(Name = "Bài viết")]
        public int MaBaiViet { get; set; }
        public virtual BaiViet? BaiViet { get; set; }

        [Required]
        [Display(Name = "Người báo cáo")]
        public int NguoiBaoCaoId { get; set; }
        public virtual TaiKhoan? NguoiBaoCao { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lý do báo cáo!")]
        [StringLength(200)]
        [Display(Name = "Lý do báo cáo")]
        public string LyDo { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Chi tiết sai phạm")]
        public string? ChiTiet { get; set; }

        [Display(Name = "Thời gian báo cáo")]
        public DateTime NgayBaoCao { get; set; } = DateTime.Now;

        // ChoXuLy (Chờ xử lý), DaXuLy_XoaBai (Đã xóa bài), DaBoQua (Đã bỏ qua)
        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái xử lý")]
        public string TrangThai { get; set; } = "ChoXuLy";
    }
}
