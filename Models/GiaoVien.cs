// ============================================================================
// [MODULE 2 - TRẦN TRỌNG TRÁNG: Thực thể Giáo viên / Giảng viên]
// Tên Tệp: GiaoVien.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("GiaoVien")]
    public class GiaoVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã giáo viên")]
        public int MaGiaoVien { get; set; }

        [Required]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên giáo viên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chuyên môn đào tạo không được để trống!")]
        [StringLength(50)]
        [Display(Name = "Chuyên môn")]
        public string ChuyenMon { get; set; } = "Tiếng Anh (IELTS)";

        [StringLength(100)]
        [Display(Name = "Học vị / Chứng chỉ")]
        public string? HocVi { get; set; } = "Thạc sĩ / IELTS 8.5";

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Tiểu sử / Kinh nghiệm")]
        public string? GioiThieu { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDien { get; set; }

        // Quan hệ 1 - N với LopHoc (các lớp học do giáo viên phụ trách)
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}
