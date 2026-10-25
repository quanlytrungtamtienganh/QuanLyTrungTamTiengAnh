// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Câu hỏi luyện thi trắc nghiệm & file Audio]
// Tên Tệp: CauHoiLuyenThi.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("CauHoiLuyenThi")]
    public class CauHoiLuyenThi
    {
        [Key]
        public int MaCauHoi { get; set; }

        public int MaPhanThi { get; set; }

        public int SoThuTu { get; set; } = 1; // 1, 2, 3...

        [Required]
        public string NoiDung { get; set; } = string.Empty; // Nội dung câu hỏi

        [Required]
        [StringLength(30)]
        public string LoaiCauHoi { get; set; } = "TracNghiem"; // TracNghiem hoặc DienTu

        public string? DapAnA { get; set; }
        public string? DapAnB { get; set; }
        public string? DapAnC { get; set; }
        public string? DapAnD { get; set; }

        [Required]
        [StringLength(255)]
        public string DapAnDung { get; set; } = string.Empty; // A, B, C, D hoặc từ cần điền (VD: "Greendale", "300")

        public string? GiaiThichChiTiet { get; set; } // LÝ DO GIẢI THÍCH CHI TIẾT (Transcript, Keywords, Phân tích ngữ pháp)

        [StringLength(100)]
        public string? PhanLoaiDangBai { get; set; } // VD: [Listening] Note/Form Completion, [Listening] Table Completion

        public double Diem { get; set; } = 1.0;

        // Navigation properties
        [ForeignKey("MaPhanThi")]
        public virtual PhanThi? PhanThi { get; set; }
    }
}
