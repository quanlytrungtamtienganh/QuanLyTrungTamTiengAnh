// ============================================================================
// [MODULE 1 - VŨ TRƯỜNG AN: Thực thể Tài khoản người dùng & Phân quyền]
// Tên Tệp: TaiKhoan.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không vượt quá 50 ký tự!")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        [StringLength(100)]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "HocVien"; // "Admin", "HocVien"

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "HoatDong"; // "HoatDong", "Khoa"

        // Quan hệ 1 - 1 với HocVien (nếu tài khoản là học viên)
        public virtual HocVien? HocVien { get; set; }

        // Quan hệ 1 - 1 với GiaoVien (nếu tài khoản là giáo viên)
        public virtual GiaoVien? GiaoVien { get; set; }

        // Quan hệ 1 - N với BinhLuan
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
    }
}
