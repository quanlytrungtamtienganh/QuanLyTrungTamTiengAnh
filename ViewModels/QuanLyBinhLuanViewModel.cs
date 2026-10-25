using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class TaiKhoanTuongTacItem
    {
        public int MaTaiKhoan { get; set; }
        public int? MaHocVien { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string VaiTro { get; set; } = "HocVien";
        
        // Thống kê bình luận & tương tác
        public int SoBinhLuan { get; set; }
        public double PhanTramDongGop { get; set; } // % trên tổng số bình luận toàn hệ thống
        public DateTime? LanTuongTacCuoi { get; set; }
        public string MucDoTuongTac { get; set; } = "Chưa tương tác";
    }

    public class QuanLyBinhLuanViewModel
    {
        // 4 Chỉ số KPI Tương Tác
        public int TongBinhLuan { get; set; }
        public int TongTaiKhoanHocVien { get; set; }
        public int HocVienDaTuongTac { get; set; }
        public double TyLeHocVienTuongTac { get; set; } // % học viên có tương tác (đã gửi comment)
        public string TopHocVienTuongTac { get; set; } = "Chưa có";
        public int SoLuongTop { get; set; }

        // Bảng danh sách % tương tác của các tài khoản dưới quyền admin
        public List<TaiKhoanTuongTacItem> DanhSachTaiKhoan { get; set; } = new();

        // Bảng danh sách toàn bộ bình luận để duyệt và xóa
        public List<BinhLuan> DanhSachBinhLuan { get; set; } = new();
    }
}
