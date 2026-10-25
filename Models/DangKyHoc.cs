// ============================================================================
// [MODULE 3 - PHẠM THANH TÙNG: Thực thể Đăng ký lớp học & Lịch sử học tập]
// Tên Tệp: DangKyHoc.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("DangKyHoc")]
    public class DangKyHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Required]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required]
        [Display(Name = "Lớp học")]
        public int MaLop { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí")]
        public decimal HocPhi { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ xác nhận"; // "Chờ xác nhận", "Đã xác nhận", "Đã hủy"

        // Quan hệ
        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }
    }
}
