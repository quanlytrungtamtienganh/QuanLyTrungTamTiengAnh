using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.LINQ.Module3
{
    /// <summary>
    /// PHÂN CHIA CHỨC NĂNG: SINH VIÊN 3 - MODULE 3
    /// NHIỆM VỤ: Quản lý học viên – Đăng ký lớp học – Lịch sử đăng ký
    /// </summary>
    public class LINQ_Module3_HocVien_DangKyLopHoc
    {
        private readonly TrungTamDbContext _context;

        public LINQ_Module3_HocVien_DangKyLopHoc(TrungTamDbContext context)
        {
            _context = context;
        }

        #region ======================= PHẦN 1: QUẢN LÝ HỌC VIÊN =======================

        /// <summary>
        /// Câu 1: Lấy danh sách tất cả học viên trong trung tâm kèm thông tin tài khoản đăng nhập.
        /// Sử dụng: AsNoTracking, Include, OrderBy
        /// </summary>
        public async Task<List<HocVien>> Q1_LayDanhSachHocVienKiemTaiKhoanAsync()
        {
            return await _context.HocViens
                .AsNoTracking()
                .Include(h => h.TaiKhoan)
                .OrderBy(h => h.HoTen)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 1 (Query Syntax): Viết lại bằng cú pháp LINQ SQL-like
        /// </summary>
        public List<HocVien> Q1_QuerySyntax_LayDanhSachHocVien()
        {
            var query = from hv in _context.HocViens
                        orderby hv.HoTen ascending
                        select hv;
            return query.ToList();
        }

        /// <summary>
        /// Câu 2: Tìm kiếm học viên theo từ khóa (khớp họ tên, email, số điện thoại hoặc địa chỉ).
        /// Sử dụng: Where, Contains, ToLower
        /// </summary>
        public async Task<List<HocVien>> Q2_TimKiemHocVienAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await _context.HocViens.ToListAsync();

            tuKhoa = tuKhoa.Trim().ToLower();

            return await _context.HocViens
                .AsNoTracking()
                .Include(h => h.TaiKhoan)
                .Where(h => h.HoTen.ToLower().Contains(tuKhoa)
                         || h.Email.ToLower().Contains(tuKhoa)
                         || h.SoDienThoai.Contains(tuKhoa)
                         || (h.DiaChi != null && h.DiaChi.ToLower().Contains(tuKhoa)))
                .OrderBy(h => h.HoTen)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 3: Lấy hồ sơ chi tiết học viên kèm theo tất cả các lớp học đã từng đăng ký và trạng thái học tập.
        /// Sử dụng: Include, ThenInclude, FirstOrDefaultAsync
        /// </summary>
        public async Task<HocVien?> Q3_ChiTietHocVienVaCacLopDaHocAsync(int maHocVien)
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l!.KhoaHoc)
                .FirstOrDefaultAsync(h => h.MaHocVien == maHocVien);
        }

        /// <summary>
        /// Câu 4: Kiểm tra tính hợp lệ và duy nhất của số điện thoại và email học viên (Validation).
        /// Sử dụng: AnyAsync
        /// </summary>
        public async Task<bool> Q4_KiemTraSoDienThoaiTonTaiAsync(string sdt, int? boQuaMaHocVien = null)
        {
            var query = _context.HocViens.Where(h => h.SoDienThoai == sdt.Trim());
            if (boQuaMaHocVien.HasValue)
                query = query.Where(h => h.MaHocVien != boQuaMaHocVien.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Câu 5: Thống kê số lượng học viên theo giới tính (Nam / Nữ) và độ tuổi trung bình.
        /// Sử dụng: GroupBy, Count, Select
        /// </summary>
        public async Task<List<object>> Q5_ThongKeHocVienTheoGioiTinhAsync()
        {
            var namHienTai = DateTime.Today.Year;

            var result = await _context.HocViens
                .GroupBy(h => h.GioiTinh)
                .Select(g => new
                {
                    GioiTinh = g.Key,
                    SoLuong = g.Count(),
                    TuoiTrungBinh = g.Average(h => namHienTai - h.NgaySinh.Year)
                })
                .ToListAsync<object>();

            return result;
        }

        #endregion

        #region ====================== PHẦN 2: ĐĂNG KÝ LỚP HỌC ======================

        /// <summary>
        /// Câu 6: Kiểm tra học viên đã đăng ký vào một lớp học xác định hay chưa.
        /// Sử dụng: AnyAsync (ngăn chặn tình trạng 1 học viên bấm đăng ký 2 lần vào cùng 1 lớp)
        /// </summary>
        public async Task<bool> Q6_KiemTraHocVienDaDangKyLopAsync(int maHocVien, int maLop)
        {
            return await _context.DangKyHocs
                .AnyAsync(d => d.MaHocVien == maHocVien 
                            && d.MaLop == maLop 
                            && d.TrangThai != "Đã hủy");
        }

        /// <summary>
        /// Câu 7: Kiểm tra xem lịch học của lớp mới có bị xung đột (trùng ca/thứ) với các lớp học viên đang học không.
        /// Sử dụng: Where, Select, Any
        /// </summary>
        public async Task<bool> Q7_KiemTraTrungLichHocAsync(int maHocVien, string lichHocMoi)
        {
            var lichHocDangHoc = await _context.DangKyHocs
                .Where(d => d.MaHocVien == maHocVien 
                         && d.TrangThai == "Đã xác nhận" 
                         && d.LopHoc != null 
                         && d.LopHoc.TrangThai == "Đang học")
                .Select(d => d.LopHoc!.LichHoc)
                .ToListAsync();

            // So sánh chuỗi lịch học
            return lichHocDangHoc.Any(lh => lh.Trim().Equals(lichHocMoi.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Câu 8: Lấy danh sách tất cả các lớp học mở mà học viên này CHƯA đăng ký và vẫn CÒN chỗ trống.
        /// Sử dụng: Subquery với !Contains, Where, Include
        /// </summary>
        public async Task<List<LopHoc>> Q8_LayDanhSachLopCoTheDangKyAsync(int maHocVien)
        {
            // Danh sách mã lớp học viên này đã đăng ký
            var cacLopDaDangKyIds = _context.DangKyHocs
                .Where(d => d.MaHocVien == maHocVien && d.TrangThai != "Đã hủy")
                .Select(d => d.MaLop);

            // Lọc ra các lớp đang tuyển sinh, chưa có trong danh sách trên
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Where(l => l.TrangThai == "Đang tuyển sinh" 
                         && !cacLopDaDangKyIds.Contains(l.MaLop))
                .OrderBy(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        #endregion

        #region ===================== PHẦN 3: LỊCH SỬ ĐĂNG KÝ =====================

        /// <summary>
        /// Câu 9: Lấy toàn bộ lịch sử đăng ký học của một học viên, sắp xếp theo thời gian đăng ký mới nhất.
        /// Sử dụng: Include, ThenInclude, Where, OrderByDescending
        /// </summary>
        public async Task<List<DangKyHoc>> Q9_LayLichSuDangKyCuaHocVienAsync(int maHocVien)
        {
            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.GiaoVien)
                .Where(d => d.MaHocVien == maHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 10: Lọc lịch sử đăng ký của học viên theo trạng thái duyệt ("Chờ xác nhận", "Đã xác nhận", "Đã hủy").
        /// Sử dụng: Where kép
        /// </summary>
        public async Task<List<DangKyHoc>> Q10_LocLichSuDangKyTheoTrangThaiAsync(int maHocVien, string trangThai)
        {
            return await _context.DangKyHocs
                .AsNoTracking()
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Where(d => d.MaHocVien == maHocVien && d.TrangThai == trangThai)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 11: Tính tổng số tiền học phí học viên đã thanh toán/hoàn tất cho các lớp học thành công.
        /// Sử dụng: Where, SumAsync
        /// </summary>
        public async Task<decimal> Q11_TinhTongHocPhiHocVienDaDongAsync(int maHocVien)
        {
            return await _context.DangKyHocs
                .Where(d => d.MaHocVien == maHocVien && d.TrangThai == "Đã xác nhận")
                .SumAsync(d => d.HocPhi);
        }

        /// <summary>
        /// Câu 12: Lấy danh sách tên các khóa học (không trùng lặp) mà học viên đã và đang tham gia.
        /// Sử dụng: Where, Select, Distinct, ToListAsync
        /// </summary>
        public async Task<List<string>> Q12_DanhSachKhoaHocHocVienDaThamGiaAsync(int maHocVien)
        {
            return await _context.DangKyHocs
                .Where(d => d.MaHocVien == maHocVien 
                         && d.TrangThai == "Đã xác nhận" 
                         && d.LopHoc != null 
                         && d.LopHoc.KhoaHoc != null)
                .Select(d => d.LopHoc!.KhoaHoc!.TenKhoaHoc)
                .Distinct()
                .OrderBy(ten => ten)
                .ToListAsync();
        }

        #endregion
    }
}
