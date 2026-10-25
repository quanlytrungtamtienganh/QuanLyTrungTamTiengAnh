// ============================================================================
// [MODULE 3 - SINH VIÊN 3: Thực thể Học viên]
// Tên Tệp: HocVien.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Required]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống!")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; } = new DateTime(2000, 1, 1);

        [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam"; // "Nam", "Nữ"

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ! (Ví dụ: 0912345678)")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        [StringLength(100)]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Địa chỉ liên hệ")]
        public string? DiaChi { get; set; }

        // Quan hệ
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
