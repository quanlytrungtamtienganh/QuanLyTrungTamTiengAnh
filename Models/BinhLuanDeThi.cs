// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Bình luận thảo luận đề thi]
// Tên Tệp: BinhLuanDeThi.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("BinhLuanDeThi")]
    public class BinhLuanDeThi
    {
        [Key]
        public int MaBinhLuan { get; set; }

        public int MaDeThi { get; set; }

        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(1000)]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayBinhLuan { get; set; } = DateTime.Now;

        public int? MaBinhLuanCha { get; set; } // Hỗ trợ trả lời (reply) bình luận

        // Navigation properties
        [ForeignKey("MaDeThi")]
        public virtual DeThi? DeThi { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
