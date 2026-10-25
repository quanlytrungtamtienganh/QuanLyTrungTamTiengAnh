// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Nhóm hội thoại & phòng gọi Video]
// Tên Tệp: NhomChat.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("NhomChat")]
    public class NhomChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhom { get; set; }

        [Required(ErrorMessage = "Tên nhóm không được để trống!")]
        [StringLength(150, ErrorMessage = "Tên nhóm không vượt quá 150 ký tự!")]
        [Display(Name = "Tên nhóm chat")]
        public string TenNhom { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Ảnh đại diện nhóm")]
        public string? AnhNhom { get; set; }

        [Required]
        [Display(Name = "Người tạo nhóm")]
        public int NguoiTaoId { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Trạng thái cuộc gọi trực tiếp trong nhóm
        [Display(Name = "Cuộc gọi đang diễn ra")]
        public bool CuocGoiDangDienRa { get; set; } = false;

        [StringLength(20)]
        [Display(Name = "Loại cuộc gọi")]
        public string? LoaiCuocGoi { get; set; } // "voice" hoặc "video"

        [Display(Name = "Người gọi")]
        public int? NguoiGoiId { get; set; }

        [Display(Name = "Thời gian bắt đầu gọi")]
        public DateTime? ThoiGianBatDauGoi { get; set; }

        // Quan hệ
        [ForeignKey("NguoiTaoId")]
        public virtual TaiKhoan? NguoiTao { get; set; }

        [ForeignKey("NguoiGoiId")]
        public virtual TaiKhoan? NguoiGoi { get; set; }

        public virtual ICollection<ThanhVienNhom> ThanhViens { get; set; } = new List<ThanhVienNhom>();
        public virtual ICollection<TinNhan> TinNhans { get; set; } = new List<TinNhan>();
    }
}
