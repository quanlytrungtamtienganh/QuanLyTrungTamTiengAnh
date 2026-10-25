// ============================================================================
// [MODULE 2 - TRẦN TRỌNG TRÁNG: QUẢN LÝ LỚP HỌC – TÌM KIẾM – LỌC – SẮP XẾP – PHÂN TRANG]
// Tên Tệp: GiaoVienController.cs
// Mô tả: Chức năng: Quản lý thông tin giảng viên, Bộ lọc tìm kiếm giáo viên phụ trách lớp học
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Controllers
{
    public class GiaoVienController : Controller
    {
        private readonly TrungTamDbContext _context;

        public GiaoVienController(TrungTamDbContext context)
        {
            _context = context;
        }

        // GET: /GiaoVien/Index (Danh sách giáo viên dành cho Admin quản lý - 4.8 Bộ lọc tìm kiếm GV)
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index(string? search, string? chuyenMon, string? hocVi, string? trangThaiDay)
        {
            var query = _context.GiaoViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                    .ThenInclude(l => l.KhoaHoc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(g => g.HoTen.Contains(search) || 
                                         (g.Email != null && g.Email.Contains(search)) || 
                                         (g.SoDienThoai != null && g.SoDienThoai.Contains(search)) ||
                                         (g.TaiKhoan != null && g.TaiKhoan.TenDangNhap.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(chuyenMon))
            {
                query = query.Where(g => g.ChuyenMon.Contains(chuyenMon));
            }

            if (!string.IsNullOrWhiteSpace(hocVi))
            {
                query = query.Where(g => g.HocVi != null && g.HocVi.Contains(hocVi));
            }

            if (trangThaiDay == "CoLop")
            {
                query = query.Where(g => g.LopHocs.Any());
            }
            else if (trangThaiDay == "ChuaCoLop")
            {
                query = query.Where(g => !g.LopHocs.Any());
            }

            var teachers = await query.OrderByDescending(g => g.MaGiaoVien).ToListAsync();

            ViewBag.Search = search;
            ViewBag.ChuyenMon = chuyenMon;
            ViewBag.HocVi = hocVi;
            ViewBag.TrangThaiDay = trangThaiDay;
            ViewBag.TotalCount = teachers.Count;
            ViewBag.AllTeachersCount = await _context.GiaoViens.CountAsync();
            ViewBag.WithClassesCount = await _context.GiaoViens.CountAsync(g => g.LopHocs.Any());
            ViewBag.NoClassesCount = await _context.GiaoViens.CountAsync(g => !g.LopHocs.Any());

            return View(teachers);
        }

        // GET: /GiaoVien/Create (Thêm giáo viên mới + tạo tài khoản)
        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /GiaoVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(
            string tenDangNhap,
            string matKhau,
            string hoTen,
            string chuyenMon,
            string? hocVi,
            string? soDienThoai,
            string? email,
            string? gioiThieu,
            string? anhDaiDien)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau) || string.IsNullOrWhiteSpace(hoTen))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ tên đăng nhập, mật khẩu và họ tên!");
                return View();
            }

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == tenDangNhap.Trim().ToLower()))
            {
                ModelState.AddModelError("tenDangNhap", "Tên đăng nhập này đã được sử dụng!");
                return View();
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap.Trim(),
                MatKhau = matKhau.Trim(),
                HoTen = hoTen.Trim(),
                Email = email?.Trim() ?? $"{tenDangNhap.Trim()}@educenter.edu.vn",
                VaiTro = "GiaoVien",
                TrangThai = "HoatDong"
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var giaoVien = new GiaoVien
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = hoTen.Trim(),
                ChuyenMon = chuyenMon?.Trim() ?? "Tiếng Anh",
                HocVi = hocVi?.Trim() ?? "Cử nhân Sư phạm",
                SoDienThoai = soDienThoai?.Trim(),
                Email = email?.Trim() ?? taiKhoan.Email,
                GioiThieu = gioiThieu?.Trim(),
                AnhDaiDien = string.IsNullOrWhiteSpace(anhDaiDien) 
                    ? "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80" 
                    : anhDaiDien.Trim()
            };

            _context.GiaoViens.Add(giaoVien);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Tạo thành công tài khoản giáo viên cho thầy/cô {giaoVien.HoTen}!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /GiaoVien/PhanLich (Phân lịch giảng dạy cho giáo viên)
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> PhanLich(int? maKhoaHoc, int? maGiaoVien)
        {
            var classesQuery = _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Include(l => l.DangKyHocs)
                .AsQueryable();

            if (maKhoaHoc.HasValue && maKhoaHoc > 0)
            {
                classesQuery = classesQuery.Where(l => l.MaKhoaHoc == maKhoaHoc.Value);
            }

            if (maGiaoVien.HasValue)
            {
                if (maGiaoVien.Value == 0) // Chưa phân công
                {
                    classesQuery = classesQuery.Where(l => l.MaGiaoVien == null);
                }
                else
                {
                    classesQuery = classesQuery.Where(l => l.MaGiaoVien == maGiaoVien.Value);
                }
            }

            var classes = await classesQuery.OrderBy(l => l.NgayKhaiGiang).ToListAsync();
            var teachers = await _context.GiaoViens.OrderBy(g => g.HoTen).ToListAsync();
            var courses = await _context.KhoaHocs.OrderBy(k => k.TenKhoaHoc).ToListAsync();

            ViewBag.Teachers = teachers;
            ViewBag.Courses = courses;
            ViewBag.SelectedKhoaHoc = maKhoaHoc;
            ViewBag.SelectedGiaoVien = maGiaoVien;

            return View(classes);
        }

        // POST: /GiaoVien/GanLop (Admin gán giáo viên phụ trách lớp học)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> GanLop(int maLop, int? maGiaoVien)
        {
            var lopHoc = await _context.LopHocs.FindAsync(maLop);
            if (lopHoc == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học yêu cầu!";
                return RedirectToAction(nameof(PhanLich));
            }

            if (maGiaoVien.HasValue && maGiaoVien.Value > 0)
            {
                var gv = await _context.GiaoViens.FindAsync(maGiaoVien.Value);
                if (gv != null)
                {
                    lopHoc.MaGiaoVien = gv.MaGiaoVien;
                    TempData["Success"] = $"Đã phân công giáo viên {gv.HoTen} phụ trách lớp {lopHoc.TenLop} thành công!";
                }
            }
            else
            {
                lopHoc.MaGiaoVien = null;
                TempData["Success"] = $"Đã hủy phân công giáo viên cho lớp {lopHoc.TenLop}!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PhanLich));
        }

        // GET: /GiaoVien/LichDay (Thời khóa biểu cá nhân của giáo viên hoặc xem bởi Admin)
        [AuthorizeRole("GiaoVien", "Admin")]
        public async Task<IActionResult> LichDay(int? maGiaoVien)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var sessionMaTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            GiaoVien? teacher = null;

            if (vaiTro == "GiaoVien")
            {
                teacher = await _context.GiaoViens
                    .Include(g => g.TaiKhoan)
                    .FirstOrDefaultAsync(g => g.MaTaiKhoan == sessionMaTaiKhoan);
            }
            else if (vaiTro == "Admin")
            {
                if (maGiaoVien.HasValue)
                {
                    teacher = await _context.GiaoViens
                        .Include(g => g.TaiKhoan)
                        .FirstOrDefaultAsync(g => g.MaGiaoVien == maGiaoVien.Value);
                }
                else
                {
                    teacher = await _context.GiaoViens
                        .Include(g => g.TaiKhoan)
                        .FirstOrDefaultAsync();
                }
            }

            if (teacher == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin giáo viên phù hợp!";
                return RedirectToAction("Index", "Home");
            }

            var assignedClasses = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.DangKyHocs)
                    .ThenInclude(d => d.HocVien)
                .Where(l => l.MaGiaoVien == teacher.MaGiaoVien)
                .OrderBy(l => l.NgayKhaiGiang)
                .ToListAsync();

            ViewBag.AllTeachers = await _context.GiaoViens.OrderBy(g => g.HoTen).ToListAsync();
            ViewBag.CurrentTeacher = teacher;
            ViewBag.IsAdmin = vaiTro == "Admin";

            return View(assignedClasses);
        }

        // GET: /GiaoVien/DanhSachHocVien?maLop=... (Xem danh sách học viên trong lớp)
        [HttpGet]
        [AuthorizeRole("GiaoVien", "Admin")]
        public async Task<IActionResult> DanhSachHocVien(int maLop)
        {
            var lopHoc = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiaoVien)
                .Include(l => l.DangKyHocs)
                    .ThenInclude(d => d.HocVien)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);

            if (lopHoc == null)
            {
                return NotFound();
            }

            return PartialView("_DanhSachHocVienModal", lopHoc);
        }
    }
}
