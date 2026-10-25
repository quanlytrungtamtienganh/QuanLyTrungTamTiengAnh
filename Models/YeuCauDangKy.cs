// ============================================================================
// [MODULE 4 - SINH VIÊN 4: Thực thể Yêu cầu đăng ký khóa học]
// Tên Tệp: YeuCauDangKy.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("YeuCauDangKy")]
    public class YeuCauDangKy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaYeuCau { get; set; }

        [Required]
        [StringLength(150)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string VaiTroMongMuon { get; set; } = "HocVien";

        [StringLength(500)]
        public string? GhiChu { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;

        [StringLength(30)]
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy, DaTao, DaTu
    }
}
