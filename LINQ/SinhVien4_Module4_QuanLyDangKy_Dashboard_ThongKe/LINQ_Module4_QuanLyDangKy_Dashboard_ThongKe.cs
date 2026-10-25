using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.LINQ.Module4
{
    /// <summary>
    /// PHÂN CHIA CHỨC NĂNG: SINH VIÊN 4 - MODULE 4
    /// NHIỆM VỤ: Quản lý đăng ký – Dashboard – Thống kê
    /// </summary>
    public class LINQ_Module4_QuanLyDangKy_Dashboard_ThongKe
    {
        private readonly TrungTamDbContext _context;

        public LINQ_Module4_QuanLyDangKy_Dashboard_ThongKe(TrungTamDbContext context)
        {
            _context = context;
        }

        #region ====================== PHẦN 1: QUẢN LÝ ĐĂNG KÝ ======================

        /// <summary>
        /// Câu 1: Lấy danh sách toàn bộ phiếu đăng ký học kèm thông tin Học viên, Lớp học và Khóa học.
        /// Sử dụng: Include, ThenInclude, OrderByDescending
        /// </summary>
        public async Task<List<DangKyHoc>> Q1_LayDanhSachDangKyTongHopAsync()
        {
            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 1 (Query Syntax): Viết lại bằng cú pháp LINQ SQL-like
        /// </summary>
        public List<DangKyHoc> Q1_QuerySyntax_LayDanhSachDangKy()
        {
            var query = from d in _context.DangKyHocs
                        orderby d.NgayDangKy descending
                        select d;
            return query.ToList();
        }

        /// <summary>
        /// Câu 2: Lọc danh sách đăng ký học theo trạng thái ("Chờ xác nhận", "Đã xác nhận", "Đã hủy").
        /// Sử dụng: Where, Include, OrderByDescending
        /// </summary>
        public async Task<List<DangKyHoc>> Q2_LocDangKyTheoTrangThaiAsync(string trangThai)
        {
            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Where(d => d.TrangThai == trangThai)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 3: Tìm kiếm phiếu đăng ký theo từ khóa (khớp tên học viên, số điện thoại hoặc tên lớp học).
        /// Sử dụng: Where, Contains, ToLower
        /// </summary>
        public async Task<List<DangKyHoc>> Q3_TimKiemDangKyAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await Q1_LayDanhSachDangKyTongHopAsync();

            tuKhoa = tuKhoa.Trim().ToLower();

            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Where(d => (d.HocVien != null && d.HocVien.HoTen.ToLower().Contains(tuKhoa))
                         || (d.HocVien != null && d.HocVien.SoDienThoai.Contains(tuKhoa))
                         || (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(tuKhoa)))
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 4: Lấy danh sách các đơn đăng ký mới cần duyệt trong ngày hôm nay.
        /// Sử dụng: Where so sánh Date, OrderBy
        /// </summary>
        public async Task<List<DangKyHoc>> Q4_DanhSachDangKyCanDuyetHomNayAsync()
        {
            var homNay = DateTime.Today;

            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .Where(d => d.TrangThai == "Chờ xác nhận" && d.NgayDangKy.Date == homNay)
                .OrderBy(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 5: Phân trang danh sách đăng ký học.
        /// Sử dụng: Skip, Take, CountAsync
        /// </summary>
        public async Task<(List<DangKyHoc> Items, int TotalCount, int TotalPages)> Q5_PhanTrangDanhSachDangKyAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .OrderByDescending(d => d.NgayDangKy);

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalPages);
        }

        #endregion

        #region ====================== PHẦN 2: DASHBOARD TỔNG QUAN ======================

        /// <summary>
        /// Câu 6: Lấy tất cả các chỉ số tổng quan (KPIs) hiển thị trên màn hình Dashboard Admin.
        /// Sử dụng: CountAsync, SumAsync
        /// </summary>
        public async Task<object> Q6_LayChiSoDashboardTongQuanAsync()
        {
            int tongHocVien = await _context.HocViens.CountAsync();
            int tongKhoaHoc = await _context.KhoaHocs.CountAsync();
            int tongLopHoc = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang tuyển sinh" || l.TrangThai == "Đang học");
            int dangKyChoDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ xác nhận");
            decimal tongDoanhThu = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận")
                .SumAsync(d => (decimal?)d.HocPhi) ?? 0m;

            return new
            {
                TongHocVien = tongHocVien,
                TongKhoaHoc = tongKhoaHoc,
                TongLopHocDangMo = tongLopHoc,
                DangKyChoDuyet = dangKyChoDuyet,
                TongDoanhThu = tongDoanhThu
            };
        }

        /// <summary>
        /// Câu 7: Thống kê tỷ lệ các đơn đăng ký theo trạng thái ("Chờ xác nhận", "Đã xác nhận", "Đã hủy").
        /// Sử dụng: GroupBy, Count, Select
        /// </summary>
        public async Task<List<object>> Q7_ThongKeTyLeDangKyTheoTrangThaiAsync()
        {
            int tongDangKy = await _context.DangKyHocs.CountAsync();
            if (tongDangKy == 0) return new List<object>();

            var result = await _context.DangKyHocs
                .GroupBy(d => d.TrangThai)
                .Select(g => new
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count(),
                    TyLePhanTram = Math.Round((double)g.Count() * 100.0 / tongDangKy, 2)
                })
                .ToListAsync<object>();

            return result;
        }

        /// <summary>
        /// Câu 8: Lấy Top 5 đơn đăng ký gần đây nhất để hiển thị widget hoạt động mới trên Dashboard.
        /// Sử dụng: OrderByDescending, Take
        /// </summary>
        public async Task<List<DangKyHoc>> Q8_Top5DangKyMoiNhatAsync()
        {
            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .OrderByDescending(d => d.NgayDangKy)
                .Take(5)
                .ToListAsync();
        }

        #endregion

        #region ================== PHẦN 3: BÁO CÁO THỐNG KÊ DOANH THU ==================

        /// <summary>
        /// Câu 9: Thống kê doanh thu và số lượt đăng ký theo từng khóa học.
        /// Sử dụng: GroupBy theo quan hệ LopHoc.KhoaHoc, Sum, Count, OrderByDescending
        /// </summary>
        public async Task<List<object>> Q9_ThongKeDoanhThuTheoKhoaHocAsync()
        {
            var result = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận" && d.LopHoc != null && d.LopHoc.KhoaHoc != null)
                .GroupBy(d => new
                {
                    d.LopHoc!.KhoaHoc!.MaKhoaHoc,
                    d.LopHoc!.KhoaHoc!.TenKhoaHoc,
                    d.LopHoc!.KhoaHoc!.NgoaiNgu
                })
                .Select(g => new
                {
                    MaKhoaHoc = g.Key.MaKhoaHoc,
                    TenKhoaHoc = g.Key.TenKhoaHoc,
                    NgoaiNgu = g.Key.NgoaiNgu,
                    TongLuotDangKy = g.Count(),
                    TongDoanhThu = g.Sum(d => d.HocPhi),
                    HocPhiTrungBinh = g.Average(d => d.HocPhi)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync<object>();

            return result;
        }

        /// <summary>
        /// Câu 10: Thống kê doanh thu theo từng tháng trong năm xác định (12 tháng).
        /// Sử dụng: GroupBy theo Tháng, Sum, OrderBy
        /// </summary>
        public async Task<List<object>> Q10_ThongKeDoanhThuTheoThangAsync(int nam)
        {
            var doanhThuCacThang = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận" && d.NgayDangKy.Year == nam)
                .GroupBy(d => d.NgayDangKy.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    SoLuongDon = g.Count(),
                    DoanhThu = g.Sum(d => d.HocPhi)
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            // Đảm bảo đủ 12 tháng (kể cả tháng doanh thu = 0)
            var full12Thang = Enumerable.Range(1, 12).Select(thang =>
            {
                var timThay = doanhThuCacThang.FirstOrDefault(x => x.Thang == thang);
                return (object)new
                {
                    Thang = $"Tháng {thang}",
                    SoLuongDon = timThay != null ? timThay.SoLuongDon : 0,
                    DoanhThu = timThay != null ? timThay.DoanhThu : 0m
                };
            }).ToList();

            return full12Thang;
        }

        /// <summary>
        /// Câu 11: Top N khóa học được học viên yêu thích và đăng ký nhiều nhất.
        /// Sử dụng: GroupBy, Count, OrderByDescending, Take
        /// </summary>
        public async Task<List<object>> Q11_TopKhoaHocUaChuongNhatAsync(int top = 5)
        {
            var result = await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã xác nhận" && d.LopHoc != null && d.LopHoc.KhoaHoc != null)
                .GroupBy(d => d.LopHoc!.KhoaHoc!.TenKhoaHoc)
                .Select(g => new
                {
                    TenKhoaHoc = g.Key,
                    SoHocVien = g.Count(),
                    DoanhThu = g.Sum(d => d.HocPhi)
                })
                .OrderByDescending(x => x.SoHocVien)
                .Take(top)
                .ToListAsync<object>();

            return result;
        }

        /// <summary>
        /// Câu 12: Thống kê số lượng lớp học và tổng số học viên theo từng giáo viên giảng dạy.
        /// Sử dụng: GroupBy theo Giáo viên, Count, Sum
        /// </summary>
        public async Task<List<object>> Q12_ThongKeLopVaHocVienTheoGiaoVienAsync()
        {
            var result = await _context.GiaoViens
                .Select(gv => new
                {
                    gv.MaGiaoVien,
                    gv.HoTen,
                    gv.ChuyenMon,
                    TongSoLop = gv.LopHocs.Count(),
                    TongSoHocVien = gv.LopHocs.SelectMany(l => l.DangKyHocs).Count(d => d.TrangThai == "Đã xác nhận")
                })
                .OrderByDescending(x => x.TongSoHocVien)
                .ToListAsync<object>();

            return result;
        }

        #endregion
    }
}
