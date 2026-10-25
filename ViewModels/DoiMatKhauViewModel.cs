// ============================================================================
// [MODULE 1 - VŨ TRƯỜNG AN: ViewModels Đổi Mật Khẩu qua OTP Gmail]
// Tên Tệp: DoiMatKhauViewModel.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class GuiYeuCauOtpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Gmail tài khoản cần đổi mật khẩu!")]
        [EmailAddress(ErrorMessage = "Địa chỉ Gmail không đúng định dạng!")]
        [Display(Name = "Gmail tài khoản")]
        public string Email { get; set; } = string.Empty;
    }

    public class XacNhanDoiMatKhauViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Địa chỉ Gmail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã OTP 6 chữ số!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải bao gồm đúng 6 chữ số!")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Mã OTP chỉ bao gồm 6 chữ số (0-9)!")]
        [Display(Name = "Mã xác thực OTP")]
        public string MaOtp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có tối thiểu 6 ký tự!")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu mới!")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không trùng khớp!")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        public int RemainingSeconds { get; set; } = 300; // 5 phút = 300 giây

        public int SoLanGuiOtpHomNay { get; set; } = 0;

        public int SoLanDaDoiHomNay { get; set; } = 0;
    }
}
