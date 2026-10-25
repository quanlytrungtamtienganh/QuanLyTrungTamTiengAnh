// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Kết quả làm bài thi & bảng điểm]
// Tên Tệp: KetQuaLuyenThi.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("KetQuaLuyenThi")]
    public class KetQuaLuyenThi
    {
        [Key]
        public int MaKetQua { get; set; }

        public int MaDeThi { get; set; }

        public int? MaTaiKhoan { get; set; } // Có thể lưu kết quả của học viên hoặc khách

        [StringLength(50)]
        public string CheDoLam { get; set; } = "LuyenTap"; // LuyenTap hoặc FullTest

        [StringLength(255)]
        public string? CacPhanDaLam { get; set; } // VD: "Recording 1" hoặc "Full Test (4 phần)"

        public int SoCauDung { get; set; } = 0;

        public int SoCauSai { get; set; } = 0;

        public int SoCauBoQua { get; set; } = 0;

        public int TongSoCau { get; set; } = 0;

        public double DiemSo { get; set; } = 0.0; // Điểm quy đổi hoặc thang 10

        public double DoChinhXac { get; set; } = 0.0; // Phần trăm đúng (VD: 85.0%)

        public int ThoiGianLamGiay { get; set; } = 0; // Số giây làm bài

        public DateTime NgayNop { get; set; } = DateTime.Now;

        public string? ChiTietBaiLamJson { get; set; } // Chuỗi JSON lưu chi tiết từng câu: { [MaCauHoi]: { UserAnswer: "A", IsCorrect: true } }

        // Navigation properties
        [ForeignKey("MaDeThi")]
        public virtual DeThi? DeThi { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
