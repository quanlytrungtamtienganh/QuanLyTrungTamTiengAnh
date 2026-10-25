// ============================================================================
// [MODULE 1 - SINH VIÊN 1: TÀI KHOẢN – ĐĂNG NHẬP VÀ PHÂN QUYỀN – QUẢN LÝ KHÓA HỌC]
// Tên Tệp: TaiKhoanController.cs
// Mô tả: Chức năng: Quản lý tài khoản người dùng, Đăng nhập, Đăng ký, Phân quyền vai trò (Admin, GiaoVien, HocVien)
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly TrungTamDbContext _context;

        public TaiKhoanController(TrungTamDbContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoan/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            // Nếu đã đăng nhập, điều hướng tương ứng với vai trò
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.IsNullOrEmpty(vaiTro))
            {
                if (vaiTro == "Admin") return RedirectToAction("Index", "ThongKe");
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        // POST: /TaiKhoan/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.MatKhau == model.MatKhau);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác!");
                return View(model);
            }

            if (user.TrangThai == "Khoa")
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị tạm khóa. Vui lòng liên hệ trung tâm!");
                return View(model);
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);

            if (user.HocVien != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", user.HocVien.MaHocVien);
            }

            if (user.GiaoVien != null)
            {
                HttpContext.Session.SetInt32("MaGiaoVien", user.GiaoVien.MaGiaoVien);
            }

            TempData["Success"] = $"Xin chào {user.HoTen}, chúc bạn một ngày làm việc và học tập hiệu quả!";

            // Điều hướng theo ReturnUrl nếu có hoặc theo vai trò
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (user.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "ThongKe");
            }
            if (user.VaiTro == "GiaoVien")
            {
                return RedirectToAction("LichDay", "GiaoVien");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET/POST: /TaiKhoan/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Index", "Landing");
        }

        // GET: /TaiKhoan/Profile
        [HttpGet]
        [AuthorizeRole("Admin", "HocVien")]
        public async Task<IActionResult> Profile()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan.Value);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                MaTaiKhoan = user.MaTaiKhoan,
                TenDangNhap = user.TenDangNhap,
                HoTen = user.HoTen,
                Email = user.Email,
                VaiTro = user.VaiTro,
                MaHocVien = user.HocVien?.MaHocVien ?? 0,
                NgaySinh = user.HocVien?.NgaySinh ?? DateTime.Today.AddYears(-20),
                GioiTinh = user.HocVien?.GioiTinh ?? "Nam",
                SoDienThoai = user.HocVien?.SoDienThoai ?? "",
                DiaChi = user.HocVien?.DiaChi ?? ""
            };

            return View(viewModel);
        }

        // POST: /TaiKhoan/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "HocVien")]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue || maTaiKhoan.Value != model.MaTaiKhoan)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan.Value);

            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin tài khoản
            user.HoTen = model.HoTen;
            user.Email = model.Email;

            // Nếu là học viên, cập nhật thông tin trong bảng HocVien
            if (user.HocVien != null)
            {
                user.HocVien.HoTen = model.HoTen;
                user.HocVien.Email = model.Email;
                user.HocVien.NgaySinh = model.NgaySinh;
                user.HocVien.GioiTinh = model.GioiTinh;
                user.HocVien.SoDienThoai = model.SoDienThoai;
                user.HocVien.DiaChi = model.DiaChi;
            }

            await _context.SaveChangesAsync();

            // Cập nhật lại họ tên trong Session
            HttpContext.Session.SetString("HoTen", user.HoTen);

            TempData["Success"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction("Profile");
        }

        // =========================================================================
        // 4.4. GIAO DIỆN QUẢN LÝ TÀI KHOẢN (ADMIN)
        // =========================================================================
        // GET: /TaiKhoan/Index
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index(string? search, string? vaiTro, string? trangThai)
        {
            var query = _context.TaiKhoans
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(t => t.TenDangNhap.Contains(s) || t.HoTen.Contains(s) || t.Email.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            var list = await query.OrderByDescending(t => t.MaTaiKhoan).ToListAsync();

            ViewBag.Search = search;
            ViewBag.VaiTro = vaiTro;
            ViewBag.TrangThai = trangThai;
            ViewBag.TotalCount = list.Count;

            return View(list);
        }

        // GET: /TaiKhoan/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.TaiKhoans.FindAsync(id);
            if (user == null) return NotFound();

            var model = new EditTaiKhoanViewModel
            {
                MaTaiKhoan = user.MaTaiKhoan,
                TenDangNhap = user.TenDangNhap,
                HoTen = user.HoTen,
                Email = user.Email,
                VaiTro = user.VaiTro,
                TrangThai = user.TrangThai
            };

            return View(model);
        }

        // POST: /TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, EditTaiKhoanViewModel model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (user == null) return NotFound();

            // Kiểm tra trùng tên đăng nhập với tài khoản khác
            var duplicateUser = await _context.TaiKhoans
                .AnyAsync(t => t.MaTaiKhoan != id && t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());

            if (duplicateUser)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng bởi tài khoản khác!");
                return View(model);
            }

            user.TenDangNhap = model.TenDangNhap.Trim();
            user.HoTen = model.HoTen.Trim();
            user.Email = model.Email.Trim();
            user.VaiTro = model.VaiTro;
            user.TrangThai = model.TrangThai;

            if (!string.IsNullOrWhiteSpace(model.MatKhauMoi))
            {
                user.MatKhau = model.MatKhauMoi.Trim();
            }

            // Đồng bộ tên và email với HocVien hoặc GiaoVien nếu có
            if (user.HocVien != null)
            {
                user.HocVien.HoTen = user.HoTen;
                user.HocVien.Email = user.Email;
            }
            if (user.GiaoVien != null)
            {
                user.GiaoVien.HoTen = user.HoTen;
                user.GiaoVien.Email = user.Email;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cập nhật tài khoản '{user.TenDangNhap}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TaiKhoan/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                    .ThenInclude(h => h!.DangKyHocs)
                .Include(t => t.GiaoVien)
                    .ThenInclude(g => g!.LopHocs)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentAdminId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (currentAdminId.HasValue && currentAdminId.Value == id)
            {
                TempData["Error"] = "Bạn không thể tự xóa tài khoản của chính mình khi đang đăng nhập!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                    .ThenInclude(h => h!.DangKyHocs)
                .Include(t => t.GiaoVien)
                    .ThenInclude(g => g!.LopHocs)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (user != null)
            {
                // Kiểm tra ràng buộc dữ liệu: Nếu học viên đã có đơn đăng ký học
                if (user.HocVien != null && user.HocVien.DangKyHocs.Any())
                {
                    TempData["Error"] = $"Không thể xóa tài khoản '{user.TenDangNhap}' vì học viên này đã có {user.HocVien.DangKyHocs.Count} lượt đăng ký lớp học! Hãy chuyển trạng thái sang 'Khoa' để vô hiệu hóa.";
                    return RedirectToAction(nameof(Index));
                }

                // Nếu giáo viên đang phụ trách lớp học
                if (user.GiaoVien != null && user.GiaoVien.LopHocs.Any())
                {
                    TempData["Error"] = $"Không thể xóa tài khoản '{user.TenDangNhap}' vì giảng viên này đang được phân công dạy {user.GiaoVien.LopHocs.Count} lớp học!";
                    return RedirectToAction(nameof(Index));
                }

                _context.TaiKhoans.Remove(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã xóa tài khoản '{user.TenDangNhap}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
