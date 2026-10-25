// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Đề thi trắc nghiệm trực tuyến]
// Tên Tệp: DeThi.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("DeThi")]
    public class DeThi
    {
        [Key]
        public int MaDeThi { get; set; }

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required]
        [StringLength(50)]
        public string TheLoai { get; set; } = "IELTS Academic"; // IELTS Academic, IELTS General, TOEIC, HSK 1-6, TOPIK, N5-N1, Tiếng Anh THPTQG

        [Required]
        [StringLength(50)]
        public string KyNang { get; set; } = "Listening"; // Listening, Reading, Full Test

        public int ThoiGianPhut { get; set; } = 40; // Thời gian chuẩn tính bằng phút

        public int TongSoCau { get; set; } = 40;

        public int SoLuotLam { get; set; } = 0;

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [StringLength(500)]
        public string? FileAudio { get; set; } // Link file âm thanh (nếu là bài nghe)

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual ICollection<PhanThi> PhanThis { get; set; } = new List<PhanThi>();
        public virtual ICollection<BinhLuanDeThi> BinhLuans { get; set; } = new List<BinhLuanDeThi>();
        public virtual ICollection<KetQuaLuyenThi> KetQuas { get; set; } = new List<KetQuaLuyenThi>();
    }
}
