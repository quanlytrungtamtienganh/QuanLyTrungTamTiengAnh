// ============================================================================
// [MODULE 4 - SINH VIÊN 4: QUẢN LÝ ĐĂNG KÝ – DASHBOARD – THỐNG KÊ]
// Tên Tệp: YeuCauDangKyController.cs
// Mô tả: Chức năng: Giao diện quản lý đăng ký (Mã ĐK, Khóa học, Lớp học, Trạng thái, Học phí), Duyệt/Từ chối hồ sơ đăng ký
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Filters;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.Services;

namespace TrungTamNgoaiNgu.Controllers
{
    [AuthorizeRole("Admin")]
    public class YeuCauDangKyController : Controller
    {
        private readonly TrungTamDbContext _context;
        private readonly IEmailService _emailService;

        public YeuCauDangKyController(TrungTamDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /YeuCauDangKy
        [HttpGet]
        public async Task<IActionResult> Index(string? trangThai)
        {
            var query = _context.YeuCauDangKys.AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(y => y.TrangThai == trangThai);

            var list = await query.OrderByDescending(y => y.NgayGui).ToListAsync();

            ViewBag.TrangThai = trangThai;
            ViewBag.TongChoXuLy = await _context.YeuCauDangKys.CountAsync(y => y.TrangThai == "ChoXuLy");
            ViewBag.TongDaTao = await _context.YeuCauDangKys.CountAsync(y => y.TrangThai == "DaTao");
            ViewBag.TongDaTu = await _context.YeuCauDangKys.CountAsync(y => y.TrangThai == "DaTu");

            return View(list);
        }

        // GET: /YeuCauDangKy/GetPendingCount (cho badge notification - check session inline)
        [HttpGet]
        public async Task<IActionResult> GetPendingCount()
        {
            // Chỉ trả về count nếu đang là Admin
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
                return Json(new { count = 0 });

            var count = await _context.YeuCauDangKys.CountAsync(y => y.TrangThai == "ChoXuLy");
            return Json(new { count });
        }

        // POST: /YeuCauDangKy/TaoTaiKhoan - Tạo tài khoản từ yêu cầu và gửi mail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTaiKhoan(int maYeuCau, string tenDangNhap, string matKhau, string hoTen, string email, string vaiTro)
        {
            var yeuCau = await _context.YeuCauDangKys.FindAsync(maYeuCau);
            if (yeuCau == null)
            {
                TempData["Error"] = "Không tìm thấy yêu cầu!";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                TempData["Error"] = "Tên đăng nhập và mật khẩu không được để trống!";
                return RedirectToAction("Index");
            }

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            var exists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap.Trim());
            if (exists)
            {
                TempData["Error"] = $"Tên đăng nhập '{tenDangNhap}' đã tồn tại! Vui lòng chọn tên khác.";
                return RedirectToAction("Index");
            }

            // Tạo tài khoản mới
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap.Trim().ToLower(),
                MatKhau = matKhau.Trim(),
                HoTen = hoTen.Trim(),
                Email = email.Trim().ToLower(),
                VaiTro = vaiTro == "GiaoVien" ? "GiaoVien" : "HocVien",
                TrangThai = "HoatDong"
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // Tạo record HocVien hoặc GiaoVien tương ứng
            if (taiKhoan.VaiTro == "HocVien")
            {
                _context.HocViens.Add(new HocVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    NgaySinh = DateTime.Today.AddYears(-20),
                    GioiTinh = "Nam",
                    SoDienThoai = yeuCau.SoDienThoai
                });
            }
            else if (taiKhoan.VaiTro == "GiaoVien")
            {
                _context.GiaoViens.Add(new GiaoVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    ChuyenMon = "Giáo viên",
                    SoDienThoai = yeuCau.SoDienThoai,
                    Email = taiKhoan.Email
                });
            }

            // Cập nhật trạng thái yêu cầu
            yeuCau.TrangThai = "DaTao";
            await _context.SaveChangesAsync();

            // Gửi email thông báo
            await _emailService.SendAccountCreatedEmailAsync(
                toEmail: email,
                toName: hoTen,
                username: tenDangNhap.Trim().ToLower(),
                password: matKhau.Trim(),
                role: taiKhoan.VaiTro
            );

            TempData["Success"] = $"✅ Đã tạo tài khoản '{tenDangNhap}' thành công và gửi email tới {email}!";
            return RedirectToAction("Index");
        }

        // POST: /YeuCauDangKy/TuChoi - Từ chối yêu cầu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int maYeuCau)
        {
            var yeuCau = await _context.YeuCauDangKys.FindAsync(maYeuCau);
            if (yeuCau != null)
            {
                yeuCau.TrangThai = "DaTu";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã từ chối yêu cầu của {yeuCau.HoTen}.";
            }
            return RedirectToAction("Index");
        }

        // POST: /YeuCauDangKy/Xoa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int maYeuCau)
        {
            var yeuCau = await _context.YeuCauDangKys.FindAsync(maYeuCau);
            if (yeuCau != null)
            {
                _context.YeuCauDangKys.Remove(yeuCau);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa yêu cầu thành công.";
            }
            return RedirectToAction("Index");
        }
    }
}
