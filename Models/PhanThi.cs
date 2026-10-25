// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Phần thi (Sections/Parts) trong đề thi]
// Tên Tệp: PhanThi.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("PhanThi")]
    public class PhanThi
    {
        [Key]
        public int MaPhanThi { get; set; }

        public int MaDeThi { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPhanThi { get; set; } = string.Empty; // VD: Recording 1, Recording 2, Passage 1, Part 5

        public int ThuTu { get; set; } = 1;

        [StringLength(255)]
        public string? TieuDePhan { get; set; } // VD: Complete the notes below. Write ONE WORD AND/OR A NUMBER for each answer.

        public string? DoanVanDoc { get; set; } // Đoạn văn đọc Reading hoặc Transcript

        [StringLength(500)]
        public string? FileAudio { get; set; }

        [StringLength(100)]
        public string? TagDangBai { get; set; } // VD: #Listening|Note/Form Completion, #Listening|Table Completion

        // Navigation properties
        [ForeignKey("MaDeThi")]
        public virtual DeThi? DeThi { get; set; }

        public virtual ICollection<CauHoiLuyenThi> CauHois { get; set; } = new List<CauHoiLuyenThi>();
    }
}
