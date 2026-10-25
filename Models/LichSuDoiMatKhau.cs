// ============================================================================
// [MODULE 1 - VŨ TRƯỜNG AN: Thực thể Lịch Sử Đổi Mật Khẩu (Giới hạn 5 lần/ngày)]
// Tên Tệp: LichSuDoiMatKhau.cs
// ============================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("LichSuDoiMatKhau")]
    public class LichSuDoiMatKhau
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

        [Column(TypeName = "date")]
        [Display(Name = "Ngày đổi mật khẩu")]
        public DateTime NgayDoi { get; set; } = DateTime.Today;

        [Display(Name = "Thời gian đổi")]
        public DateTime ThoiGianDoi { get; set; } = DateTime.Now;

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
