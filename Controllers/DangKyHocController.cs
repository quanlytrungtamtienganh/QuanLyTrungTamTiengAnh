// ============================================================================
// [MODULE 3 - PHẠM THANH TÙNG: QUẢN LÝ HỌC VIÊN – ĐĂNG KÝ LỚP HỌC – LỊCH SỬ ĐĂNG KÝ]
// Tên Tệp: DangKyHocController.cs
// Mô tả: Chức năng: Đăng ký lớp học (Kiểm tra đăng nhập, lớp tuyển sinh, sỉ số, trùng lặp), Xem lịch sử đăng ký, Hủy đăng ký
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class DangKyHocController : Controller
    {
        private readonly TrungTamDbContext _context;

        public DangKyHocController(TrungTamDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. ADMIN: QUẢN LÝ TẤT CẢ LƯỢT ĐĂNG KÝ HỌC (Yêu cầu #9)
        // =========================================================================
        // GET: /DangKyHoc/Index
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index([FromQuery] DangKyFilterViewModel filter)
        {
            var query = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .AsQueryable();

            // Lọc theo tên học viên
            if (!string.IsNullOrWhiteSpace(filter.SearchHocVien))
            {
                var term = filter.SearchHocVien.Trim();
                query = query.Where(d => d.HocVien != null && (d.HocVien.HoTen.Contains(term) || d.HocVien.SoDienThoai.Contains(term)));
            }

            // Lọc theo lớp học
            if (filter.MaLop.HasValue && filter.MaLop.Value > 0)
            {
                query = query.Where(d => d.MaLop == filter.MaLop.Value);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            {
                query = query.Where(d => d.TrangThai == filter.TrangThai);
            }

            filter.Items = await query.OrderByDescending(d => d.NgayDangKy).ToListAsync();

            var lopHocs = await _context.LopHocs.OrderBy(l => l.TenLop).ToListAsync();
            filter.LopHocList = new SelectList(lopHocs, "MaLop", "TenLop", filter.MaLop);

            var trangThais = new List<string> { "Chờ xác nhận", "Đã xác nhận", "Đã hủy" };
            filter.TrangThaiList = new SelectList(trangThais, filter.TrangThai);

            return View(filter);
        }

        // =========================================================================
        // 2. ADMIN: XÁC NHẬN ĐĂNG KÝ (KIỂM TRA SĨ SỐ TỐI ĐA SoLuongToiDa - Yêu cầu #9)
        // =========================================================================
        // POST: /DangKyHoc/XacNhan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> XacNhan(int id)
        {
            var dangKy = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.DangKyHocs)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin lượt đăng ký!";
                return RedirectToAction(nameof(Index));
            }

            if (dangKy.TrangThai == "Đã xác nhận")
            {
                TempData["Warning"] = "Lượt đăng ký này đã được xác nhận trước đó!";
                return RedirectToAction(nameof(Index));
            }

            // Ràng buộc nghiệp vụ: Không được vượt quá SoLuongToiDa
            var lop = dangKy.LopHoc;
            if (lop != null)
            {
                int currentConfirmed = lop.DangKyHocs.Count(d => d.TrangThai == "Đã xác nhận");
                if (currentConfirmed >= lop.SoLuongToiDa)
                {
                    TempData["Error"] = $"Không thể xác nhận! Lớp học '{lop.TenLop}' đã đủ số lượng tối đa ({lop.SoLuongToiDa} học viên).";
                    return RedirectToAction(nameof(Index));
                }
            }

            dangKy.TrangThai = "Đã xác nhận";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xác nhận thành công cho học viên '{dangKy.HocVien?.HoTen}' vào lớp '{dangKy.LopHoc?.TenLop}'!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 3. ADMIN / HỌC VIÊN: HỦY ĐĂNG KÝ
        // =========================================================================
        // POST: /DangKyHoc/HuyDangKy/5 hoặc /DangKyHoc/Huy/5
        [HttpPost]
        [ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            return await HuyDangKy(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id)
        {
            var session = HttpContext.Session;
            var vaiTro = session.GetString("VaiTro");
            var maHocVien = session.GetInt32("MaHocVien");

            if (string.IsNullOrEmpty(vaiTro))
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var dangKy = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .Include(d => d.HocVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi đăng ký!";
                return RedirectToAction(vaiTro == "Admin" ? nameof(Index) : nameof(History));
            }

            // Nếu là học viên, chỉ được phép hủy đăng ký của chính mình và khi đang ở trạng thái 'Chờ xác nhận'
            if (vaiTro == "HocVien")
            {
                if (!maHocVien.HasValue || dangKy.MaHocVien != maHocVien.Value)
                {
                    TempData["Error"] = "Bạn không có quyền thao tác trên lượt đăng ký của người khác!";
                    return RedirectToAction(nameof(History));
                }

                if (dangKy.TrangThai == "Đã xác nhận")
                {
                    TempData["Error"] = "Lượt đăng ký đã được Ban Quản Trị xác nhận, không thể tự hủy. Vui lòng liên hệ trung tâm để được hỗ trợ.";
                    return RedirectToAction(nameof(History));
                }
            }

            dangKy.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã hủy lượt đăng ký lớp '{dangKy.LopHoc?.TenLop}' thành công!";
            return RedirectToAction(vaiTro == "Admin" ? nameof(Index) : nameof(History));
        }

        // =========================================================================
        // 4. HỌC VIÊN: ĐĂNG KÝ LỚP HỌC (KIỂM TRA 5 BƯỚC NGHIỆP VỤ - Yêu cầu #14)
        // =========================================================================
        // POST: /DangKyHoc/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("HocVien")]
        public async Task<IActionResult> DangKy(int maLop)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            // 1. Kiểm tra học viên tồn tại
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin học viên của bạn. Vui lòng cập nhật hồ sơ trước!";
                return RedirectToAction("Profile", "TaiKhoan");
            }

            // 2. Kiểm tra lớp học tồn tại
            var lopHoc = await _context.LopHocs
                .Include(l => l.DangKyHocs)
                .Include(l => l.KhoaHoc)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);

            if (lopHoc == null)
            {
                TempData["Error"] = "Lớp học không tồn tại trong hệ thống!";
                return RedirectToAction("Index", "LopHoc");
            }

            // 3. Kiểm tra lớp đang tuyển sinh
            if (lopHoc.TrangThai != "Đang tuyển sinh")
            {
                TempData["Error"] = "Lớp không còn tuyển sinh.";
                return RedirectToAction("Details", "LopHoc", new { id = maLop });
            }

            // 4. Kiểm tra học viên chưa đăng ký lớp này
            bool daDangKy = lopHoc.DangKyHocs.Any(d => d.MaHocVien == hocVien.MaHocVien && d.TrangThai != "Đã hủy");
            if (daDangKy)
            {
                TempData["Error"] = "Bạn đã đăng ký lớp này.";
                return RedirectToAction("Details", "LopHoc", new { id = maLop });
            }

            // 5. Kiểm tra lớp chưa đủ số lượng
            int soLuongHienTai = lopHoc.DangKyHocs.Count(d => d.TrangThai != "Đã hủy");
            if (soLuongHienTai >= lopHoc.SoLuongToiDa)
            {
                TempData["Error"] = "Lớp đã đủ số lượng.";
                return RedirectToAction("Details", "LopHoc", new { id = maLop });
            }

            // TẤT CẢ HỢP LỆ -> TẠO BẢN GHI ĐĂNG KÝ
            var dangKyMoi = new DangKyHoc
            {
                MaHocVien = hocVien.MaHocVien,
                MaLop = lopHoc.MaLop,
                NgayDangKy = DateTime.Now,
                HocPhi = lopHoc.HocPhi,
                TrangThai = "Chờ xác nhận"
            };

            _context.DangKyHocs.Add(dangKyMoi);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đăng ký lớp '{lopHoc.TenLop}' thành công! Trạng thái: Chờ xác nhận.";
            return RedirectToAction(nameof(ThanhCong), new { id = dangKyMoi.MaDangKy });
        }

        // =========================================================================
        // 4.16. MÀN HÌNH ĐĂNG KÝ THÀNH CÔNG (QUY TRÌNH 5 BƯỚC)
        // =========================================================================
        // GET: /DangKyHoc/ThanhCong/5
        [HttpGet]
        [AuthorizeRole("HocVien")]
        public async Task<IActionResult> ThanhCong(int id)
        {
            var dangKy = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.GiaoVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null) return NotFound();

            return View(dangKy);
        }

        // =========================================================================
        // 5. HỌC VIÊN: LỊCH SỬ ĐĂNG KÝ CỦA CHÍNH MÌNH (Yêu cầu #15)
        // =========================================================================
        // GET: /DangKyHoc/History
        [HttpGet]
        [AuthorizeRole("HocVien")]
        public async Task<IActionResult> History()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null)
            {
                return RedirectToAction("Profile", "TaiKhoan");
            }

            var history = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(history);
        }

        // =========================================================================
        // 6. ADMIN: XEM CHI TIẾT ĐĂNG KÝ
        // =========================================================================
        // GET: /DangKyHoc/Details/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _context.DangKyHocs
                .Include(d => d.HocVien)
                    .ThenInclude(h => h!.TaiKhoan)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.KhoaHoc)
                .FirstOrDefaultAsync(m => m.MaDangKy == id);

            if (dangKy == null) return NotFound();

            return View(dangKy);
        }
    }
}
