using System.ComponentModel.DataAnnotations;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class CreateHocVienViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không quá 50 ký tự!")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Mật khẩu ít nhất 3 ký tự!")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = "123456";

        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự!")]
        [Display(Name = "Họ và tên học viên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống!")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; } = new DateTime(2002, 1, 1);

        [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số bắt đầu từ 0!")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không đúng định dạng!")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Địa chỉ liên hệ")]
        public string? DiaChi { get; set; }
    }
}
