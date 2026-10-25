using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.LINQ.Module1
{
    /// <summary>
    /// PHÂN CHIA CHỨC NĂNG: VŨ TRƯỜNG AN - MODULE 1
    /// NHIỆM VỤ: Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học
    /// </summary>
    public class LINQ_Module1_TaiKhoan_KhoaHoc
    {
        private readonly TrungTamDbContext _context;

        public LINQ_Module1_TaiKhoan_KhoaHoc(TrungTamDbContext context)
        {
            _context = context;
        }

        #region ======================= PHẦN 1: QUẢN LÝ TÀI KHOẢN =======================

        /// <summary>
        /// Câu 1: Lấy danh sách tất cả tài khoản đang hoạt động trong hệ thống kèm thông tin giáo viên/học viên.
        /// Sử dụng: Where, Include, AsNoTracking, ToListAsync
        /// </summary>
        public async Task<List<TaiKhoan>> Q1_LayTatCaTaiKhoanHoatDongAsync()
        {
            // Method Syntax (Lambda)
            return await _context.TaiKhoans
                .AsNoTracking()
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .Where(t => t.TrangThai == "HoatDong")
                .OrderBy(t => t.HoTen)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 1 (Query Syntax): Viết lại bằng cú pháp truy vấn giống SQL
        /// </summary>
        public List<TaiKhoan> Q1_QuerySyntax_LayTatCaTaiKhoanHoatDong()
        {
            var query = from tk in _context.TaiKhoans
                        where tk.TrangThai == "HoatDong"
                        orderby tk.HoTen ascending
                        select tk;
            return query.ToList();
        }

        /// <summary>
        /// Câu 2: Tìm kiếm tài khoản theo từ khóa (khớp tên đăng nhập, họ tên hoặc email).
        /// Sử dụng: Where, Contains, ToLower
        /// </summary>
        public async Task<List<TaiKhoan>> Q2_TimKiemTaiKhoanAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await _context.TaiKhoans.ToListAsync();

            tuKhoa = tuKhoa.Trim().ToLower();

            return await _context.TaiKhoans
                .AsNoTracking()
                .Where(t => t.TenDangNhap.ToLower().Contains(tuKhoa)
                         || t.HoTen.ToLower().Contains(tuKhoa)
                         || t.Email.ToLower().Contains(tuKhoa))
                .OrderBy(t => t.TenDangNhap)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 3: Lọc tài khoản theo vai trò cụ thể (Admin, GiaoVien, HocVien).
        /// Sử dụng: Where, OrderBy
        /// </summary>
        public async Task<List<TaiKhoan>> Q3_LocTaiKhoanTheoVaiTroAsync(string vaiTro)
        {
            return await _context.TaiKhoans
                .AsNoTracking()
                .Where(t => t.VaiTro.Equals(vaiTro, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.MaTaiKhoan)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 4: Lấy chi tiết tài khoản theo ID kèm hồ sơ học viên hoặc giáo viên (Eager Loading).
        /// Sử dụng: Include, FirstOrDefaultAsync
        /// </summary>
        public async Task<TaiKhoan?> Q4_LayChiTietTaiKhoanAsync(int maTaiKhoan)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);
        }

        /// <summary>
        /// Câu 5: Kiểm tra xem tên đăng nhập đã được sử dụng hay chưa (dùng khi đăng ký tài khoản mới).
        /// Sử dụng: AnyAsync (tối ưu hiệu năng, dừng ngay khi tìm thấy bản ghi đầu tiên)
        /// </summary>
        public async Task<bool> Q5_KiemTraTenDangNhapTonTaiAsync(string tenDangNhap)
        {
            return await _context.TaiKhoans
                .AnyAsync(t => t.TenDangNhap.ToLower() == tenDangNhap.Trim().ToLower());
        }

        /// <summary>
        /// Câu 6: Kiểm tra địa chỉ Email đã tồn tại trong hệ thống hay chưa.
        /// Sử dụng: AnyAsync
        /// </summary>
        public async Task<bool> Q6_KiemTraEmailTonTaiAsync(string email)
        {
            return await _context.TaiKhoans
                .AnyAsync(t => t.Email.ToLower() == email.Trim().ToLower());
        }

        #endregion

        #region ================== PHẦN 2: ĐĂNG NHẬP VÀ PHÂN QUYỀN ==================

        /// <summary>
        /// Câu 7: Xác thực đăng nhập hệ thống (kiểm tra username, password và trạng thái tài khoản không bị khóa).
        /// Sử dụng: SingleOrDefaultAsync hoặc FirstOrDefaultAsync
        /// </summary>
        public async Task<TaiKhoan?> Q7_XacThucDangNhapAsync(string tenDangNhap, string matKhau)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap
                                       && t.MatKhau == matKhau
                                       && t.TrangThai == "HoatDong");
        }

        /// <summary>
        /// Câu 8: Lấy thông tin phân quyền (Role) của người dùng hiện tại dựa trên mã tài khoản.
        /// Sử dụng: Select (Projection) để chỉ lấy cột VaiTro cần thiết, giảm tải truyền dữ liệu.
        /// </summary>
        public async Task<string?> Q8_LayVaiTroNguoiDungAsync(int maTaiKhoan)
        {
            return await _context.TaiKhoans
                .Where(t => t.MaTaiKhoan == maTaiKhoan)
                .Select(t => t.VaiTro)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Câu 9: Thống kê số lượng người dùng theo từng vai trò trong hệ thống (Admin, Giáo viên, Học viên).
        /// Sử dụng: GroupBy, Select, Count, Anonymous Type / DTO
        /// </summary>
        public async Task<List<object>> Q9_ThongKeTaiKhoanTheoVaiTroAsync()
        {
            var result = await _context.TaiKhoans
                .GroupBy(t => t.VaiTro)
                .Select(g => new
                {
                    VaiTro = g.Key,
                    SoLuong = g.Count(),
                    SoLuongHoatDong = g.Count(x => x.TrangThai == "HoatDong"),
                    SoLuongBiKhoa = g.Count(x => x.TrangThai == "Khoa")
                })
                .ToListAsync<object>();

            return result;
        }

        /// <summary>
        /// Câu 9 (Query Syntax): Viết lại bằng cú pháp truy vấn giống SQL
        /// </summary>
        public List<object> Q9_QuerySyntax_ThongKeTaiKhoanTheoVaiTro()
        {
            var query = from tk in _context.TaiKhoans
                        group tk by tk.VaiTro into g
                        select new
                        {
                            VaiTro = g.Key,
                            TongSoTaiKhoan = g.Count(),
                            HoatDong = g.Count(x => x.TrangThai == "HoatDong")
                        };
            return query.ToList<object>();
        }

        /// <summary>
        /// Câu 10: Lấy danh sách các tài khoản đang bị khóa để quản trị viên xem xét mở khóa.
        /// Sử dụng: Where, OrderByDescending
        /// </summary>
        public async Task<List<TaiKhoan>> Q10_DanhSachTaiKhoanBiKhoaAsync()
        {
            return await _context.TaiKhoans
                .Where(t => t.TrangThai == "Khoa")
                .OrderByDescending(t => t.MaTaiKhoan)
                .ToListAsync();
        }

        #endregion

        #region ======================= PHẦN 3: QUẢN LÝ KHÓA HỌC =======================

        /// <summary>
        /// Câu 11: Lấy danh sách tất cả các khóa học đang mở tuyển sinh.
        /// Sử dụng: Where, OrderBy
        /// </summary>
        public async Task<List<KhoaHoc>> Q11_LayDanhSachKhoaHocDangMoAsync()
        {
            return await _context.KhoaHocs
                .AsNoTracking()
                .Where(k => k.TrangThai == "Đang mở")
                .OrderBy(k => k.TenKhoaHoc)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 12: Tìm kiếm khóa học đa tiêu chí: Từ khóa, Ngoại ngữ, Cấp độ.
        /// Sử dụng: Dynamic LINQ Chaining (IQueryable)
        /// </summary>
        public async Task<List<KhoaHoc>> Q12_TimKiemKhoaHocDaTieuChiAsync(string? tuKhoa, string? ngoaiNgu, string? capDo)
        {
            var query = _context.KhoaHocs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim().ToLower();
                query = query.Where(k => k.TenKhoaHoc.ToLower().Contains(tuKhoa)
                                      || (k.MoTa != null && k.MoTa.ToLower().Contains(tuKhoa)));
            }

            if (!string.IsNullOrWhiteSpace(ngoaiNgu))
            {
                query = query.Where(k => k.NgoaiNgu == ngoaiNgu.Trim());
            }

            if (!string.IsNullOrWhiteSpace(capDo))
            {
                query = query.Where(k => k.CapDo == capDo.Trim());
            }

            return await query.OrderBy(k => k.TenKhoaHoc).ToListAsync();
        }

        /// <summary>
        /// Câu 13: Lọc khóa học theo khoảng học phí (Từ giá Min đến giá Max).
        /// Sử dụng: Where kết hợp điều kiện kép
        /// </summary>
        public async Task<List<KhoaHoc>> Q13_LocKhoaHocTheoHocPhiAsync(decimal giaMin, decimal giaMax)
        {
            return await _context.KhoaHocs
                .AsNoTracking()
                .Where(k => k.HocPhi >= giaMin && k.HocPhi <= giaMax)
                .OrderBy(k => k.HocPhi)
                .ToListAsync();
        }

        /// <summary>
        /// Câu 14: Sắp xếp danh sách khóa học linh hoạt (theo Tên A-Z, Tên Z-A, Học phí tăng dần, Học phí giảm dần).
        /// Sử dụng: OrderBy, OrderByDescending
        /// </summary>
        public async Task<List<KhoaHoc>> Q14_SapXepKhoaHocAsync(string? kieuSapXep)
        {
            var query = _context.KhoaHocs.AsNoTracking();

            query = kieuSapXep switch
            {
                "ten_desc" => query.OrderByDescending(k => k.TenKhoaHoc),
                "gia_asc" => query.OrderBy(k => k.HocPhi),
                "gia_desc" => query.OrderByDescending(k => k.HocPhi),
                "sobuoi_desc" => query.OrderByDescending(k => k.SoBuoi),
                _ => query.OrderBy(k => k.TenKhoaHoc) // Mặc định A-Z
            };

            return await query.ToListAsync();
        }

        /// <summary>
        /// Câu 15: Lấy thông tin chi tiết một khóa học kèm theo tất cả các lớp học trực thuộc và các bình luận đánh giá.
        /// Sử dụng: Include, ThenInclude, FirstOrDefaultAsync
        /// </summary>
        public async Task<KhoaHoc?> Q15_LayChiTietKhoaHocKiemLopHocAsync(int maKhoaHoc)
        {
            return await _context.KhoaHocs
                .Include(k => k.LopHocs)
                    .ThenInclude(l => l.GiaoVien)
                .Include(k => k.BinhLuans)
                    .ThenInclude(b => b.TaiKhoan)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);
        }

        /// <summary>
        /// Câu 16: Thống kê số lượng khóa học và học phí trung bình theo từng ngôn ngữ đào tạo (Tiếng Anh, Tiếng Trung, Tiếng Nhật, Tiếng Hàn).
        /// Sử dụng: GroupBy, Count, Average, Min, Max
        /// </summary>
        public async Task<List<object>> Q16_ThongKeKhoaHocTheoNgoaiNguAsync()
        {
            var result = await _context.KhoaHocs
                .GroupBy(k => k.NgoaiNgu)
                .Select(g => new
                {
                    NgoaiNgu = g.Key,
                    TongSoKhoaHoc = g.Count(),
                    HocPhiTrungBinh = g.Average(k => k.HocPhi),
                    HocPhiThapNhat = g.Min(k => k.HocPhi),
                    HocPhiCaoNhat = g.Max(k => k.HocPhi),
                    TongSoBuoiTrungBinh = g.Average(k => k.SoBuoi)
                })
                .ToListAsync<object>();

            return result;
        }

        /// <summary>
        /// Câu 17: Lấy Top N khóa học có học phí cao nhất (khóa học chất lượng cao / chuyên sâu).
        /// Sử dụng: OrderByDescending, Take
        /// </summary>
        public async Task<List<KhoaHoc>> Q17_TopKhoaHocHocPhiCaoNhatAsync(int top)
        {
            return await _context.KhoaHocs
                .AsNoTracking()
                .OrderByDescending(k => k.HocPhi)
                .Take(top)
                .ToListAsync();
        }

        #endregion
    }
}
