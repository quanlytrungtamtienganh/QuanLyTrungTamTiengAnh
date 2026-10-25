using System.ComponentModel.DataAnnotations;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class ProfileViewModel
    {
        public int MaHocVien { get; set; }
        public int MaTaiKhoan { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống!")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số bắt đầu bằng 0!")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ thường trú")]
        [StringLength(255)]
        public string? DiaChi { get; set; }

        public string VaiTro { get; set; } = "HocVien";
    }
}
