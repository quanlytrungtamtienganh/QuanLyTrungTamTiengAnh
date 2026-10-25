// ============================================================================
// [MODULE HỆ THỐNG CHUNG: LANDING PAGE GIỚI THIỆU TRUNG TÂM]
// Tên Tệp: LandingController.cs
// Mô tả: Chức năng: Trang đích marketing, giới thiệu ưu đãi khóa học ngoại ngữ mới
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class LandingController : Controller
    {
        private readonly TrungTamDbContext _context;

        public LandingController(TrungTamDbContext context)
        {
            _context = context;
        }

        // GET: / (trang landing chính - hiện trước khi đăng nhập)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Nếu đã đăng nhập thì chuyển thẳng vào nội dung
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.IsNullOrEmpty(vaiTro))
            {
                if (vaiTro == "Admin") return RedirectToAction("Index", "ThongKe");
                return RedirectToAction("Index", "Home");
            }

            var courses = await _context.KhoaHocs
                .OrderBy(k => k.NgoaiNgu)
                .Take(12)
                .ToListAsync();

            var teachers = await _context.GiaoViens
                .Include(g => g.TaiKhoan)
                .Where(g => g.TaiKhoan != null && g.TaiKhoan.TrangThai == "HoatDong")
                .Take(6)
                .ToListAsync();

            var deThisHot = await _context.DeThis
                .AsNoTracking()
                .Include(d => d.PhanThis)
                .Where(d => d.TrangThai)
                .OrderByDescending(d => d.SoLuotLam)
                .Take(4)
                .ToListAsync();

            var model = new LandingViewModel
            {
                KhoaHocs = courses,
                GiaoViens = teachers,
                DeThisHot = deThisHot
            };

            return View(model);
        }

        // GET: /Landing/KiemTraEmail?email=... - AJAX realtime check email
        [HttpGet]
        public async Task<IActionResult> KiemTraEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { trung = false });

            var emailNorm = email.Trim().ToLower();

            // Kiểm tra đã có tài khoản chưa
            var coTaiKhoan = await _context.TaiKhoans.AnyAsync(t => t.Email == emailNorm);
            if (coTaiKhoan)
                return Json(new { trung = true, loai = "taikhoan", thongBao = "Gmail này đã có tài khoản trong hệ thống. Vui lòng đăng nhập thay vì đăng ký mới!" });

            // Kiểm tra đang chờ xử lý chưa
            var dangChoXuLy = await _context.YeuCauDangKys
                .AnyAsync(y => y.Email == emailNorm && y.TrangThai == "ChoXuLy");
            if (dangChoXuLy)
                return Json(new { trung = true, loai = "yeucau", thongBao = "Gmail này đã gửi yêu cầu đăng ký và đang chờ Admin xử lý. Vui lòng chờ thông báo!" });

            return Json(new { trung = false });
        }

        // GET: /Landing/KiemTraSDT?soDienThoai=... - AJAX realtime check SĐT
        [HttpGet]
        public async Task<IActionResult> KiemTraSDT(string soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai) || soDienThoai.Trim().Length < 9)
                return Json(new { trung = false });

            var sdtNorm = soDienThoai.Trim();

            // Kiểm tra trong bảng HocVien
            var coHocVien = await _context.HocViens.AnyAsync(h => h.SoDienThoai == sdtNorm);
            if (coHocVien)
                return Json(new { trung = true, loai = "hocvien", thongBao = "Số điện thoại này đã được đăng ký bởi một học viên trong hệ thống!" });

            // Kiểm tra trong bảng GiaoVien
            var coGiaoVien = await _context.GiaoViens.AnyAsync(g => g.SoDienThoai == sdtNorm);
            if (coGiaoVien)
                return Json(new { trung = true, loai = "giaovien", thongBao = "Số điện thoại này đã được đăng ký bởi một giáo viên trong hệ thống!" });

            // Kiểm tra trong yêu cầu đang chờ
            var dangChoXuLy = await _context.YeuCauDangKys
                .AnyAsync(y => y.SoDienThoai == sdtNorm && y.TrangThai == "ChoXuLy");
            if (dangChoXuLy)
                return Json(new { trung = true, loai = "yeucau", thongBao = "Số điện thoại này đã có trong yêu cầu đăng ký đang chờ xử lý!" });

            return Json(new { trung = false });
        }

        // POST: /Landing/GuiYeuCau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiYeuCau(string hoTen, string email, string soDienThoai, string vaiTroMongMuon, string? ghiChu)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ họ tên và địa chỉ Gmail!";
                return RedirectToAction("Index");
            }

            var emailNorm = email.Trim().ToLower();

            // Kiểm tra trùng email đang chờ xử lý
            var exists = await _context.YeuCauDangKys
                .AnyAsync(y => y.Email == emailNorm && y.TrangThai == "ChoXuLy");
            if (exists)
            {
                TempData["Info"] = $"Gmail {email} đã gửi yêu cầu trước đó và đang chờ xử lý. Admin sẽ liên hệ bạn sớm!";
                return RedirectToAction("Index");
            }

            // Kiểm tra email đã có tài khoản chưa
            var hasAccount = await _context.TaiKhoans.AnyAsync(t => t.Email == emailNorm);
            if (hasAccount)
            {
                TempData["Info"] = "Gmail này đã có tài khoản trong hệ thống. Vui lòng đăng nhập!";
                return RedirectToAction("Index");
            }

            // Kiểm tra SĐT trùng (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(soDienThoai))
            {
                var sdtNorm = soDienThoai.Trim();
                var sdtHocVien = await _context.HocViens.AnyAsync(h => h.SoDienThoai == sdtNorm);
                var sdtGiaoVien = await _context.GiaoViens.AnyAsync(g => g.SoDienThoai == sdtNorm);
                var sdtYeuCau = await _context.YeuCauDangKys.AnyAsync(y => y.SoDienThoai == sdtNorm && y.TrangThai == "ChoXuLy");

                if (sdtHocVien || sdtGiaoVien || sdtYeuCau)
                {
                    TempData["Error"] = $"Số điện thoại {soDienThoai} đã được đăng ký trong hệ thống. Vui lòng kiểm tra lại hoặc dùng số khác!";
                    return RedirectToAction("Index");
                }
            }

            var yeuCau = new YeuCauDangKy
            {
                HoTen = hoTen.Trim(),
                Email = emailNorm,
                SoDienThoai = soDienThoai?.Trim() ?? "",
                VaiTroMongMuon = vaiTroMongMuon == "GiaoVien" ? "GiaoVien" : "HocVien",
                GhiChu = ghiChu?.Trim(),
                NgayGui = DateTime.Now,
                TrangThai = "ChoXuLy"
            };

            _context.YeuCauDangKys.Add(yeuCau);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cảm ơn {hoTen}! Yêu cầu đã được ghi nhận. Admin sẽ tạo tài khoản và gửi thông tin về Gmail {email} trong vòng 24 giờ!";
            return RedirectToAction("Index");
        }

        // GET: /Landing/GetPendingCount - API cho badge admin (không cần AuthorizeRole vì check session inline)
        [HttpGet]
        public async Task<IActionResult> GetPendingCount()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
                return Json(new { count = 0 });

            var count = await _context.YeuCauDangKys.CountAsync(y => y.TrangThai == "ChoXuLy");
            return Json(new { count });
        }
    }
}
