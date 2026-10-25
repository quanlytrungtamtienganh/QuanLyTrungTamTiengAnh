using System.ComponentModel.DataAnnotations;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class EditTaiKhoanViewModel
    {
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò!")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "HocVien";

        [Required(ErrorMessage = "Vui lòng chọn trạng thái!")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "HoatDong";

        [Display(Name = "Mật khẩu mới (Để trống nếu giữ nguyên)")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Mật khẩu tối thiểu 3 ký tự!")]
        public string? MatKhauMoi { get; set; }
    }
}
