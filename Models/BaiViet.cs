// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Bài viết diễn đàn cộng đồng]
// Tên Tệp: BaiViet.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("BaiViet")]
    public class BaiViet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã bài viết")]
        public int MaBaiViet { get; set; }

        [Required]
        [Display(Name = "Người đăng")]
        public int MaTaiKhoan { get; set; }
        public virtual TaiKhoan? NguoiDang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết!")]
        [StringLength(4000, ErrorMessage = "Nội dung bài viết không vượt quá 4000 ký tự!")]
        [Display(Name = "Nội dung bài viết")]
        public string NoiDung { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnhUrl { get; set; }

        [StringLength(500)]
        [Display(Name = "Video")]
        public string? VideoUrl { get; set; }

        [StringLength(50)]
        [Display(Name = "Phông nền")]
        public string? PhongNen { get; set; } // default, sunset, ocean, fire, berry, midnight, emerald

        [Display(Name = "Ngày đăng")]
        public DateTime NgayDang { get; set; } = DateTime.Now;

        // ChoDuyet (Chờ duyệt), DaDuyet (Đã duyệt), TuChoi (Từ chối)
        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DaDuyet";

        public int? NguoiDuyetId { get; set; }
        public virtual TaiKhoan? NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        // Bài viết gốc (nếu là bài chia sẻ)
        [Display(Name = "Bài viết gốc")]
        public int? MaBaiVietGoc { get; set; }
        public virtual BaiViet? BaiVietGoc { get; set; }

        [Display(Name = "Lượt chia sẻ")]
        public int SoChiaSe { get; set; } = 0;

        public virtual ICollection<BaoCaoBaiViet> BaoCaos { get; set; } = new List<BaoCaoBaiViet>();
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
        public virtual ICollection<ThichBaiViet> Thichs { get; set; } = new List<ThichBaiViet>();
    }
}
