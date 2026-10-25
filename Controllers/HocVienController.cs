// ============================================================================
// [MODULE 3 - SINH VIÊN 3: QUẢN LÝ HỌC VIÊN – ĐĂNG KÝ LỚP HỌC – LỊCH SỬ ĐĂNG KÝ]
// Tên Tệp: HocVienController.cs
// Mô tả: Chức năng: Quản lý hồ sơ học viên (Họ tên, SĐT, Email, Địa chỉ), Thêm, Chỉnh sửa thông tin, Xóa học viên
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;

namespace TrungTamNgoaiNgu.Controllers
{
    [AuthorizeRole("Admin")]
    public class HocVienController : Controller
    {
        private readonly TrungTamDbContext _context;

        public HocVienController(TrungTamDbContext context)
        {
            _context = context;
        }

        // GET: /HocVien/Index
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(h => h.HoTen.Contains(searchString) ||
                                         h.SoDienThoai.Contains(searchString) ||
                                         h.Email.Contains(searchString) ||
                                         (h.DiaChi != null && h.DiaChi.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;
            var list = await query.OrderByDescending(h => h.MaHocVien).ToListAsync();
            return View(list);
        }

        // GET: /HocVien/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l!.KhoaHoc)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);

            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // =========================================================================
        // 4.14. CÁC CHỨC NĂNG QUẢN LÝ HỌC VIÊN: THÊM, SỬA, XÓA
        // =========================================================================

        // GET: /HocVien/Create (Thêm học viên)
        [HttpGet]
        public IActionResult Create()
        {
            var model = new TrungTamNgoaiNgu.ViewModels.CreateHocVienViewModel();
            return View(model);
        }

        // POST: /HocVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrungTamNgoaiNgu.ViewModels.CreateHocVienViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng tên đăng nhập
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower()))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng!");
                return View(model);
            }

            // 1. Tạo tài khoản cho học viên
            var taiKhoan = new Models.TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = model.MatKhau.Trim(),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                VaiTro = "HocVien",
                TrangThai = "HoatDong"
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // 2. Tạo bản ghi học viên
            var hocVien = new Models.HocVien
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen.Trim(),
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                SoDienThoai = model.SoDienThoai.Trim(),
                Email = model.Email.Trim(),
                DiaChi = model.DiaChi?.Trim()
            };

            _context.HocViens.Add(hocVien);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Thêm học viên '{hocVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /HocVien/Edit/5 (Sửa học viên)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // POST: /HocVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.HocVien model)
        {
            if (id != model.MaHocVien) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            hocVien.HoTen = model.HoTen.Trim();
            hocVien.NgaySinh = model.NgaySinh;
            hocVien.GioiTinh = model.GioiTinh;
            hocVien.SoDienThoai = model.SoDienThoai.Trim();
            hocVien.Email = model.Email.Trim();
            hocVien.DiaChi = model.DiaChi?.Trim();

            // Đồng bộ tên và email với tài khoản nếu có
            if (hocVien.TaiKhoan != null)
            {
                hocVien.TaiKhoan.HoTen = hocVien.HoTen;
                hocVien.TaiKhoan.Email = hocVien.Email;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cập nhật thông tin học viên '{hocVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /HocVien/Delete/5 (Xóa học viên)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // POST: /HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien != null)
            {
                if (hocVien.DangKyHocs.Any())
                {
                    TempData["Error"] = $"Không thể xóa học viên '{hocVien.HoTen}' vì đã có {hocVien.DangKyHocs.Count} lượt đăng ký lớp học!";
                    return RedirectToAction(nameof(Index));
                }

                var taiKhoan = hocVien.TaiKhoan;

                _context.HocViens.Remove(hocVien);
                if (taiKhoan != null)
                {
                    _context.TaiKhoans.Remove(taiKhoan);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa học viên '{hocVien.HoTen}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
