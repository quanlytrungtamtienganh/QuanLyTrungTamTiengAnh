// ============================================================================
// [MODULE 1 - VŨ TRƯỜNG AN: Thực thể Khóa học ngoại ngữ]
// Tên Tệp: KhoaHoc.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học không được để trống!")]
        [StringLength(150, ErrorMessage = "Tên khóa học không quá 150 ký tự!")]
        [Display(Name = "Tên khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngoại ngữ không được để trống!")]
        [StringLength(50)]
        [Display(Name = "Ngoại ngữ")]
        public string NgoaiNgu { get; set; } = string.Empty; // Tiếng Anh, Tiếng Hàn, Tiếng Nhật, Tiếng Trung...

        [Required(ErrorMessage = "Cấp độ không được để trống!")]
        [StringLength(50)]
        [Display(Name = "Cấp độ")]
        public string CapDo { get; set; } = string.Empty; // Sơ cấp, Trung cấp, Cao cấp, IELTS, TOPIK...

        [Required(ErrorMessage = "Học phí không được để trống!")]
        [Range(0, 1000000000, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0!")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Số buổi không được để trống!")]
        [Range(1, 500, ErrorMessage = "Số buổi học phải từ 1 đến 500 buổi!")]
        [Display(Name = "Số buổi")]
        public int SoBuoi { get; set; }

        [Display(Name = "Mô tả khóa học")]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang mở"; // "Đang mở", "Tạm ngưng"

        // Quan hệ 1 - N với LopHoc
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

        // Quan hệ 1 - N với BinhLuan
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
    }
}
