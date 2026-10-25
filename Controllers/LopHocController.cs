// ============================================================================
// [MODULE 2 - SINH VIÊN 2: QUẢN LÝ LỚP HỌC – TÌM KIẾM – LỌC – SẮP XẾP – PHÂN TRANG]
// Tên Tệp: LopHocController.cs
// Mô tả: Chức năng: Quản lý lớp học (Thêm/Sửa/Xóa), Tìm kiếm theo tên/mã, Lọc theo giáo viên/trạng thái, Sắp xếp học phí/ngày, Phân trang
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
    public class LopHocController : Controller
    {
        private readonly TrungTamDbContext _context;

        public LopHocController(TrungTamDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. DANH SÁCH LỚP HỌC (SEARCH, FILTER, SORT, PAGINATION)
        // =========================================================================
        // GET: /LopHoc/Index
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] LopHocFilterViewModel filter)
        {
            // BƯỚC 1: Khởi tạo Queryable
            var query = _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Include(l => l.DangKyHocs)
                .AsQueryable();

            // BƯỚC 2: Tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(l => l.TenLop.Contains(term) ||
                                         (l.KhoaHoc != null && l.KhoaHoc.TenKhoaHoc.Contains(term)) ||
                                         l.PhongHoc.Contains(term));
            }

            // BƯỚC 3: Bộ lọc (Filter)
            if (filter.MaKhoaHoc.HasValue && filter.MaKhoaHoc.Value > 0)
            {
                query = query.Where(l => l.MaKhoaHoc == filter.MaKhoaHoc.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.NgoaiNgu))
            {
                query = query.Where(l => l.KhoaHoc != null && l.KhoaHoc.NgoaiNgu == filter.NgoaiNgu);
            }

            if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            {
                query = query.Where(l => l.TrangThai == filter.TrangThai);
            }

            if (filter.HocPhiTu.HasValue)
            {
                query = query.Where(l => l.HocPhi >= filter.HocPhiTu.Value);
            }

            if (filter.HocPhiDen.HasValue)
            {
                query = query.Where(l => l.HocPhi <= filter.HocPhiDen.Value);
            }

            // BƯỚC 4: 4.9. Sắp xếp dữ liệu (Sort) theo 6 tiêu chí
            query = filter.SortOrder switch
            {
                "ten_asc" => query.OrderBy(l => l.TenLop),
                "ten_desc" => query.OrderByDescending(l => l.TenLop),
                "hocphi_asc" => query.OrderBy(l => l.HocPhi),
                "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
                "ngaytao_desc" => query.OrderByDescending(l => l.MaLop),
                "ngaytao_asc" => query.OrderBy(l => l.MaLop),
                "ngay_asc" => query.OrderBy(l => l.NgayKhaiGiang),
                "ngay_desc" => query.OrderByDescending(l => l.NgayKhaiGiang),
                _ => query.OrderByDescending(l => l.MaLop) // Mặc định: Ngày tạo mới nhất
            };

            // BƯỚC 5: Đếm tổng số bản ghi thỏa điều kiện
            filter.TotalItems = await query.CountAsync();

            // Đảm bảo PageIndex hợp lệ
            if (filter.PageIndex < 1) filter.PageIndex = 1;

            // BƯỚC 6: Phân trang qua LINQ EF Core (Skip & Take) - Không tải toàn bộ về browser
            filter.Items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Nạp dữ liệu cho các dropdown SelectList
            var khoaHocs = await _context.KhoaHocs.OrderBy(k => k.TenKhoaHoc).ToListAsync();
            filter.KhoaHocList = new SelectList(khoaHocs, "MaKhoaHoc", "TenKhoaHoc", filter.MaKhoaHoc);

            var ngoaiNgus = new List<string> { "Tiếng Anh", "Tiếng Hàn", "Tiếng Nhật", "Tiếng Trung" };
            filter.NgoaiNguList = new SelectList(ngoaiNgus, filter.NgoaiNgu);

            var trangThais = new List<string> { "Đang tuyển sinh", "Đang học", "Đã kết thúc", "Đã hủy" };
            filter.TrangThaiList = new SelectList(trangThais, filter.TrangThai);

            var sortOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "ngaytao_desc", Text = "• Ngày tạo mới nhất" },
                new SelectListItem { Value = "ngaytao_asc", Text = "• Ngày tạo cũ nhất" },
                new SelectListItem { Value = "ten_asc", Text = "• Tên lớp tăng dần" },
                new SelectListItem { Value = "ten_desc", Text = "• Tên lớp giảm dần" },
                new SelectListItem { Value = "hocphi_asc", Text = "• Học phí tăng dần" },
                new SelectListItem { Value = "hocphi_desc", Text = "• Học phí giảm dần" }
            };
            filter.SortOrderList = new SelectList(sortOptions, "Value", "Text", filter.SortOrder ?? "ngaytao_desc");

            return View(filter);
        }

        // =========================================================================
        // 2. CHI TIẾT LỚP HỌC (Xem và Kiểm tra ràng buộc đăng ký)
        // =========================================================================
        // GET: /LopHoc/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lopHoc = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Include(l => l.DangKyHocs)
                    .ThenInclude(d => d.HocVien)
                .FirstOrDefaultAsync(m => m.MaLop == id);

            if (lopHoc == null) return NotFound();

            var session = HttpContext.Session;
            var vaiTro = session.GetString("VaiTro");
            var maHocVien = session.GetInt32("MaHocVien");
            bool isLoggedIn = !string.IsNullOrEmpty(vaiTro);
            bool isStudent = vaiTro == "HocVien";

            // Đếm số lượng học viên đã đăng ký hợp lệ (chờ duyệt hoặc đã duyệt)
            int soLuongDaDangKy = lopHoc.DangKyHocs.Count(d => d.TrangThai != "Đã hủy");

            bool alreadyRegistered = false;
            if (isStudent && maHocVien.HasValue)
            {
                alreadyRegistered = lopHoc.DangKyHocs.Any(d => d.MaHocVien == maHocVien.Value && d.TrangThai != "Đã hủy");
            }

            // Kiểm tra các ràng buộc theo đề bài:
            bool canRegister = true;
            string? reasonCannotRegister = null;

            if (!isLoggedIn)
            {
                canRegister = false;
                reasonCannotRegister = "Vui lòng đăng nhập với tài khoản học viên để đăng ký lớp học này.";
            }
            else if (!isStudent)
            {
                canRegister = false;
                reasonCannotRegister = "Tài khoản của bạn là Quản trị viên, không thể đăng ký học viên.";
            }
            else if (lopHoc.TrangThai != "Đang tuyển sinh")
            {
                canRegister = false;
                reasonCannotRegister = "Lớp không còn tuyển sinh.";
            }
            else if (alreadyRegistered)
            {
                canRegister = false;
                reasonCannotRegister = "Bạn đã đăng ký lớp này.";
            }
            else if (soLuongDaDangKy >= lopHoc.SoLuongToiDa)
            {
                canRegister = false;
                reasonCannotRegister = "Lớp đã đủ số lượng.";
            }

            var viewModel = new LopHocDetailsViewModel
            {
                LopHoc = lopHoc,
                SoLuongDaDangKy = soLuongDaDangKy,
                CanRegister = canRegister,
                ReasonCannotRegister = reasonCannotRegister,
                AlreadyRegistered = alreadyRegistered,
                IsStudent = isStudent,
                IsLoggedIn = isLoggedIn
            };

            return View(viewModel);
        }

        // =========================================================================
        // 3. ADMIN: THÊM LỚP HỌC MỚI
        // =========================================================================
        // GET: /LopHoc/Create
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(int? maKhoaHoc)
        {
            var khoaHocs = await _context.KhoaHocs.Where(k => k.TrangThai == "Đang mở").ToListAsync();
            ViewBag.MaKhoaHoc = new SelectList(khoaHocs, "MaKhoaHoc", "TenKhoaHoc", maKhoaHoc);

            var teachers = await _context.GiaoViens.OrderBy(g => g.HoTen).ToListAsync();
            ViewBag.MaGiaoVien = new SelectList(teachers, "MaGiaoVien", "HoTen");

            var defaultHocPhi = 0m;
            if (maKhoaHoc.HasValue)
            {
                var kh = khoaHocs.FirstOrDefault(k => k.MaKhoaHoc == maKhoaHoc.Value);
                if (kh != null) defaultHocPhi = kh.HocPhi;
            }

            var model = new LopHoc
            {
                MaKhoaHoc = maKhoaHoc ?? (khoaHocs.FirstOrDefault()?.MaKhoaHoc ?? 0),
                NgayKhaiGiang = DateTime.Today.AddDays(7),
                LichHoc = "Tối 2-4-6 (18:30 - 20:00)",
                PhongHoc = "Phòng A.101",
                SoLuongToiDa = 25,
                HocPhi = defaultHocPhi,
                TrangThai = "Đang tuyển sinh"
            };

            return View(model);
        }

        // POST: /LopHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(LopHoc model)
        {
            if (ModelState.IsValid)
            {
                _context.LopHocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã mở thành công lớp học '{model.TenLop}'!";
                return RedirectToAction(nameof(Index));
            }

            var khoaHocs = await _context.KhoaHocs.Where(k => k.TrangThai == "Đang mở").ToListAsync();
            ViewBag.MaKhoaHoc = new SelectList(khoaHocs, "MaKhoaHoc", "TenKhoaHoc", model.MaKhoaHoc);
            var teachers = await _context.GiaoViens.OrderBy(g => g.HoTen).ToListAsync();
            ViewBag.MaGiaoVien = new SelectList(teachers, "MaGiaoVien", "HoTen", model.MaGiaoVien);
            return View(model);
        }

        // =========================================================================
        // 4. ADMIN: CHỈNH SỬA LỚP HỌC
        // =========================================================================
        // GET: /LopHoc/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc == null) return NotFound();

            var khoaHocs = await _context.KhoaHocs.ToListAsync();
            ViewBag.MaKhoaHoc = new SelectList(khoaHocs, "MaKhoaHoc", "TenKhoaHoc", lopHoc.MaKhoaHoc);
            var teachers = await _context.GiaoViens.OrderBy(g => g.HoTen).ToListAsync();
            ViewBag.MaGiaoVien = new SelectList(teachers, "MaGiaoVien", "HoTen", lopHoc.MaGiaoVien);

            return View(lopHoc);
        }

        // POST: /LopHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, LopHoc model)
        {
            if (id != model.MaLop) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Cập nhật thông tin lớp '{model.TenLop}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocExists(model.MaLop)) return NotFound();
                    throw;
                }
            }

            var khoaHocs = await _context.KhoaHocs.ToListAsync();
            ViewBag.MaKhoaHoc = new SelectList(khoaHocs, "MaKhoaHoc", "TenKhoaHoc", model.MaKhoaHoc);
            return View(model);
        }

        // =========================================================================
        // 5. ADMIN: XÓA LỚP HỌC
        // =========================================================================
        // GET: /LopHoc/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lopHoc = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.DangKyHocs)
                .FirstOrDefaultAsync(m => m.MaLop == id);

            if (lopHoc == null) return NotFound();

            return View(lopHoc);
        }

        // POST: /LopHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHoc = await _context.LopHocs
                .Include(l => l.DangKyHocs)
                .FirstOrDefaultAsync(l => l.MaLop == id);

            if (lopHoc != null)
            {
                if (lopHoc.DangKyHocs.Any())
                {
                    TempData["Error"] = $"Không thể xóa lớp '{lopHoc.TenLop}' vì đã có {lopHoc.DangKyHocs.Count} lượt đăng ký liên kết!";
                    return RedirectToAction(nameof(Index));
                }

                _context.LopHocs.Remove(lopHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa lớp học '{lopHoc.TenLop}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LopHocExists(int id)
        {
            return _context.LopHocs.Any(e => e.MaLop == id);
        }
    }
}
