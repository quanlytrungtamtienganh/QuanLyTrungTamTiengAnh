// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Thành viên trong nhóm hội thoại]
// Tên Tệp: ThanhVienNhom.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("ThanhVienNhom")]
    public class ThanhVienNhom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThanhVien { get; set; }

        [Required]
        [Display(Name = "Mã nhóm")]
        public int MaNhom { get; set; }

        [Required]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Vai trò trong nhóm")]
        public string VaiTroTrongNhom { get; set; } = "ThanhVien"; // "TruongNhom", "QuanTriVien", "ThanhVien"

        [Display(Name = "Ngày tham gia")]
        public DateTime NgayThamGia { get; set; } = DateTime.Now;

        // Quan hệ
        [ForeignKey("MaNhom")]
        public virtual NhomChat? NhomChat { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
