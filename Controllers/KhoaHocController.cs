// ============================================================================
// [MODULE 1 - SINH VIÊN 1: TÀI KHOẢN – ĐĂNG NHẬP VÀ PHÂN QUYỀN – QUẢN LÝ KHÓA HỌC]
// Tên Tệp: KhoaHocController.cs
// Mô tả: Chức năng: Quản lý danh mục khóa học ngoại ngữ, Thêm/Sửa/Xóa khóa học, Tìm kiếm & hiển thị khóa học
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly TrungTamDbContext _context;

        public KhoaHocController(TrungTamDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DÀNH CHO ADMIN (Quản lý Khóa Học)
        // ==========================================

        // GET: /KhoaHoc/Index
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.KhoaHocs
                .Include(k => k.LopHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(k => k.TenKhoaHoc.Contains(searchString) ||
                                         k.NgoaiNgu.Contains(searchString) ||
                                         k.CapDo.Contains(searchString));
            }

            ViewBag.SearchString = searchString;
            var list = await query.OrderByDescending(k => k.MaKhoaHoc).ToListAsync();
            return View(list);
        }

        // GET: /KhoaHoc/Create
        [HttpGet]
        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            var model = new KhoaHoc
            {
                TrangThai = "Đang mở",
                NgoaiNgu = "Tiếng Anh",
                CapDo = "Trung cấp (B1/B2)",
                SoBuoi = 24
            };
            return View(model);
        }

        // POST: /KhoaHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(KhoaHoc model)
        {
            if (ModelState.IsValid)
            {
                _context.KhoaHocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm thành công khóa học '{model.TenKhoaHoc}'!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /KhoaHoc/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, KhoaHoc model)
        {
            if (id != model.MaKhoaHoc) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Cập nhật khóa học '{model.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaHocExists(model.MaKhoaHoc)) return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        // GET: /KhoaHoc/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.LopHocs)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.LopHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc != null)
            {
                // Kiểm tra ràng buộc: nếu khóa học đã có lớp học thì không cho xóa trực tiếp
                if (khoaHoc.LopHocs.Any())
                {
                    TempData["Error"] = $"Không thể xóa khóa học '{khoaHoc.TenKhoaHoc}' vì đang có {khoaHoc.LopHocs.Count} lớp học liên kết!";
                    return RedirectToAction(nameof(Index));
                }

                _context.KhoaHocs.Remove(khoaHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 2. DÀNH CHO HỌC VIÊN & PUBLIC (Catalog & Details)
        // ==========================================

        // GET: /KhoaHoc/Catalog
        [HttpGet]
        public async Task<IActionResult> Catalog(string? ngoaiNgu, string? capDo, string? searchString)
        {
            var query = _context.KhoaHocs
                .Include(k => k.LopHocs)
                .Where(k => k.TrangThai == "Đang mở")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(k => k.TenKhoaHoc.Contains(searchString) || (k.MoTa != null && k.MoTa.Contains(searchString)));
            }

            if (!string.IsNullOrWhiteSpace(ngoaiNgu))
            {
                query = query.Where(k => k.NgoaiNgu == ngoaiNgu);
            }

            if (!string.IsNullOrWhiteSpace(capDo))
            {
                query = query.Where(k => k.CapDo == capDo);
            }

            ViewBag.CurrentNgoaiNgu = ngoaiNgu;
            ViewBag.CurrentCapDo = capDo;
            ViewBag.SearchString = searchString;

            var list = await query.OrderBy(k => k.TenKhoaHoc).ToListAsync();
            return View(list);
        }

        // GET: /KhoaHoc/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.LopHocs)
                    .ThenInclude(l => l.DangKyHocs)
                .Include(k => k.BinhLuans)
                    .ThenInclude(b => b.TaiKhoan)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        private bool KhoaHocExists(int id)
        {
            return _context.KhoaHocs.Any(e => e.MaKhoaHoc == id);
        }
    }
}
