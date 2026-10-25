// ============================================================================
// [MODULE MỞ RỘNG (NÂNG CAO): MẠNG XÃ HỘI HỌC TẬP & DIỄN ĐÀN TRUNG TÂM]
// Tên Tệp: BaiVietController.cs
// Mô tả: Chức năng: Bảng tin cộng đồng, Đăng bài viết chia sẻ phương pháp học, Thích bài viết, Bình luận trao đổi, Báo cáo vi phạm
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly TrungTamDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BaiVietController(TrungTamDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================================================================
        // 1. GET: /BaiViet/GetFeed (Lấy danh sách bài viết bảng tin)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetFeed(string filter = "all")
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");
            bool isAdmin = userRole == "Admin";

            // Kiểm tra cấu hình xét duyệt
            var config = await _context.CauHinhHeThongs.FirstOrDefaultAsync(c => c.Khoa == "BatXetDuyetBaiViet");
            bool batXetDuyet = config?.GiaTri == "true";

            IQueryable<BaiViet> query = _context.BaiViets
                .Include(b => b.NguoiDang)
                    .ThenInclude(t => t!.GiaoVien)
                .Include(b => b.NguoiDang)
                    .ThenInclude(t => t!.HocVien)
                .Include(b => b.BaoCaos)
                .Include(b => b.BinhLuans)
                .Include(b => b.Thichs)
                .Include(b => b.BaiVietGoc)
                    .ThenInclude(g => g!.NguoiDang)
                        .ThenInclude(t => t!.GiaoVien);

            if (isAdmin)
            {
                if (filter == "pending")
                {
                    query = query.Where(b => b.TrangThai == "ChoDuyet");
                }
                else if (filter == "reported")
                {
                    query = query.Where(b => b.BaoCaos.Any(r => r.TrangThai == "ChoXuLy"));
                }
                // Nếu "all", Admin xem tất cả bài đã duyệt và chờ duyệt
                else
                {
                    query = query.Where(b => b.TrangThai == "DaDuyet" || b.TrangThai == "ChoDuyet");
                }
            }
            else
            {
                // Người dùng thông thường: xem bài đã duyệt, hoặc bài của chính mình đang chờ duyệt
                if (currentUserId.HasValue)
                {
                    query = query.Where(b => b.TrangThai == "DaDuyet" || (b.TrangThai == "ChoDuyet" && b.MaTaiKhoan == currentUserId.Value));
                }
                else
                {
                    query = query.Where(b => b.TrangThai == "DaDuyet");
                }
            }

            var posts = await query
                .OrderByDescending(b => b.NgayDang)
                .Select(b => new
                {
                    maBaiViet = b.MaBaiViet,
                    maTaiKhoan = b.MaTaiKhoan,
                    tacGiaTen = b.NguoiDang != null ? b.NguoiDang.HoTen : "Người dùng",
                    tacGiaVaiTro = b.NguoiDang != null ? b.NguoiDang.VaiTro : "HocVien",
                    tacGiaAvatar = b.NguoiDang != null && b.NguoiDang.GiaoVien != null ? b.NguoiDang.GiaoVien.AnhDaiDien : null,
                    noiDung = b.NoiDung,
                    hinhAnhUrl = b.HinhAnhUrl,
                    videoUrl = b.VideoUrl,
                    phongNen = b.PhongNen,
                    ngayDang = b.NgayDang.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ngayDangHienThi = FormatTimeAgo(b.NgayDang),
                    trangThai = b.TrangThai,
                    soBaoCao = b.BaoCaos.Count(r => r.TrangThai == "ChoXuLy"),
                    soBinhLuan = b.BinhLuans.Count,
                    soThich = b.Thichs.Count,
                    daThich = currentUserId.HasValue && b.Thichs.Any(t => t.MaTaiKhoan == currentUserId.Value),
                    soChiaSe = b.SoChiaSe,
                    maBaiVietGoc = b.MaBaiVietGoc,
                    baiVietGoc = b.BaiVietGoc != null ? new
                    {
                        maBaiViet = b.BaiVietGoc.MaBaiViet,
                        tacGiaTen = b.BaiVietGoc.NguoiDang != null ? b.BaiVietGoc.NguoiDang.HoTen : "Người dùng",
                        tacGiaVaiTro = b.BaiVietGoc.NguoiDang != null ? b.BaiVietGoc.NguoiDang.VaiTro : "HocVien",
                        tacGiaAvatar = b.BaiVietGoc.NguoiDang != null && b.BaiVietGoc.NguoiDang.GiaoVien != null ? b.BaiVietGoc.NguoiDang.GiaoVien.AnhDaiDien : null,
                        noiDung = b.BaiVietGoc.NoiDung,
                        hinhAnhUrl = b.BaiVietGoc.HinhAnhUrl,
                        videoUrl = b.BaiVietGoc.VideoUrl,
                        phongNen = b.BaiVietGoc.PhongNen,
                        ngayDangHienThi = FormatTimeAgo(b.BaiVietGoc.NgayDang)
                    } : null,
                    isAuthor = currentUserId.HasValue && b.MaTaiKhoan == currentUserId.Value,
                    canDelete = isAdmin || (currentUserId.HasValue && b.MaTaiKhoan == currentUserId.Value),
                    canReport = currentUserId.HasValue && b.MaTaiKhoan != currentUserId.Value && !isAdmin,
                    canApprove = isAdmin && b.TrangThai == "ChoDuyet"
                })
                .ToListAsync();

            int pendingCount = isAdmin ? await _context.BaiViets.CountAsync(b => b.TrangThai == "ChoDuyet") : 0;
            int reportCount = isAdmin ? await _context.BaoCaoBaiViets.CountAsync(r => r.TrangThai == "ChoXuLy") : 0;

            return Json(new
            {
                success = true,
                posts,
                batXetDuyet,
                isAdmin,
                pendingCount,
                reportCount,
                currentUserId,
                userRole
            });
        }

        // =========================================================================
        // 2. POST: /BaiViet/TaoBaiViet (Đăng bài viết mới)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> TaoBaiViet(
            [FromForm] string noiDung,
            [FromForm] string? phongNen,
            [FromForm] string? hinhAnhUrl,
            [FromForm] string? videoUrl,
            IFormFile? hinhAnhFile,
            IFormFile? videoFile)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");

            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để đăng bài viết!" });
            }

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung bài viết!" });
            }

            // Xử lý file ảnh tải lên nếu có
            string? finalHinhAnh = hinhAnhUrl?.Trim();
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var ext = Path.GetExtension(hinhAnhFile.FileName).ToLower();
                    var fileName = $"img_{DateTime.Now.Ticks}_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await hinhAnhFile.CopyToAsync(stream);
                    }
                    finalHinhAnh = $"/uploads/posts/{fileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi upload ảnh: " + ex.Message);
                }
            }

            // Xử lý file video tải lên nếu có
            string? finalVideo = videoUrl?.Trim();
            if (videoFile != null && videoFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var ext = Path.GetExtension(videoFile.FileName).ToLower();
                    var fileName = $"vid_{DateTime.Now.Ticks}_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await videoFile.CopyToAsync(stream);
                    }
                    finalVideo = $"/uploads/posts/{fileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi upload video: " + ex.Message);
                }
            }

            // Nếu có ảnh hoặc video thì hủy phông nền màu (chuẩn Facebook)
            string? finalPhongNen = (!string.IsNullOrEmpty(finalHinhAnh) || !string.IsNullOrEmpty(finalVideo)) ? null : phongNen?.Trim();
            if (finalPhongNen == "none" || finalPhongNen == "default" || string.IsNullOrWhiteSpace(finalPhongNen))
            {
                finalPhongNen = null;
            }

            // Kiểm tra cơ chế xét duyệt
            var config = await _context.CauHinhHeThongs.FirstOrDefaultAsync(c => c.Khoa == "BatXetDuyetBaiViet");
            bool batXetDuyet = config?.GiaTri == "true";

            // Nếu người đăng là Admin -> Tự động duyệt ngay
            // Nếu BẬT xét duyệt và người đăng là Học viên hoặc Giáo viên -> Trạng thái Chờ duyệt
            // Nếu TẮT xét duyệt -> Đã duyệt ngay
            string trangThai = (userRole == "Admin" || !batXetDuyet) ? "DaDuyet" : "ChoDuyet";

            var baiViet = new BaiViet
            {
                MaTaiKhoan = currentUserId.Value,
                NoiDung = noiDung.Trim(),
                HinhAnhUrl = finalHinhAnh,
                VideoUrl = finalVideo,
                PhongNen = finalPhongNen,
                NgayDang = DateTime.Now,
                TrangThai = trangThai
            };

            _context.BaiViets.Add(baiViet);
            await _context.SaveChangesAsync();

            string notifyMessage = (trangThai == "ChoDuyet")
                ? "Bài viết của bạn đã được gửi và đang chờ Quản trị viên xét duyệt trước khi hiển thị công khai!"
                : "Đăng bài viết lên bảng tin thành công!";

            return Json(new
            {
                success = true,
                maBaiViet = baiViet.MaBaiViet,
                trangThai = baiViet.TrangThai,
                message = notifyMessage
            });
        }

        // =========================================================================
        // 3. POST: /BaiViet/XoaBaiViet (Người đăng hoặc Quản lý có thể xóa bài viết)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> XoaBaiViet(int maBaiViet)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");

            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện!" });
            }

            var post = await _context.BaiViets.FindAsync(maBaiViet);
            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại hoặc đã bị xóa!" });
            }

            // KIỂM TRA QUYỀN: Người đăng hoặc Quản lý (Admin)
            bool canDelete = (userRole == "Admin") || (post.MaTaiKhoan == currentUserId.Value);
            if (!canDelete)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa bài viết này!" });
            }

            _context.BaiViets.Remove(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa bài viết thành công khỏi bảng tin!" });
        }

        // =========================================================================
        // 4. POST: /BaiViet/BaoCaoBaiViet (Học sinh và Giáo viên báo cáo bài viết)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> BaoCaoBaiViet(int maBaiViet, string lyDo, string? chiTiet)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để gửi báo cáo!" });
            }

            if (string.IsNullOrWhiteSpace(lyDo))
            {
                return Json(new { success = false, message = "Vui lòng chọn lý do báo cáo sai phạm!" });
            }

            var post = await _context.BaiViets.FindAsync(maBaiViet);
            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại!" });
            }

            if (post.MaTaiKhoan == currentUserId.Value)
            {
                return Json(new { success = false, message = "Bạn không thể tự báo cáo bài viết của chính mình!" });
            }

            // Kiểm tra xem người dùng đã báo cáo bài viết này chưa
            var already = await _context.BaoCaoBaiViets.AnyAsync(r => r.MaBaiViet == maBaiViet && r.NguoiBaoCaoId == currentUserId.Value && r.TrangThai == "ChoXuLy");
            if (already)
            {
                return Json(new { success = false, message = "Bạn đã gửi báo cáo vi phạm cho bài viết này rồi. Quản trị viên đang xem xét!" });
            }

            var baoCao = new BaoCaoBaiViet
            {
                MaBaiViet = maBaiViet,
                NguoiBaoCaoId = currentUserId.Value,
                LyDo = lyDo.Trim(),
                ChiTiet = chiTiet?.Trim(),
                NgayBaoCao = DateTime.Now,
                TrangThai = "ChoXuLy"
            };

            _context.BaoCaoBaiViets.Add(baoCao);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Cảm ơn bạn đã phản hồi! Báo cáo sai phạm đã được gửi đến Quản trị viên để kiểm tra và xử lý."
            });
        }

        // =========================================================================
        // 5. POST: /BaiViet/DuyetBaiViet (Quản lý duyệt hoặc từ chối bài viết)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> DuyetBaiViet(int maBaiViet, bool dongY)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");

            if (!currentUserId.HasValue || userRole != "Admin")
            {
                return Json(new { success = false, message = "Chỉ Quản lý (Admin) mới có quyền duyệt bài viết!" });
            }

            var post = await _context.BaiViets.FindAsync(maBaiViet);
            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại!" });
            }

            if (dongY)
            {
                post.TrangThai = "DaDuyet";
                post.NguoiDuyetId = currentUserId.Value;
                post.NgayDuyet = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã phê duyệt bài viết! Bài viết hiện đã hiển thị công khai trên Bảng tin." });
            }
            else
            {
                // Từ chối và xóa bài viết khỏi hệ thống
                _context.BaiViets.Remove(post);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã từ chối và gỡ bỏ bài viết thành công!" });
            }
        }

        // =========================================================================
        // 6. POST: /BaiViet/BatTatXetDuyet (Quản lý Bật / Tắt chế độ xét duyệt bài đăng)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> BatTatXetDuyet(bool batXetDuyet)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");

            if (!currentUserId.HasValue || userRole != "Admin")
            {
                return Json(new { success = false, message = "Chỉ Quản lý mới có quyền thay đổi cài đặt xét duyệt bài đăng!" });
            }

            var config = await _context.CauHinhHeThongs.FirstOrDefaultAsync(c => c.Khoa == "BatXetDuyetBaiViet");
            if (config == null)
            {
                config = new CauHinhHeThong
                {
                    Khoa = "BatXetDuyetBaiViet",
                    GiaTri = batXetDuyet ? "true" : "false",
                    MoTa = "Bật/Tắt chế độ kiểm duyệt bài viết của học viên và giáo viên trước khi hiển thị công khai",
                    NgayCapNhat = DateTime.Now
                };
                _context.CauHinhHeThongs.Add(config);
            }
            else
            {
                config.GiaTri = batXetDuyet ? "true" : "false";
                config.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            string statusMsg = batXetDuyet
                ? "Đã BẬT chế độ kiểm duyệt! Từ bây giờ các bài đăng mới của Học viên và Giáo viên sẽ cần Quản lý duyệt trước khi xuất hiện công khai."
                : "Đã TẮT chế độ kiểm duyệt! Các bài đăng mới của Học viên và Giáo viên sẽ được hiển thị ngay lập tức.";

            return Json(new
            {
                success = true,
                batXetDuyet = batXetDuyet,
                message = statusMsg
            });
        }

        // =========================================================================
        // 7. GET: /BaiViet/GetDanhSachBaoCao (Quản lý xem danh sách các báo cáo vi phạm)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetDanhSachBaoCao()
        {
            var userRole = HttpContext.Session.GetString("VaiTro");
            if (userRole != "Admin")
            {
                return Json(new { success = false, message = "Chỉ Quản lý mới có quyền xem danh sách báo cáo vi phạm!" });
            }

            var reports = await _context.BaoCaoBaiViets
                .Include(r => r.NguoiBaoCao)
                .Include(r => r.BaiViet)
                    .ThenInclude(b => b!.NguoiDang)
                .Where(r => r.TrangThai == "ChoXuLy")
                .OrderByDescending(r => r.NgayBaoCao)
                .Select(r => new
                {
                    maBaoCao = r.MaBaoCao,
                    maBaiViet = r.MaBaiViet,
                    nguoiBaoCaoTen = r.NguoiBaoCao != null ? r.NguoiBaoCao.HoTen : "Người dùng",
                    nguoiBaoCaoVaiTro = r.NguoiBaoCao != null ? r.NguoiBaoCao.VaiTro : "HocVien",
                    lyDo = r.LyDo,
                    chiTiet = r.ChiTiet,
                    ngayBaoCao = r.NgayBaoCao.ToString("HH:mm dd/MM/yyyy"),
                    baiVietNoiDung = r.BaiViet != null ? r.BaiViet.NoiDung : "[Bài viết đã bị xóa]",
                    baiVietTacGia = r.BaiViet != null && r.BaiViet.NguoiDang != null ? r.BaiViet.NguoiDang.HoTen : "Không xác định",
                    baiVietHinhAnh = r.BaiViet != null ? r.BaiViet.HinhAnhUrl : null,
                    baiVietVideo = r.BaiViet != null ? r.BaiViet.VideoUrl : null
                })
                .ToListAsync();

            return Json(new { success = true, reports });
        }

        // =========================================================================
        // 8. POST: /BaiViet/XuLyBaoCao (Quản lý xử lý báo cáo: Xóa bài hoặc Bỏ qua)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> XuLyBaoCao(int maBaoCao, string hanhDong)
        {
            var userRole = HttpContext.Session.GetString("VaiTro");
            if (userRole != "Admin")
            {
                return Json(new { success = false, message = "Chỉ Quản lý mới có quyền xử lý báo cáo vi phạm!" });
            }

            var baoCao = await _context.BaoCaoBaiViets
                .Include(r => r.BaiViet)
                .FirstOrDefaultAsync(r => r.MaBaoCao == maBaoCao);

            if (baoCao == null)
            {
                return Json(new { success = false, message = "Báo cáo không tồn tại hoặc đã xử lý!" });
            }

            if (hanhDong == "xoa_bai")
            {
                if (baoCao.BaiViet != null)
                {
                    _context.BaiViets.Remove(baoCao.BaiViet);
                }
                baoCao.TrangThai = "DaXuLy_XoaBai";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa bài viết vi phạm thành công!" });
            }
            else // "bo_qua"
            {
                baoCao.TrangThai = "DaBoQua";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã bỏ qua báo cáo! Bài viết vẫn được giữ lại." });
            }
        }

        // =========================================================================
        // 9. POST: /BaiViet/ToggleLike (Thích hoặc Bỏ thích bài viết)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int maBaiViet)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để thích bài viết!" });
            }

            var post = await _context.BaiViets.FindAsync(maBaiViet);
            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại!" });
            }

            var existingLike = await _context.ThichBaiViets
                .FirstOrDefaultAsync(t => t.MaBaiViet == maBaiViet && t.MaTaiKhoan == currentUserId.Value);

            bool isLiked;
            if (existingLike != null)
            {
                _context.ThichBaiViets.Remove(existingLike);
                isLiked = false;
            }
            else
            {
                _context.ThichBaiViets.Add(new ThichBaiViet
                {
                    MaBaiViet = maBaiViet,
                    MaTaiKhoan = currentUserId.Value,
                    NgayThich = DateTime.Now
                });
                isLiked = true;
            }

            await _context.SaveChangesAsync();

            int likeCount = await _context.ThichBaiViets.CountAsync(t => t.MaBaiViet == maBaiViet);

            return Json(new
            {
                success = true,
                isLiked,
                likeCount
            });
        }

        // =========================================================================
        // 10. GET: /BaiViet/GetBinhLuans (Lấy danh sách bình luận của bài viết)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetBinhLuans(int maBaiViet)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");
            bool isAdmin = userRole == "Admin";

            var comments = await _context.BinhLuans
                .Include(b => b.TaiKhoan)
                    .ThenInclude(t => t!.GiaoVien)
                .Where(b => b.MaBaiViet == maBaiViet)
                .OrderBy(b => b.NgayBinhLuan)
                .Select(b => new
                {
                    maBinhLuan = b.MaBinhLuan,
                    maTaiKhoan = b.MaTaiKhoan,
                    tacGiaTen = b.TaiKhoan != null ? b.TaiKhoan.HoTen : "Người dùng",
                    tacGiaVaiTro = b.TaiKhoan != null ? b.TaiKhoan.VaiTro : "HocVien",
                    tacGiaAvatar = b.TaiKhoan != null && b.TaiKhoan.GiaoVien != null ? b.TaiKhoan.GiaoVien.AnhDaiDien : null,
                    noiDung = b.NoiDung,
                    ngayBinhLuanHienThi = FormatTimeAgo(b.NgayBinhLuan),
                    canDelete = isAdmin || (currentUserId.HasValue && b.MaTaiKhoan == currentUserId.Value)
                })
                .ToListAsync();

            return Json(new { success = true, comments, totalComments = comments.Count });
        }

        // =========================================================================
        // 11. POST: /BaiViet/ThemBinhLuan (Gửi bình luận vào bài viết)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> ThemBinhLuan(int maBaiViet, string noiDung)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để bình luận!" });
            }

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung bình luận!" });
            }

            var post = await _context.BaiViets.FindAsync(maBaiViet);
            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại hoặc đã bị xóa!" });
            }

            var user = await _context.TaiKhoans
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);

            var comment = new BinhLuan
            {
                MaBaiViet = maBaiViet,
                MaTaiKhoan = currentUserId.Value,
                NoiDung = noiDung.Trim(),
                NgayBinhLuan = DateTime.Now
            };

            _context.BinhLuans.Add(comment);
            await _context.SaveChangesAsync();

            int totalComments = await _context.BinhLuans.CountAsync(b => b.MaBaiViet == maBaiViet);

            return Json(new
            {
                success = true,
                message = "Đã gửi bình luận!",
                totalComments,
                comment = new
                {
                    maBinhLuan = comment.MaBinhLuan,
                    maTaiKhoan = comment.MaTaiKhoan,
                    tacGiaTen = user != null ? user.HoTen : "Bạn",
                    tacGiaVaiTro = user != null ? user.VaiTro : "HocVien",
                    tacGiaAvatar = user?.GiaoVien?.AnhDaiDien,
                    noiDung = comment.NoiDung,
                    ngayBinhLuanHienThi = "Vừa xong",
                    canDelete = true
                }
            });
        }

        // =========================================================================
        // 12. POST: /BaiViet/XoaBinhLuan (Xóa bình luận)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> XoaBinhLuan(int maBinhLuan)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var userRole = HttpContext.Session.GetString("VaiTro");
            bool isAdmin = userRole == "Admin";

            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện!" });
            }

            var comment = await _context.BinhLuans.FindAsync(maBinhLuan);
            if (comment == null)
            {
                return Json(new { success = false, message = "Bình luận không tồn tại hoặc đã bị xóa!" });
            }

            bool canDelete = isAdmin || (comment.MaTaiKhoan == currentUserId.Value);
            if (!canDelete)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa bình luận này!" });
            }

            int? maBaiViet = comment.MaBaiViet;
            _context.BinhLuans.Remove(comment);
            await _context.SaveChangesAsync();

            int totalComments = maBaiViet.HasValue ? await _context.BinhLuans.CountAsync(b => b.MaBaiViet == maBaiViet.Value) : 0;

            return Json(new
            {
                success = true,
                message = "Đã xóa bình luận thành công!",
                totalComments
            });
        }

        // =========================================================================
        // 13. POST: /BaiViet/ChiaSeBaiViet (Chia sẻ bài viết lên bảng tin cá nhân)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> ChiaSeBaiViet(int maBaiVietGoc, string? loiBinh)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để chia sẻ bài viết!" });
            }

            var originalPost = await _context.BaiViets
                .Include(b => b.NguoiDang)
                .FirstOrDefaultAsync(b => b.MaBaiViet == maBaiVietGoc);

            if (originalPost == null)
            {
                return Json(new { success = false, message = "Bài viết gốc không tồn tại!" });
            }

            // Tạo bài viết mới dạng bài chia sẻ
            var sharedPost = new BaiViet
            {
                MaTaiKhoan = currentUserId.Value,
                MaBaiVietGoc = maBaiVietGoc,
                NoiDung = string.IsNullOrWhiteSpace(loiBinh) ? "Đã chia sẻ một bài viết." : loiBinh.Trim(),
                NgayDang = DateTime.Now,
                TrangThai = "DaDuyet"
            };

            _context.BaiViets.Add(sharedPost);
            originalPost.SoChiaSe++;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Đã chia sẻ bài viết lên bảng tin thành công!",
                soChiaSe = originalPost.SoChiaSe
            });
        }

        // Helper định dạng thời gian thân thiện (Ví dụ: "Vừa xong", "5 phút trước", "2 giờ trước")
        private static string FormatTimeAgo(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalMinutes < 1) return "Vừa xong";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} phút trước";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} giờ trước";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays} ngày trước";
            return dt.ToString("dd/MM/yyyy");
        }
    }
}
