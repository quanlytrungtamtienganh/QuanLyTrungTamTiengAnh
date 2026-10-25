// ============================================================================
// [MODULE 4 - KHIẾU QUỐC TOẢN: QUẢN LÝ ĐĂNG KÝ – DASHBOARD – THỐNG KÊ]
// Tên Tệp: ThongKeController.cs
// Mô tả: Chức năng: Giao diện thống kê Admin, Dashboard tổng quan, Thống kê doanh thu học phí, Biểu đồ học viên & khóa học
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    [AuthorizeRole("Admin")]
    public class ThongKeController : Controller
    {
        private readonly TrungTamDbContext _context;

        public ThongKeController(TrungTamDbContext context)
        {
            _context = context;
        }

        // GET: /ThongKe hoặc /ThongKe/Index (Admin Dashboard)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                // 1. Tổng khóa học
                TongKhoaHoc = await _context.KhoaHocs.CountAsync(),

                // 2. Tổng lớp học
                TongLopHoc = await _context.LopHocs.CountAsync(),

                // 3. Tổng học viên
                TongHocVien = await _context.HocViens.CountAsync(),

                // 4. Tổng lượt đăng ký
                TongDangKy = await _context.DangKyHocs.CountAsync(),

                // 5. Chờ xác nhận
                ChoXacNhan = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ xác nhận"),

                // 6. Đã xác nhận
                DaXacNhan = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã xác nhận"),

                // 7. Lớp đang tuyển sinh
                DangTuyenSinh = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang tuyển sinh"),

                // Dữ liệu đăng ký gần đây
                RecentDangKys = await _context.DangKyHocs
                    .Include(d => d.HocVien)
                    .Include(d => d.LopHoc)
                        .ThenInclude(l => l!.KhoaHoc)
                    .OrderByDescending(d => d.NgayDangKy)
                    .Take(7)
                    .ToListAsync()
            };

            // Dữ liệu biểu đồ phân bổ theo ngoại ngữ
            var languages = new[] { "Tiếng Anh", "Tiếng Hàn", "Tiếng Nhật", "Tiếng Trung" };
            model.LanguageLabels = languages.ToList();
            foreach (var lang in languages)
            {
                int count = await _context.LopHocs.CountAsync(l => l.KhoaHoc != null && l.KhoaHoc.NgoaiNgu == lang);
                model.LanguageCounts.Add(count);
            }

            // Dữ liệu biểu đồ trạng thái đăng ký
            model.StatusLabels = new List<string> { "Đã xác nhận", "Chờ xác nhận", "Đã hủy" };
            model.StatusCounts = new List<int>
            {
                model.DaXacNhan,
                model.ChoXacNhan,
                await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã hủy")
            };

            // =========================================================================
            // 4.20. GIAO DIỆN THỐNG KÊ ADMIN: DOANH THU HỌC PHÍ
            // =========================================================================
            // 1. Tổng doanh thu thực tế (từ các đơn Đã xác nhận)
            model.TongDoanhThuHocPhi = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận")
                .SumAsync(d => (decimal?)d.HocPhi) ?? 0;

            // 2. Doanh thu dự kiến (từ các đơn Chờ xác nhận)
            model.DoanhThuDuKien = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Chờ xác nhận")
                .SumAsync(d => (decimal?)d.HocPhi) ?? 0;

            // 3. Doanh thu theo 6 tháng gần nhất
            var now = DateTime.Today;
            for (int i = 5; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                var label = $"Tháng {targetMonth.Month}/{targetMonth.Year}";
                model.DoanhThuThangLabels.Add(label);

                var revMonth = await _context.DangKyHocs
                    .Where(d => d.TrangThai == "Đã xác nhận" && d.NgayDangKy.Month == targetMonth.Month && d.NgayDangKy.Year == targetMonth.Year)
                    .SumAsync(d => (decimal?)d.HocPhi) ?? 0;

                model.DoanhThuThangValues.Add(revMonth);
            }

            // Nếu toàn bộ 6 tháng đều 0 nhưng có doanh thu thực tế, hiển thị doanh thu vào tháng hiện tại
            if (model.DoanhThuThangValues.All(v => v == 0) && model.TongDoanhThuHocPhi > 0)
            {
                model.DoanhThuThangValues[model.DoanhThuThangValues.Count - 1] = model.TongDoanhThuHocPhi;
            }

            // 4. Doanh thu theo từng Ngôn ngữ đào tạo
            foreach (var lang in languages)
            {
                var revLang = await _context.DangKyHocs
                    .Where(d => d.TrangThai == "Đã xác nhận" && d.LopHoc != null && d.LopHoc.KhoaHoc != null && d.LopHoc.KhoaHoc.NgoaiNgu == lang)
                    .SumAsync(d => (decimal?)d.HocPhi) ?? 0;

                model.DoanhThuNgoaiNguLabels.Add(lang);
                model.DoanhThuNgoaiNguValues.Add(revLang);
            }

            // 5. Doanh thu theo Khóa học
            var courseQuery = _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận" && d.LopHoc != null && d.LopHoc.KhoaHoc != null)
                .GroupBy(d => new { d.LopHoc!.KhoaHoc!.TenKhoaHoc, d.LopHoc.KhoaHoc.NgoaiNgu })
                .Select(g => new KhoaHocDoanhThuItem
                {
                    TenKhoaHoc = g.Key.TenKhoaHoc,
                    NgoaiNgu = g.Key.NgoaiNgu,
                    SoHocVien = g.Count(),
                    DoanhThu = g.Sum(x => x.HocPhi)
                })
                .OrderByDescending(x => x.DoanhThu)
                .Take(6);

            model.TopKhoaHocDoanhThu = await courseQuery.ToListAsync();

            return View(model);
        }
    }
}
