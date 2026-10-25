// ============================================================================
// [MODULE MỞ RỘNG (NÂNG CAO): MẠNG XÃ HỘI HỌC TẬP & DIỄN ĐÀN TRUNG TÂM]
// Tên Tệp: BinhLuanController.cs
// Mô tả: Chức năng: Xử lý bình luận đa cấp cho bài viết diễn đàn và đề thi trực tuyến
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class BinhLuanController : Controller
    {
        private readonly TrungTamDbContext _context;

        public BinhLuanController(TrungTamDbContext context)
        {
            _context = context;
        }

        // =====================================================================
        // 1. THÊM BÌNH LUẬN (Học viên, Quản trị viên, Người dùng đã đăng nhập)
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? maKhoaHoc, string noiDung, string? returnUrl)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để tham gia bình luận!";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = returnUrl ?? "/" });
            }

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                TempData["Error"] = "Nội dung bình luận không được để trống!";
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            if (noiDung.Length > 1000)
            {
                TempData["Error"] = "Nội dung bình luận không được vượt quá 1000 ký tự!";
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            var binhLuan = new BinhLuan
            {
                MaTaiKhoan = maTaiKhoan.Value,
                MaKhoaHoc = maKhoaHoc,
                NoiDung = noiDung.Trim(),
                NgayBinhLuan = DateTime.Now
            };

            _context.BinhLuans.Add(binhLuan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gửi bình luận thành công!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // =====================================================================
        // 2. XÓA BÌNH LUẬN (Admin xóa mọi bình luận, hoặc người dùng xóa của mình)
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? returnUrl)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (maTaiKhoan == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thực hiện thao tác này!";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var binhLuan = await _context.BinhLuans.FindAsync(id);
            if (binhLuan == null)
            {
                TempData["Error"] = "Không tìm thấy bình luận cần xóa!";
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            // Phân quyền: Chỉ Quản trị viên (Admin) hoặc chính chủ sở hữu bình luận mới có quyền xóa
            bool isAdmin = vaiTro == "Admin";
            bool isOwner = binhLuan.MaTaiKhoan == maTaiKhoan.Value;

            if (!isAdmin && !isOwner)
            {
                TempData["Error"] = "Bạn không có quyền xóa bình luận này!";
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            _context.BinhLuans.Remove(binhLuan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa bình luận thành công!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // =====================================================================
        // 3. QUẢN LÝ BÌNH LUẬN & XEM % TƯƠNG TÁC TÀI KHOẢN (Dành riêng cho Admin)
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> QuanLy()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang quản lý bình luận & tương tác!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy tất cả tài khoản học viên (tài khoản dưới quyền Admin)
            var studentAccounts = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .Where(t => t.VaiTro == "HocVien")
                .ToListAsync();

            // Lấy toàn bộ bình luận trong hệ thống
            var allComments = await _context.BinhLuans
                .Include(b => b.TaiKhoan)
                .Include(b => b.KhoaHoc)
                .OrderByDescending(b => b.NgayBinhLuan)
                .ToListAsync();

            int totalComments = allComments.Count;
            int totalStudents = studentAccounts.Count;

            // Xây dựng danh sách thống kê % tương tác từng tài khoản dưới quyền Admin
            var danhSachTuongTac = new List<TaiKhoanTuongTacItem>();

            foreach (var student in studentAccounts)
            {
                var userComments = allComments.Where(c => c.MaTaiKhoan == student.MaTaiKhoan).ToList();
                int commentCount = userComments.Count;
                
                // % đóng góp tương tác trên tổng số bình luận toàn hệ thống
                double phanTram = totalComments > 0 ? Math.Round((double)commentCount / totalComments * 100.0, 1) : 0;
                var lastInteraction = userComments.OrderByDescending(c => c.NgayBinhLuan).FirstOrDefault()?.NgayBinhLuan;

                string mucDo = "Chưa tương tác";
                if (commentCount >= 4) mucDo = "Rất tích cực (Top)";
                else if (commentCount >= 2) mucDo = "Tích cực";
                else if (commentCount == 1) mucDo = "Thỉnh thoảng";

                danhSachTuongTac.Add(new TaiKhoanTuongTacItem
                {
                    MaTaiKhoan = student.MaTaiKhoan,
                    MaHocVien = student.HocVien?.MaHocVien,
                    TenDangNhap = student.TenDangNhap,
                    HoTen = student.HoTen,
                    Email = student.Email,
                    SoDienThoai = student.HocVien?.SoDienThoai,
                    VaiTro = student.VaiTro,
                    SoBinhLuan = commentCount,
                    PhanTramDongGop = phanTram,
                    LanTuongTacCuoi = lastInteraction,
                    MucDoTuongTac = mucDo
                });
            }

            // Sắp xếp học viên có % tương tác cao nhất lên đầu
            danhSachTuongTac = danhSachTuongTac.OrderByDescending(x => x.SoBinhLuan).ThenByDescending(x => x.LanTuongTacCuoi).ToList();

            // Tính tỷ lệ % học viên đã tham gia tương tác toàn hệ thống
            int activeStudents = danhSachTuongTac.Count(x => x.SoBinhLuan > 0);
            double tyLeTuongTacHocVien = totalStudents > 0 ? Math.Round((double)activeStudents / totalStudents * 100.0, 1) : 0;

            var topUser = danhSachTuongTac.FirstOrDefault(x => x.SoBinhLuan > 0);

            var viewModel = new QuanLyBinhLuanViewModel
            {
                TongBinhLuan = totalComments,
                TongTaiKhoanHocVien = totalStudents,
                HocVienDaTuongTac = activeStudents,
                TyLeHocVienTuongTac = tyLeTuongTacHocVien,
                TopHocVienTuongTac = topUser != null ? topUser.HoTen : "Chưa có",
                SoLuongTop = topUser != null ? topUser.SoBinhLuan : 0,
                DanhSachTaiKhoan = danhSachTuongTac,
                DanhSachBinhLuan = allComments
            };

            return View(viewModel);
        }
    }
}
