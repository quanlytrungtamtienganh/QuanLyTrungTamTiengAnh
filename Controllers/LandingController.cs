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

        // GET: / (trang landing chính - hiện trước khi đăng nhập hoặc khi Admin/User xem trước)
        [HttpGet]
        public async Task<IActionResult> Index(bool? preview)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool isPreview = preview == true || Request.Query.ContainsKey("preview") || Request.Query.ContainsKey("landing");

            // Nếu đã đăng nhập và không yêu cầu xem trước landing page thì chuyển tới Trang chủ Bảng tin (/Home)
            if (!string.IsNullOrEmpty(vaiTro) && !isPreview)
            {
                // Cho phép cả Admin và Học viên truy cập xem giao diện người dùng (/Home) thay vì kẹt ở Console
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

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||

                          Request.Headers.Accept.Any(a => a != null && a.Contains("application/json"));

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email))

            {

                var errMsg = "Vui lòng nhập đầy đủ họ tên và địa chỉ Gmail!";

                if (isAjax) return Json(new { success = false, message = errMsg });

                TempData["Error"] = errMsg;

                return RedirectToAction("Index");

            }

            var emailNorm = email.Trim().ToLower();

            // 1. Kiểm tra email đã có tài khoản trong hệ thống chưa

            var hasAccount = await _context.TaiKhoans.AnyAsync(t => t.Email == emailNorm);

            if (hasAccount)

            {

                var accMsg = "Địa chỉ Gmail này đã có tài khoản trong hệ thống! Vui lòng đăng nhập.";

                if (isAjax) return Json(new { success = false, message = accMsg });

                TempData["Info"] = accMsg;

                return RedirectToAction("Index");

            }

            // 2. Nếu đã có yêu cầu gửi trước đó đang chờ xử lý -> Cập nhật thông tin mới nhất và gửi đến Admin

            var existingYeuCau = await _context.YeuCauDangKys

                .FirstOrDefaultAsync(y => y.Email == emailNorm && y.TrangThai == "ChoXuLy");

            if (existingYeuCau != null)

            {

                existingYeuCau.HoTen = hoTen.Trim();

                existingYeuCau.SoDienThoai = soDienThoai?.Trim() ?? "";

                existingYeuCau.VaiTroMongMuon = vaiTroMongMuon == "GiaoVien" ? "GiaoVien" : "HocVien";

                existingYeuCau.GhiChu = ghiChu?.Trim();

                existingYeuCau.NgayGui = DateTime.Now;

                await _context.SaveChangesAsync();

                var updateMsg = $"Cảm ơn {hoTen}! Yêu cầu đăng ký tài khoản của bạn đã được cập nhật và gửi đến Admin thành công.";

                if (isAjax) return Json(new { success = true, message = updateMsg });

                TempData["Success"] = updateMsg;

                return RedirectToAction("Index");

            }

            // 3. Tạo mới yêu cầu đăng ký

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

            var successMsg = $"Cảm ơn {hoTen}! Yêu cầu đăng ký tài khoản đã được gửi đến Admin thành công. Admin sẽ kiểm tra và tạo tài khoản gửi về Gmail của bạn sớm nhất!";

            if (isAjax) return Json(new { success = true, message = successMsg });

            TempData["Success"] = successMsg;

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

