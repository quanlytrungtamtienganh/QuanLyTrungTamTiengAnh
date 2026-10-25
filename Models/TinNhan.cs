// ============================================================================
// [MODULE MỞ RỘNG: Thực thể Tin nhắn chat thời gian thực SignalR]
// Tên Tệp: TinNhan.cs
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamNgoaiNgu.Models
{
    [Table("TinNhan")]
    public class TinNhan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTinNhan { get; set; }

        [Required]
        [Display(Name = "Người gửi")]
        public int NguoiGuiId { get; set; }

        [Display(Name = "Người nhận (nếu chat riêng 1-1)")]
        public int? NguoiNhanId { get; set; }

        [Display(Name = "Mã nhóm (nếu chat nhóm)")]
        public int? MaNhom { get; set; }

        [Display(Name = "Loại tin nhắn")]
        [StringLength(20)]
        public string LoaiTinNhan { get; set; } = "VanBan"; // "VanBan", "HinhAnh", "HeThong", "CuocGoi"

        [Display(Name = "Nội dung")]
        [StringLength(2000)]
        public string? NoiDung { get; set; }

        [Display(Name = "Hình ảnh đính kèm")]
        [StringLength(500)]
        public string? HinhAnhUrl { get; set; }

        [Display(Name = "Thời gian")]
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [Display(Name = "Đã xem")]
        public bool DaXem { get; set; } = false;

        // Quan hệ
        [ForeignKey("NguoiGuiId")]
        public virtual TaiKhoan? NguoiGui { get; set; }

        [ForeignKey("NguoiNhanId")]
        public virtual TaiKhoan? NguoiNhan { get; set; }

        [ForeignKey("MaNhom")]
        public virtual NhomChat? NhomChat { get; set; }
    }
}
