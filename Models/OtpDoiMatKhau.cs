// ============================================================================
// [MODULE 1 - VŨ TRƯỜNG AN: Thực thể Mã OTP Đổi Mật Khẩu]
// Tên Tệp: OtpDoiMatKhau.cs
// ============================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("OtpDoiMatKhau")]
    public class OtpDoiMatKhau
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Địa chỉ Gmail")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã OTP")]
        public string MaOtp { get; set; } = string.Empty;

        [Display(Name = "Thời gian tạo")]
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        [Display(Name = "Thời gian hết hạn")]
        public DateTime ThoiGianHetHan { get; set; }

        [Display(Name = "Đã sử dụng")]
        public bool DaSuDung { get; set; } = false;

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
