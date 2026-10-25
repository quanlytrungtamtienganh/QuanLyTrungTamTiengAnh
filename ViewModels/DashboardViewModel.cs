using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class KhoaHocDoanhThuItem
    {
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string NgoaiNgu { get; set; } = string.Empty;
        public int SoHocVien { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DashboardViewModel
    {
        // 7 Chỉ số cốt lõi
        public int TongKhoaHoc { get; set; }
        public int TongLopHoc { get; set; }
        public int TongHocVien { get; set; }
        public int TongDangKy { get; set; }
        public int ChoXacNhan { get; set; }
        public int DaXacNhan { get; set; }
        public int DangTuyenSinh { get; set; }

        // 4.20. Thống kê Doanh thu học phí
        public decimal TongDoanhThuHocPhi { get; set; }
        public decimal DoanhThuDuKien { get; set; }

        // Dữ liệu biểu đồ Doanh thu theo tháng
        public List<string> DoanhThuThangLabels { get; set; } = new List<string>();
        public List<decimal> DoanhThuThangValues { get; set; } = new List<decimal>();

        // Dữ liệu biểu đồ Doanh thu theo Ngoại ngữ
        public List<string> DoanhThuNgoaiNguLabels { get; set; } = new List<string>();
        public List<decimal> DoanhThuNgoaiNguValues { get; set; } = new List<decimal>();

        // Danh sách doanh thu theo Khóa học
        public List<KhoaHocDoanhThuItem> TopKhoaHocDoanhThu { get; set; } = new List<KhoaHocDoanhThuItem>();

        // Bảng dữ liệu đăng ký gần đây
        public List<DangKyHoc> RecentDangKys { get; set; } = new List<DangKyHoc>();

        // Dữ liệu phục vụ biểu đồ phân bổ lớp học
        public List<string> LanguageLabels { get; set; } = new List<string>();
        public List<int> LanguageCounts { get; set; } = new List<int>();

        public List<string> StatusLabels { get; set; } = new List<string>();
        public List<int> StatusCounts { get; set; } = new List<int>();
    }
}
