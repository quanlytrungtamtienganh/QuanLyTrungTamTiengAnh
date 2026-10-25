// ============================================================================
// [MODULE MỞ RỘNG (NÂNG CAO): LUYỆN THI TRỰC TUYẾN ĐA NGÔN NGỮ]
// Tên Tệp: LuyenThiController.cs
// Mô tả: Chức năng: Thi thử trắc nghiệm (IELTS, TOEIC, TOPIK, HSK, JLPT), Luyện tập từng phần, Bấm giờ, Chấm điểm tự động, Bình luận đề thi
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Models;
using TrungTamNgoaiNgu.ViewModels;

namespace TrungTamNgoaiNgu.Controllers
{
    public class LuyenThiController : Controller
    {
        private readonly TrungTamDbContext _context;

        public LuyenThiController(TrungTamDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. THƯ VIỆN ĐỀ THI (Giống 100% Ảnh 1 Study4)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? theLoai, string? tuKhoa, string? tab)
        {
            var query = _context.DeThis
                .AsNoTracking()
                .Where(d => d.TrangThai)
                .AsQueryable();

            // Lọc theo thể loại
            if (!string.IsNullOrEmpty(theLoai) && theLoai != "TatCa")
            {
                query = query.Where(d => d.TheLoai.ToLower().Contains(theLoai.ToLower()) ||
                                         d.KyNang.ToLower().Contains(theLoai.ToLower()));
            }

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                var kw = tuKhoa.Trim().ToLower();
                query = query.Where(d => d.TieuDe.ToLower().Contains(kw) ||
                                         (d.MoTa != null && d.MoTa.ToLower().Contains(kw)) ||
                                         d.TheLoai.ToLower().Contains(kw));
            }

            var deThis = await query
                .Include(d => d.PhanThis)
                .Include(d => d.BinhLuans)
                .AsSplitQuery()
                .OrderByDescending(d => d.SoLuotLam)
                .ToListAsync();

            // Lịch sử làm bài gần đây của user (nếu đã đăng nhập)
            var sessionUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            List<KetQuaLuyenThi> lichSu = new List<KetQuaLuyenThi>();
            if (sessionUserId.HasValue)
            {
                lichSu = await _context.KetQuaLuyenThis
                    .Include(k => k.DeThi)
                    .Where(k => k.MaTaiKhoan == sessionUserId.Value)
                    .OrderByDescending(k => k.NgayNop)
                    .Take(5)
                    .ToListAsync();
            }

            var viewModel = new LuyenThiIndexViewModel
            {
                DeThis = deThis,
                TheLoaiHienTai = theLoai ?? "TatCa",
                TuKhoa = tuKhoa,
                Tab = tab ?? "TatCa",
                TongSoDe = deThis.Count,
                LichSuLamBaiGanDay = lichSu
            };

            return View(viewModel);
        }

        // =========================================================================
        // 2. CHI TIẾT ĐỀ THI & CHỌN CHẾ ĐỘ (Ảnh 2 & 3 Study4)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var deThi = await _context.DeThis
                .AsNoTracking()
                .Include(d => d.PhanThis)
                .Include(d => d.BinhLuans)
                    .ThenInclude(b => b.TaiKhoan)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.MaDeThi == id);

            if (deThi == null)
            {
                return NotFound("Không tìm thấy đề thi yêu cầu.");
            }

            var viewModel = new ChiTietDeThiViewModel
            {
                DeThi = deThi,
                PhanThis = deThi.PhanThis.OrderBy(p => p.ThuTu).ToList(),
                BinhLuans = deThi.BinhLuans.OrderByDescending(b => b.NgayBinhLuan).ToList(),
                TongSoCauHoi = deThi.TongSoCau > 0 ? deThi.TongSoCau : 200,
                TongSoBinhLuan = deThi.BinhLuans.Count
            };

            return View(viewModel);
        }

        // =========================================================================
        // 3. THÊM BÌNH LUẬN DƯỚI BÀI KIỂM TRA (Ảnh 3 Study4)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBinhLuan(int maDeThi, string noiDung, int? maBinhLuanCha)
        {
            var sessionUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!sessionUserId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để gửi bình luận!" });
            }

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                return Json(new { success = false, message = "Nội dung bình luận không được để trống!" });
            }

            var binhLuan = new BinhLuanDeThi
            {
                MaDeThi = maDeThi,
                MaTaiKhoan = sessionUserId.Value,
                NoiDung = noiDung.Trim(),
                NgayBinhLuan = DateTime.Now,
                MaBinhLuanCha = maBinhLuanCha
            };

            _context.BinhLuanDeThis.Add(binhLuan);
            await _context.SaveChangesAsync();

            var user = await _context.TaiKhoans.FindAsync(sessionUserId.Value);

            return Json(new
            {
                success = true,
                message = "Đã gửi bình luận thành công!",
                binhLuan = new
                {
                    maBinhLuan = binhLuan.MaBinhLuan,
                    tenNguoiDung = user?.HoTen ?? user?.TenDangNhap ?? "Học viên",
                    noiDung = binhLuan.NoiDung,
                    thoiGian = binhLuan.NgayBinhLuan.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }

        // Static thread-safe dictionary lưu trữ vé phiên thi đang hoạt động (kèm cơ chế tự dọn dẹp)
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, ExamTicket> _activeTickets 
            = new System.Collections.Concurrent.ConcurrentDictionary<string, ExamTicket>();

        // =========================================================================
        // 4.1. KHỞI TẠO PHIÊN THI AN TOÀN (POST - BẢO VỆ CHỐNG THAY ĐỔI URL & THỜI GIAN)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDauThi([FromForm] KhoiTaoThiInputModel model)
        {
            if (model == null || model.MaDeThi <= 0)
            {
                TempData["ErrorMessage"] = "Yêu cầu bắt đầu làm bài không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var deThi = await _context.DeThis
                .AsNoTracking()
                .Include(d => d.PhanThis)
                .FirstOrDefaultAsync(d => d.MaDeThi == model.MaDeThi && d.TrangThai);

            if (deThi == null)
            {
                TempData["ErrorMessage"] = "Đề thi không tồn tại hoặc đã ngừng kích hoạt.";
                return RedirectToAction(nameof(Index));
            }

            // Dọn dẹp vé phiên thi cũ hơn 24 giờ
            var nowUtc = DateTime.UtcNow;
            foreach (var item in _activeTickets)
            {
                if (nowUtc.Subtract(item.Value.StartTimeUtc).TotalHours > 24)
                {
                    _activeTickets.TryRemove(item.Key, out _);
                }
            }

            var allParts = deThi.PhanThis.OrderBy(p => p.ThuTu).ToList();
            List<PhanThi> selectedParts;
            int finalThoiGian;
            string cheDo = model.CheDo == "FullTest" ? "FullTest" : "LuyenTap";

            if (cheDo == "FullTest")
            {
                // BẢO VỆ CHẶT CHẼ: Full Test BẮT BUỘC dùng đúng thời gian chuẩn của đề thi (người dùng không thể can thiệp)
                finalThoiGian = deThi.ThoiGianPhut > 0 ? deThi.ThoiGianPhut : 120;
                selectedParts = allParts;
            }
            else
            {
                // BẢO VỆ CHẶT CHẼ: Luyện tập chỉ chấp nhận danh sách thời gian hợp lệ (Whitelist)
                var allowedMinutes = new HashSet<int> { 0, 10, 15, 20, 30, 40, 60 };
                if (model.ThoiGianPhut.HasValue && allowedMinutes.Contains(model.ThoiGianPhut.Value))
                {
                    finalThoiGian = model.ThoiGianPhut.Value;
                }
                else
                {
                    finalThoiGian = 40; // Giá trị an toàn mặc định
                }

                // Lọc danh sách phần thi: chỉ chấp nhận ID thuộc về chính đề thi này
                if (!string.IsNullOrEmpty(model.PhanThiIds))
                {
                    var idList = model.PhanThiIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out int val) ? val : 0)
                        .Where(val => val > 0)
                        .ToList();

                    selectedParts = allParts.Where(p => idList.Contains(p.MaPhanThi)).ToList();
                    if (!selectedParts.Any())
                    {
                        selectedParts = allParts;
                    }
                }
                else
                {
                    selectedParts = allParts;
                }
            }

            // Sinh Token bảo mật (GUID ngẫu nhiên ẩn toàn bộ tham số khỏi thanh địa chỉ)
            string ticketId = Guid.NewGuid().ToString("N");
            var ticket = new ExamTicket
            {
                TicketId = ticketId,
                MaDeThi = deThi.MaDeThi,
                CheDo = cheDo,
                PhanThiIds = selectedParts.Select(p => p.MaPhanThi).ToList(),
                ThoiGianPhut = finalThoiGian,
                StartTimeUtc = nowUtc,
                ExpireTimeUtc = finalThoiGian > 0 ? nowUtc.AddMinutes(finalThoiGian) : null,
                RemainingSeconds = finalThoiGian > 0 ? finalThoiGian * 60 : 0
            };

            _activeTickets[ticketId] = ticket;
            HttpContext.Session.SetString("ActiveExamTicket", ticketId);

            // Điều hướng sang Phòng thi với Token bảo vệ
            return RedirectToAction(nameof(PhongThi), new { t = ticketId });
        }

        // =========================================================================
        // 4.2. MÀN HÌNH PHÒNG THI ĐƯỢC BẢO VỆ (KHÔNG LỘ THAM SỐ TRÊN THANH ĐỊA CHỈ)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> PhongThi(string? t)
        {
            string? ticketId = t;
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = HttpContext.Session.GetString("ActiveExamTicket");
            }

            if (string.IsNullOrEmpty(ticketId) || !_activeTickets.TryGetValue(ticketId, out var ticket))
            {
                TempData["ErrorMessage"] = "Phiên làm bài không tồn tại hoặc đã hết hạn. Vui lòng chọn đề thi để bắt đầu làm bài.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra mốc thời gian thực từ Server (chống đóng băng client hoặc sửa URL)
            int remainingSeconds = 0;
            if (ticket.ExpireTimeUtc.HasValue)
            {
                var remain = (ticket.ExpireTimeUtc.Value - DateTime.UtcNow).TotalSeconds;
                if (remain <= 0)
                {
                    TempData["ErrorMessage"] = "Thời gian làm bài của phiên thi này đã kết thúc!";
                    return RedirectToAction(nameof(ChiTiet), new { id = ticket.MaDeThi });
                }
                remainingSeconds = (int)Math.Ceiling(remain);
            }

            var deThi = await _context.DeThis
                .AsNoTracking()
                .Include(d => d.PhanThis)
                    .ThenInclude(p => p.CauHois)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.MaDeThi == ticket.MaDeThi);

            if (deThi == null)
            {
                return NotFound("Không tìm thấy dữ liệu đề thi.");
            }

            var allParts = deThi.PhanThis.OrderBy(p => p.ThuTu).ToList();
            var selectedParts = allParts.Where(p => ticket.PhanThiIds.Contains(p.MaPhanThi)).ToList();
            if (!selectedParts.Any()) selectedParts = allParts;

            var viewModel = new LamBaiViewModel
            {
                TicketId = ticket.TicketId,
                DeThi = deThi,
                PhanThis = selectedParts,
                CheDo = ticket.CheDo,
                PhanThiIds = ticket.PhanThiIds,
                ThoiGianPhut = ticket.ThoiGianPhut,
                RemainingSeconds = remainingSeconds,
                TongSoCau = selectedParts.Sum(p => p.CauHois.Count)
            };

            return View("LamBai", viewModel);
        }

        // =========================================================================
        // 4.3. ĐIỀU HƯỚNG BẢO VỆ CHO ĐƯỜNG DẪN CŨ (/LuyenThi/LamBai/{id})
        // Nếu ai cố tình gõ sửa tham số trên URL cũ, server lập tức khử trùng và ép về PhongThi
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> LamBai(int id, string? cheDo, string? phanThiIds, int? thoiGianPhut)
        {
            var deThi = await _context.DeThis
                .AsNoTracking()
                .Include(d => d.PhanThis)
                .FirstOrDefaultAsync(d => d.MaDeThi == id && d.TrangThai);

            if (deThi == null)
            {
                return NotFound("Không tìm thấy đề thi.");
            }

            // BẢO VỆ CHỐNG SỬA THANH ĐỊA CHỈ:
            // Tự động kiểm tra và chuẩn hóa sang Exam Ticket an toàn
            var allowedMinutes = new HashSet<int> { 0, 10, 15, 20, 30, 40, 60 };
            int safeMinutes;
            string safeCheDo = (cheDo == "FullTest") ? "FullTest" : "LuyenTap";

            if (safeCheDo == "FullTest")
            {
                safeMinutes = deThi.ThoiGianPhut > 0 ? deThi.ThoiGianPhut : 120;
            }
            else
            {
                safeMinutes = (thoiGianPhut.HasValue && allowedMinutes.Contains(thoiGianPhut.Value)) 
                    ? thoiGianPhut.Value 
                    : 40;
            }

            var allPartIds = deThi.PhanThis.Select(p => p.MaPhanThi).ToList();
            List<int> safePartIds = allPartIds;
            if (!string.IsNullOrEmpty(phanThiIds) && safeCheDo != "FullTest")
            {
                var requestedIds = phanThiIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out int val) ? val : 0)
                    .Where(val => val > 0 && allPartIds.Contains(val))
                    .ToList();
                if (requestedIds.Any()) safePartIds = requestedIds;
            }

            string ticketId = Guid.NewGuid().ToString("N");
            var nowUtc = DateTime.UtcNow;
            var ticket = new ExamTicket
            {
                TicketId = ticketId,
                MaDeThi = deThi.MaDeThi,
                CheDo = safeCheDo,
                PhanThiIds = safePartIds,
                ThoiGianPhut = safeMinutes,
                StartTimeUtc = nowUtc,
                ExpireTimeUtc = safeMinutes > 0 ? nowUtc.AddMinutes(safeMinutes) : null,
                RemainingSeconds = safeMinutes > 0 ? safeMinutes * 60 : 0
            };

            _activeTickets[ticketId] = ticket;
            HttpContext.Session.SetString("ActiveExamTicket", ticketId);

            // Chuyển hướng ngay lập tức sang URL phòng thi bảo vệ
            return RedirectToAction(nameof(PhongThi), new { t = ticketId });
        }

        // =========================================================================
        // 5. NỘP BÀI & CHẤM ĐIỂM TỰ ĐỘNG
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> NopBai([FromBody] NopBaiInputModel model)
        {
            if (model == null || model.MaDeThi <= 0)
            {
                return BadRequest("Dữ liệu nộp bài không hợp lệ.");
            }

            var deThi = await _context.DeThis
                .Include(d => d.PhanThis)
                    .ThenInclude(p => p.CauHois)
                .FirstOrDefaultAsync(d => d.MaDeThi == model.MaDeThi);

            if (deThi == null)
            {
                return NotFound("Không tìm thấy đề thi.");
            }

            // Lấy tất cả các câu hỏi thuộc bài làm
            var allQuestions = deThi.PhanThis
                .SelectMany(p => p.CauHois)
                .ToList();

            // Nếu chỉ làm một số phần, lọc danh sách câu hỏi theo các câu có trong Answers hoặc các phần đã chọn
            List<CauHoiLuyenThi> targetQuestions;
            if (!string.IsNullOrEmpty(model.CacPhanDaLam) && model.CacPhanDaLam != "Toàn bộ")
            {
                var partNames = model.CacPhanDaLam.Split(',', StringSplitOptions.TrimEntries).ToList();
                targetQuestions = deThi.PhanThis
                    .Where(p => partNames.Contains(p.TenPhanThi))
                    .SelectMany(p => p.CauHois)
                    .ToList();
                if (!targetQuestions.Any()) targetQuestions = allQuestions;
            }
            else
            {
                targetQuestions = allQuestions;
            }

            int soDung = 0, soSai = 0, soBoQua = 0;
            var chiTietList = new Dictionary<int, object>();

            foreach (var q in targetQuestions)
            {
                model.Answers.TryGetValue(q.MaCauHoi, out string? userAnswer);
                userAnswer = userAnswer?.Trim() ?? "";

                bool isCorrect = false;
                bool isSkip = string.IsNullOrEmpty(userAnswer);

                if (isSkip)
                {
                    soBoQua++;
                }
                else
                {
                    if (q.LoaiCauHoi == "TracNghiem")
                    {
                        isCorrect = string.Equals(userAnswer, q.DapAnDung?.Trim(), StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // Dạng điền từ: Kiểm tra tương đối linh hoạt
                        var cleanUser = userAnswer.ToLower().Replace("-", " ").Trim();
                        var cleanCorrect = (q.DapAnDung ?? "").ToLower().Replace("-", " ").Trim();

                        isCorrect = cleanUser == cleanCorrect ||
                                   cleanUser.Contains(cleanCorrect) ||
                                   cleanCorrect.Contains(cleanUser);
                    }

                    if (isCorrect) soDung++;
                    else soSai++;
                }

                chiTietList[q.MaCauHoi] = new
                {
                    UserAnswer = userAnswer,
                    IsCorrect = isCorrect,
                    IsSkip = isSkip
                };
            }

            int tongSoCau = targetQuestions.Count;
            double doChinhXac = tongSoCau > 0 ? Math.Round((double)soDung / tongSoCau * 100, 1) : 0;
            double diemSo = tongSoCau > 0 ? Math.Round((double)soDung / tongSoCau * 10, 1) : 0;

            var sessionUserId = HttpContext.Session.GetInt32("MaTaiKhoan");

            int thoiGianGhiNhan = model.ThoiGianLamGiay;
            if (!string.IsNullOrEmpty(model.TicketId) && _activeTickets.TryGetValue(model.TicketId, out var ticket))
            {
                // Xác thực đề thi nộp phải khớp với đề thi của ticket
                if (ticket.MaDeThi != model.MaDeThi)
                {
                    return BadRequest("Dữ liệu nộp bài không khớp với phiên làm bài hợp lệ.");
                }

                // Tính thời gian làm bài thực tế từ mốc Server Time (chống hack thời gian từ Client)
                int serverElapsedSeconds = (int)Math.Max(1, (DateTime.UtcNow - ticket.StartTimeUtc).TotalSeconds);
                thoiGianGhiNhan = model.ThoiGianLamGiay > 0 ? Math.Min(model.ThoiGianLamGiay, serverElapsedSeconds) : serverElapsedSeconds;

                // Xóa ticket đã hoàn thành để chống nộp bài lặp lại
                _activeTickets.TryRemove(model.TicketId, out _);
            }

            var ketQua = new KetQuaLuyenThi
            {
                MaDeThi = model.MaDeThi,
                MaTaiKhoan = sessionUserId,
                CheDoLam = model.CheDo ?? "LuyenTap",
                CacPhanDaLam = model.CacPhanDaLam ?? "Toàn bộ",
                SoCauDung = soDung,
                SoCauSai = soSai,
                SoCauBoQua = soBoQua,
                TongSoCau = tongSoCau,
                DiemSo = diemSo,
                DoChinhXac = doChinhXac,
                ThoiGianLamGiay = thoiGianGhiNhan,
                NgayNop = DateTime.Now,
                ChiTietBaiLamJson = JsonSerializer.Serialize(chiTietList)
            };

            _context.KetQuaLuyenThis.Add(ketQua);

            // Tăng số lượt làm đề
            deThi.SoLuotLam++;
            await _context.SaveChangesAsync();

            return Json(new { success = true, ketQuaId = ketQua.MaKetQua });
        }

        // =========================================================================
        // 6. XEM CHI TIẾT KẾT QUẢ & LÝ DO GIẢI THÍCH (Ảnh 5 Study4)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> KetQua(int id)
        {
            var ketQua = await _context.KetQuaLuyenThis
                .AsNoTracking()
                .Include(k => k.DeThi)
                    .ThenInclude(d => d.PhanThis)
                        .ThenInclude(p => p.CauHois)
                .AsSplitQuery()
                .FirstOrDefaultAsync(k => k.MaKetQua == id);

            if (ketQua == null || ketQua.DeThi == null)
            {
                return NotFound("Không tìm thấy kết quả làm bài.");
            }

            // Parse JSON chi tiết bài làm
            var userAnswersMap = new Dictionary<int, JsonElement>();
            if (!string.IsNullOrEmpty(ketQua.ChiTietBaiLamJson))
            {
                try
                {
                    userAnswersMap = JsonSerializer.Deserialize<Dictionary<int, JsonElement>>(ketQua.ChiTietBaiLamJson)
                                    ?? new Dictionary<int, JsonElement>();
                }
                catch { }
            }

            var cauHoiItems = new List<CauHoiKetQuaItem>();
            var allQuestions = ketQua.DeThi.PhanThis
                .OrderBy(p => p.ThuTu)
                .SelectMany(p => p.CauHois.OrderBy(c => c.SoThuTu))
                .ToList();

            foreach (var q in allQuestions)
            {
                string userAnswer = "";
                bool isCorrect = false;
                bool isSkip = true;

                if (userAnswersMap.TryGetValue(q.MaCauHoi, out JsonElement el))
                {
                    if (el.TryGetProperty("UserAnswer", out var uProp)) userAnswer = uProp.GetString() ?? "";
                    if (el.TryGetProperty("IsCorrect", out var cProp)) isCorrect = cProp.GetBoolean();
                    if (el.TryGetProperty("IsSkip", out var sProp)) isSkip = sProp.GetBoolean();
                }

                // Nếu không có trong JSON thì coi như bỏ qua
                cauHoiItems.Add(new CauHoiKetQuaItem
                {
                    MaCauHoi = q.MaCauHoi,
                    SoThuTu = q.SoThuTu,
                    TenPhanThi = q.PhanThi?.TenPhanThi ?? "",
                    NoiDung = q.NoiDung,
                    LoaiCauHoi = q.LoaiCauHoi,
                    DapAnA = q.DapAnA,
                    DapAnB = q.DapAnB,
                    DapAnC = q.DapAnC,
                    DapAnD = q.DapAnD,
                    DapAnDung = q.DapAnDung,
                    DapAnCuaBan = userAnswer,
                    IsDung = isCorrect,
                    IsBoQua = isSkip,
                    GiaiThichChiTiet = q.GiaiThichChiTiet,
                    PhanLoaiDangBai = q.PhanLoaiDangBai ?? "[Tổng hợp] Câu hỏi"
                });
            }

            // Thống kê theo dạng bài / Part
            var nhomDangBai = cauHoiItems
                .GroupBy(c => c.PhanLoaiDangBai)
                .Select(g => new ThongKeDangBaiItem
                {
                    TenDangBai = g.Key,
                    SoCauDung = g.Count(x => x.IsDung),
                    SoCauSai = g.Count(x => !x.IsDung && !x.IsBoQua),
                    SoCauBoQua = g.Count(x => x.IsBoQua),
                    DanhSachSoCau = g.Select(x => new CauHoiBadgeItem
                    {
                        SoThuTu = x.SoThuTu,
                        MaCauHoi = x.MaCauHoi,
                        TrangThai = x.IsDung ? "Dung" : (x.IsBoQua ? "BoQua" : "Sai")
                    }).ToList()
                }).ToList();

            var viewModel = new KetQuaChiTietViewModel
            {
                KetQua = ketQua,
                DeThi = ketQua.DeThi,
                ThongKeDangBai = nhomDangBai,
                DanhSachCauHoi = cauHoiItems
            };

            return View(viewModel);
        }

        // =========================================================================
        // 7. LỊCH SỬ KẾT QUẢ THI CỦA HỌC VIÊN
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> LichSu()
        {
            var sessionUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!sessionUserId.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var list = await _context.KetQuaLuyenThis
                .Include(k => k.DeThi)
                .Where(k => k.MaTaiKhoan == sessionUserId.Value)
                .OrderByDescending(k => k.NgayNop)
                .ToListAsync();

            return View(list);
        }
    }
}
