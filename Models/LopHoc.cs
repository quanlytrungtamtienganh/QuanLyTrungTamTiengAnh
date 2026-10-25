// ============================================================================
// [MODULE 2 - TRẦN TRỌNG TRÁNG: Thực thể Lớp học ngoại ngữ]
// Tên Tệp: LopHoc.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("LopHoc")]
    public class LopHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã lớp")]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp học không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên lớp học không vượt quá 100 ký tự!")]
        [Display(Name = "Tên lớp học")]
        public string TenLop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn khóa học!")]
        [Display(Name = "Khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Ngày khai giảng không được để trống!")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khai giảng")]
        public DateTime NgayKhaiGiang { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Lịch học không được để trống!")]
        [StringLength(100)]
        [Display(Name = "Lịch học")]
        public string LichHoc { get; set; } = string.Empty; // VD: "Tối 2-4-6 (18:00 - 19:30)"

        [Required(ErrorMessage = "Phòng học không được để trống!")]
        [StringLength(50)]
        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = string.Empty; // VD: "Phòng A.101"

        [Required(ErrorMessage = "Sĩ số tối đa không được để trống!")]
        [Range(1, 200, ErrorMessage = "Sĩ số tối đa từ 1 đến 200 học viên!")]
        [Display(Name = "Số lượng tối đa")]
        public int SoLuongToiDa { get; set; } = 25;

        [Required(ErrorMessage = "Học phí không được để trống!")]
        [Range(0, 1000000000, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0!")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang tuyển sinh"; // "Đang tuyển sinh", "Đang học", "Đã kết thúc", "Đã hủy"

        [Display(Name = "Giáo viên giảng dạy")]
        public int? MaGiaoVien { get; set; }

        // Quan hệ
        [ForeignKey("MaKhoaHoc")]
        public virtual KhoaHoc? KhoaHoc { get; set; }

        [ForeignKey("MaGiaoVien")]
        public virtual GiaoVien? GiaoVien { get; set; }

        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
