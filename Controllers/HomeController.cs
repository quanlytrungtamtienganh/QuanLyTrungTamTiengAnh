// ============================================================================
// [MODULE HỆ THỐNG CHUNG: TRANG CHỦ & ĐIỀU HƯỚNG TỔNG QUAN]
// Tên Tệp: HomeController.cs
// Mô tả: Chức năng: Trang chủ trung tâm, Banner quảng cáo khóa học, Giới thiệu đội ngũ giảng viên, Khóa học nổi bật
// ============================================================================

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class HomeController : Controller
    {
        private readonly TrungTamDbContext _context;

        public HomeController(TrungTamDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCourses = await _context.KhoaHocs
                .Include(k => k.BinhLuans)
                    .ThenInclude(b => b.TaiKhoan)
                .Where(k => k.TrangThai == "Đang mở")
                .Take(4)
                .ToListAsync();

            var recruitingClasses = await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.DangKyHocs)
                .Where(l => l.TrangThai == "Đang tuyển sinh")
                .OrderBy(l => l.NgayKhaiGiang)
                .Take(6)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedCourses = featuredCourses,
                RecruitingClasses = recruitingClasses,
                TotalCourses = await _context.KhoaHocs.CountAsync(),
                TotalClasses = await _context.LopHocs.CountAsync(),
                TotalStudents = await _context.HocViens.CountAsync()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
