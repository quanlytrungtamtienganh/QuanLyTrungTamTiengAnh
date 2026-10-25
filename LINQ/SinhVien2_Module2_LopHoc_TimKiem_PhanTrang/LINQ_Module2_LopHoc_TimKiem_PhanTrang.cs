using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.LINQ.Module2
{
    /// <summary>
    /// PHÂN CHIA CHỨC NĂNG: TRẦN TRỌNG TRÁNG - MODULE 2
    /// NHIỆM VỤ: Quản lý lớp học – Tìm kiếm – Lọc – Sắp xếp – Phân trang
    /// </summary>
    public class LINQ_Module2_LopHoc_TimKiem_PhanTrang
    {
        private readonly TrungTamDbContext _context;

        public LINQ_Module2_LopHoc_TimKiem_PhanTrang(TrungTamDbContext context)
        {
            _context = context;
        }

        #region ======================= PHẦN 1: QUẢN LÝ LỚP HỌC =======================

        /// <summary>
        /// Câu 1: Lấy danh sách toàn bộ lớp học kèm khóa học và giáo viên phụ trách (Eager Loading).
        /// Sử dụng: AsNoTracking, Include, OrderByDescending
        /// </summary>
        public async Task<List<LopHoc>> Q1_LayDanhSachLopHocDayDuAsync()
        {
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .OrderByDescending(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 1 (Query Syntax): Viết lại bằng cú pháp truy vấn LINQ SQL-like
        /// </summary>
        public List<LopHoc> Q1_QuerySyntax_LayDanhSachLopHocDayDu()
        {
            var query = from l in _context.LopHocs
                        orderby l.NgayKhaiGiang descending
                        select l;
            return query.ToList();
        }

        /// <summary>
        /// Câu 2: Lấy chi tiết lớp học và thống kê sĩ số học viên thực tế đã được duyệt tham gia.
        /// Sử dụng: Include, FirstOrDefaultAsync, Count
        /// </summary>
        public async Task<object?> Q2_LayChiTietLopHocVaSiSoAsync(int maLop)
        {
            var lop = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Include(l => l.DangKyHocs)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);

            if (lop == null) return null;

            int soHocVienHienTai = lop.DangKyHocs.Count(d => d.TrangThai == "Đã xác nhận");
            int choConLai = Math.Max(0, lop.SoLuongToiDa - soHocVienHienTai);

            return new
            {
                lop.MaLop,
                lop.TenLop,
                TenKhoaHoc = lop.KhoaHoc?.TenKhoaHoc ?? "Chưa gán",
                TenGiaoVien = lop.GiaoVien?.HoTen ?? "Chưa phân công",
                lop.LichHoc,
                lop.PhongHoc,
                lop.NgayKhaiGiang,
                lop.HocPhi,
                lop.TrangThai,
                lop.SoLuongToiDa,
                SoHocVienHienTai = soHocVienHienTai,
                ChoConLai = choConLai,
                ConCho = choConLai > 0
            };
        }

        /// <summary>
        /// Câu 3: Kiểm tra xem lớp học có còn chỗ trống để đăng ký hay không.
        /// Sử dụng: Any, Count
        /// </summary>
        public async Task<bool> Q3_KiemTraLopHocConChoTrongAsync(int maLop)
        {
            var lop = await _context.LopHocs
                .Include(l => l.DangKyHocs)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);

            if (lop == null) return false;

            int daDangKy = lop.DangKyHocs.Count(d => d.TrangThai == "Đã xác nhận");
            return daDangKy < lop.SoLuongToiDa;
        }

        /// <summary>
        /// Câu 4: Lấy danh sách tất cả các lớp học do một giáo viên cụ thể phụ trách.
        /// Sử dụng: Where, Include, OrderBy
        /// </summary>
        public async Task<List<LopHoc>> Q4_DanhSachLopHocTheoGiaoVienAsync(int maGiaoVien)
        {
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Where(l => l.MaGiaoVien == maGiaoVien)
                .OrderBy(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 5: Lấy danh sách các lớp học thuộc một khóa học xác định.
        /// Sử dụng: Where, Include
        /// </summary>
        public async Task<List<LopHoc>> Q5_DanhSachLopHocTheoKhoaHocAsync(int maKhoaHoc)
        {
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.GiaoVien)
                .Where(l => l.MaKhoaHoc == maKhoaHoc)
                .OrderByDescending(l => l.MaLop)
                .ToListAsync();
        }

        #endregion

        #region =================== PHẦN 2: TÌM KIẾM VÀ LỌC LỚP HỌC ===================

        /// <summary>
        /// Câu 6: Tìm kiếm lớp học tổng hợp đa trường (Tên lớp, Tên khóa học, Tên giáo viên, Phòng học, Lịch học).
        /// Sử dụng: Where, Contains, ToLower
        /// </summary>
        public async Task<List<LopHoc>> Q6_TimKiemLopHocTongHopAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await Q1_LayDanhSachLopHocDayDuAsync();

            tuKhoa = tuKhoa.Trim().ToLower();

            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Where(l => l.TenLop.ToLower().Contains(tuKhoa)
                         || (l.KhoaHoc != null && l.KhoaHoc.TenKhoaHoc.ToLower().Contains(tuKhoa))
                         || (l.GiaoVien != null && l.GiaoVien.HoTen.ToLower().Contains(tuKhoa))
                         || l.PhongHoc.ToLower().Contains(tuKhoa)
                         || l.LichHoc.ToLower().Contains(tuKhoa))
                .OrderByDescending(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 7: Lọc lớp học theo trạng thái ("Đang tuyển sinh", "Đang học", "Đã kết thúc").
        /// Sử dụng: Where
        /// </summary>
        public async Task<List<LopHoc>> Q7_LocLopHocTheoTrangThaiAsync(string trangThai)
        {
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Where(l => l.TrangThai == trangThai)
                .OrderBy(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 8: Lọc các lớp học sắp khai giảng trong khoảng thời gian N ngày tới tính từ thời điểm hiện tại.
        /// Sử dụng: Where, DateTime so sánh
        /// </summary>
        public async Task<List<LopHoc>> Q8_LocLopHocSapKhaiGiangAsync(int soNgayToi = 30)
        {
            var homNay = DateTime.Today;
            var denNgay = homNay.AddDays(soNgayToi);

            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Where(l => l.NgayKhaiGiang >= homNay && l.NgayKhaiGiang <= denNgay && l.TrangThai == "Đang tuyển sinh")
                .OrderBy(l => l.NgayKhaiGiang)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 9: Lọc lớp học theo khoảng học phí (Từ giá Min đến Max).
        /// Sử dụng: Where kép
        /// </summary>
        public async Task<List<LopHoc>> Q9_LocLopHocTheoKhoangHocPhiAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Where(l => l.HocPhi >= minPrice && l.HocPhi <= maxPrice)
                .OrderBy(l => l.HocPhi)
                .ToListAsync();
        }

        #endregion

        #region ================== PHẦN 3: SẮP XẾP VÀ PHÂN TRANG ==================

        /// <summary>
        /// Câu 10: Sắp xếp danh sách lớp học theo nhiều tiêu chuẩn khác nhau.
        /// Sử dụng: Switch Expression kết hợp OrderBy / OrderByDescending
        /// </summary>
        public IQueryable<LopHoc> Q10_SapXepLopHoc(IQueryable<LopHoc> query, string? sortBy)
        {
            return sortBy switch
            {
                "ten_desc" => query.OrderByDescending(l => l.TenLop),
                "ngay_asc" => query.OrderBy(l => l.NgayKhaiGiang),
                "ngay_desc" => query.OrderByDescending(l => l.NgayKhaiGiang),
                "hocphi_asc" => query.OrderBy(l => l.HocPhi),
                "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
                "siso_desc" => query.OrderByDescending(l => l.SoLuongToiDa),
                _ => query.OrderBy(l => l.TenLop) // Mặc định tên A-Z
            };
        }

        /// <summary>
        /// Câu 11: Phân trang danh sách lớp học cơ bản.
        /// Sử dụng: Skip, Take, CountAsync
        /// </summary>
        public async Task<(List<LopHoc> Items, int TotalCount, int TotalPages)> Q11_PhanTrangLopHocAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .OrderByDescending(l => l.NgayKhaiGiang);

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalPages);
        }

        /// <summary>
        /// Câu 12: PIPELINE TOÀN DIỆN KẾT HỢP: Tìm kiếm + Lọc đa tiêu chí + Sắp xếp + Phân trang.
        /// Phương thức mẫu chuẩn mực ứng dụng LINQ tối ưu hiệu suất thực tế.
        /// </summary>
        public async Task<(List<LopHoc> Items, int TotalCount, int TotalPages)> Q12_PipelineTimKiemLocSapXepPhanTrangAsync(
            string? tuKhoa,
            int? maKhoaHoc,
            int? maGiaoVien,
            string? trangThai,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _context.LopHocs
                .AsNoTracking()
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .AsQueryable();

            // 1. Tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim().ToLower();
                query = query.Where(l => l.TenLop.ToLower().Contains(tuKhoa)
                                      || (l.KhoaHoc != null && l.KhoaHoc.TenKhoaHoc.ToLower().Contains(tuKhoa))
                                      || (l.GiaoVien != null && l.GiaoVien.HoTen.ToLower().Contains(tuKhoa))
                                      || l.PhongHoc.ToLower().Contains(tuKhoa));
            }

            // 2. Lọc (Filter)
            if (maKhoaHoc.HasValue && maKhoaHoc > 0)
                query = query.Where(l => l.MaKhoaHoc == maKhoaHoc.Value);

            if (maGiaoVien.HasValue && maGiaoVien > 0)
                query = query.Where(l => l.MaGiaoVien == maGiaoVien.Value);

            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(l => l.TrangThai == trangThai);

            if (minPrice.HasValue)
                query = query.Where(l => l.HocPhi >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(l => l.HocPhi <= maxPrice.Value);

            // 3. Đếm tổng số bản ghi thỏa mãn điều kiện lọc
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 4. Sắp xếp (Sorting)
            query = Q10_SapXepLopHoc(query, sortBy);

            // 5. Phân trang (Paging)
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalPages);
        }

        #endregion
    }
}
