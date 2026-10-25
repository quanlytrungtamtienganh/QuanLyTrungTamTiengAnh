// ============================================================================
// [MODULE 4 - SINH VIÊN 4: Thực thể Cấu hình hệ thống & Thống kê]
// Tên Tệp: CauHinhHeThong.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("CauHinhHeThong")]
    public class CauHinhHeThong
    {
        [Key]
        [StringLength(100)]
        [Display(Name = "Khóa cấu hình")]
        public string Khoa { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Giá trị")]
        public string GiaTri { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}
