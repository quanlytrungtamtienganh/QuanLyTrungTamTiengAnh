// ============================================================================



// [MODULE 1 - VŨ TRƯỜNG AN: TÀI KHOẢN – ĐĂNG NHẬP VÀ PHÂN QUYỀN – QUẢN LÝ KHÓA HỌC]



// Tên Tệp: TaiKhoanController.cs



// Mô tả: Chức năng: Quản lý tài khoản người dùng, Đăng nhập, Đăng ký, Phân quyền vai trò (Admin, GiaoVien, HocVien)



// ============================================================================







using Microsoft.AspNetCore.Mvc;



using Microsoft.EntityFrameworkCore;



using TrungTamNgoaiNgu.Data;



using TrungTamNgoaiNgu.Filters;



using TrungTamNgoaiNgu.Models;



using TrungTamNgoaiNgu.Services;



using TrungTamNgoaiNgu.ViewModels;







namespace TrungTamNgoaiNgu.Controllers



{



    public class TaiKhoanController : Controller



    {



        private readonly TrungTamDbContext _context;



        private readonly IEmailService _emailService;



        private readonly ILogger<TaiKhoanController> _logger;







        public TaiKhoanController(TrungTamDbContext context, IEmailService emailService, ILogger<TaiKhoanController> logger)



        {



            _context = context;



            _emailService = emailService;



            _logger = logger;



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







        // POST: /TaiKhoan/Delete/5 hoặc /TaiKhoan/DeleteConfirmed



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







            if (user == null)



            {



                TempData["Error"] = "Không tìm thấy thông tin tài khoản cần xóa!";



                return RedirectToAction(nameof(Index));



            }







            try



            {



                // 1. Dọn dẹp tương tác, bài viết, bình luận, chat của tài khoản để không vi phạm ràng buộc khóa ngoại



                var thichs = _context.ThichBaiViets.Where(t => t.MaTaiKhoan == id);



                _context.ThichBaiViets.RemoveRange(thichs);







                var tvNhoms = _context.ThanhVienNhoms.Where(t => t.MaTaiKhoan == id);



                _context.ThanhVienNhoms.RemoveRange(tvNhoms);







                var otps = _context.OtpDoiMatKhaus.Where(o => o.MaTaiKhoan == id);



                _context.OtpDoiMatKhaus.RemoveRange(otps);







                var lichSus = _context.LichSuDoiMatKhaus.Where(l => l.MaTaiKhoan == id);



                _context.LichSuDoiMatKhaus.RemoveRange(lichSus);







                var tinNhans = _context.TinNhans.Where(t => t.NguoiGuiId == id || t.NguoiNhanId == id);



                _context.TinNhans.RemoveRange(tinNhans);







                var binhLuans = _context.BinhLuans.Where(b => b.MaTaiKhoan == id);



                _context.BinhLuans.RemoveRange(binhLuans);







                var baiViets = _context.BaiViets.Where(b => b.MaTaiKhoan == id);



                _context.BaiViets.RemoveRange(baiViets);







                // 2. Dọn dẹp đơn đăng ký học nếu là học viên



                if (user.HocVien != null)



                {



                    if (user.HocVien.DangKyHocs != null && user.HocVien.DangKyHocs.Any())



                    {



                        _context.DangKyHocs.RemoveRange(user.HocVien.DangKyHocs);



                    }



                    _context.HocViens.Remove(user.HocVien);



                }







                // 3. Nếu là giảng viên đang phụ trách lớp học, gỡ phân công giảng viên để lớp học bảo toàn



                if (user.GiaoVien != null)



                {



                    if (user.GiaoVien.LopHocs != null && user.GiaoVien.LopHocs.Any())



                    {



                        foreach (var lop in user.GiaoVien.LopHocs)



                        {



                            lop.MaGiaoVien = null;



                        }



                    }



                    _context.GiaoViens.Remove(user.GiaoVien);



                }







                // 4. Xóa tài khoản



                _context.TaiKhoans.Remove(user);



                await _context.SaveChangesAsync();







                TempData["Success"] = $"Đã xóa tài khoản '{user.TenDangNhap}' thành công!";



            }



            catch (Exception ex)



            {



                _logger.LogError(ex, "Lỗi khi xóa tài khoản {TenDangNhap}", user.TenDangNhap);



                TempData["Error"] = $"Không thể xóa tài khoản '{user.TenDangNhap}': {ex.Message}";



            }







            return RedirectToAction(nameof(Index));



        }







        // =========================================================================



        // CHỨC NĂNG: ĐỔI MẬT KHẨU / QUÊN MẬT KHẨU QUA MÃ OTP GMAIL (5 PHÚT - 5 LẦN/NGÀY)



        // =========================================================================







        // GET: /TaiKhoan/DoiMatKhau hoặc /TaiKhoan/QuenMatKhau



        [HttpGet]



        public IActionResult DoiMatKhau(string? email)



        {



            var model = new GuiYeuCauOtpViewModel();







            // Nếu người dùng đang đăng nhập, tự động lấy email của họ



            var sessionMaTk = HttpContext.Session.GetInt32("MaTaiKhoan");



            if (sessionMaTk.HasValue && string.IsNullOrEmpty(email))



            {



                var currentUser = _context.TaiKhoans.Find(sessionMaTk.Value);



                if (currentUser != null)



                {



                    model.Email = currentUser.Email;



                }



            }



            else if (!string.IsNullOrEmpty(email))



            {



                model.Email = email;



            }







            return View(model);



        }







        [HttpGet]



        public IActionResult QuenMatKhau(string? email)



        {



            return RedirectToAction(nameof(DoiMatKhau), new { email });



        }







        // POST: /TaiKhoan/DoiMatKhau (Gửi mã OTP về Gmail)



        [HttpPost]



        [ValidateAntiForgeryToken]



        public async Task<IActionResult> DoiMatKhau(GuiYeuCauOtpViewModel model)



        {



            if (!ModelState.IsValid)



            {



                return View(model);



            }







            var cleanEmail = (model.Email ?? "").Trim().ToLower();







            // 1. So sánh có đúng Gmail đã lưu trong hệ thống không



            var user = await _context.TaiKhoans



                .FirstOrDefaultAsync(t => t.Email.ToLower() == cleanEmail);







            if (user == null)



            {



                // Sai báo Gmail không tồn tại



                ModelState.AddModelError("Email", "Địa chỉ Gmail không tồn tại trong hệ thống! Vui lòng kiểm tra lại.");



                return View(model);



            }







            // 2. GIỚI HẠN SỐ LẦN GỬI MÃ OTP TRONG NGÀY: TỐI ĐA 5 LẦN / 1 NGÀY



            var today = DateTime.Today;



            var tomorrow = today.AddDays(1);







            var soLanGuiOtpHomNay = await _context.OtpDoiMatKhaus



                .CountAsync(o => o.MaTaiKhoan == user.MaTaiKhoan && o.ThoiGianTao >= today && o.ThoiGianTao < tomorrow);







            if (soLanGuiOtpHomNay >= 5)



            {



                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã đạt giới hạn tối đa 5 lần gửi mã OTP trong ngày hôm nay (5/5 lần). Vui lòng quay lại vào ngày mai!");



                return View(model);



            }







            // 3. Vô hiệu hóa TẤT CẢ các mã OTP cũ chưa sử dụng (Đảm bảo mỗi lần chỉ có duy nhất 1 mã OTP có hiệu lực)



            var oldOtps = await _context.OtpDoiMatKhaus



                .Where(o => o.MaTaiKhoan == user.MaTaiKhoan && !o.DaSuDung)



                .ToListAsync();



            foreach (var o in oldOtps)



            {



                o.DaSuDung = true;



            }







            // 4. Sinh DUY NHẤT 1 MÃ OTP ngẫu nhiên 6 chữ số



            var otpCode = Random.Shared.Next(100000, 999999).ToString();



            var now = DateTime.Now;







            // Mã OTP có hạn sử dụng chính xác 5 phút tính từ thời gian gửi mã



            var otpRecord = new OtpDoiMatKhau



            {



                MaTaiKhoan = user.MaTaiKhoan,



                Email = user.Email,



                MaOtp = otpCode,



                ThoiGianTao = now,



                ThoiGianHetHan = now.AddMinutes(5),



                DaSuDung = false



            };







            _context.OtpDoiMatKhaus.Add(otpRecord);



            await _context.SaveChangesAsync();







            // 5. Gửi duy nhất 1 email chứa mã OTP về Gmail qua EmailService



            bool sent = await _emailService.SendPasswordResetOtpEmailAsync(user.Email, user.HoTen, otpCode, 5);



            if (!sent)



            {



                _logger.LogWarning("Không thể gửi email OTP qua SMTP. Mã OTP: {OtpCode}", otpCode);



                TempData["OfflineOtpAlert"] = $"[THÔNG BÁO]: Mã OTP của bạn là: {otpCode} (Hết hạn sau 5 phút)";



            }







            int lanGuiHienTai = soLanGuiOtpHomNay + 1;



            TempData["Success"] = $"Mã OTP đã được gửi về Gmail {user.Email}. (Lượt gửi: {lanGuiHienTai}/5 lần hôm nay). Mã có hiệu lực trong 5 phút!";



            return RedirectToAction(nameof(XacNhanDoiMatKhau), new { email = user.Email });



        }







        // GET: /TaiKhoan/XacNhanDoiMatKhau



        [HttpGet]



        public async Task<IActionResult> XacNhanDoiMatKhau(string email)



        {



            if (string.IsNullOrEmpty(email))



            {



                return RedirectToAction(nameof(DoiMatKhau));



            }







            var cleanEmail = email.Trim().ToLower();



            var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == cleanEmail);



            if (user == null)



            {



                TempData["Error"] = "Không tìm thấy thông tin tài khoản với email này!";



                return RedirectToAction(nameof(DoiMatKhau));



            }







            // Lấy mã OTP mới nhất chưa dùng để tính số giây đếm ngược còn lại



            var latestOtp = await _context.OtpDoiMatKhaus



                .Where(o => o.MaTaiKhoan == user.MaTaiKhoan && !o.DaSuDung)



                .OrderByDescending(o => o.ThoiGianTao)



                .FirstOrDefaultAsync();







            int remainingSeconds = 0;



            if (latestOtp != null && latestOtp.ThoiGianHetHan > DateTime.Now)



            {



                remainingSeconds = (int)(latestOtp.ThoiGianHetHan - DateTime.Now).TotalSeconds;



            }







            var today = DateTime.Today;



            var tomorrow = today.AddDays(1);



            var soLanGuiOtpHomNay = await _context.OtpDoiMatKhaus



                .CountAsync(o => o.MaTaiKhoan == user.MaTaiKhoan && o.ThoiGianTao >= today && o.ThoiGianTao < tomorrow);







            var model = new XacNhanDoiMatKhauViewModel



            {



                Email = user.Email,



                RemainingSeconds = remainingSeconds,



                SoLanGuiOtpHomNay = soLanGuiOtpHomNay,



                SoLanDaDoiHomNay = soLanGuiOtpHomNay



            };







            return View(model);



        }







        // POST: /TaiKhoan/XacNhanDoiMatKhau



        [HttpPost]



        [ValidateAntiForgeryToken]



        public async Task<IActionResult> XacNhanDoiMatKhau(XacNhanDoiMatKhauViewModel model)



        {



            var cleanEmail = (model.Email ?? "").Trim().ToLower();



            var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == cleanEmail);



            if (user == null)



            {



                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại!");



                return View(model);



            }







            var today = DateTime.Today;



            var tomorrow = today.AddDays(1);



            var soLanGuiOtpHomNay = await _context.OtpDoiMatKhaus



                .CountAsync(o => o.MaTaiKhoan == user.MaTaiKhoan && o.ThoiGianTao >= today && o.ThoiGianTao < tomorrow);



            model.SoLanGuiOtpHomNay = soLanGuiOtpHomNay;



            model.SoLanDaDoiHomNay = soLanGuiOtpHomNay;







            if (!ModelState.IsValid)



            {



                return View(model);



            }







            // Tìm mã OTP mới nhất của tài khoản này



            var otpRecord = await _context.OtpDoiMatKhaus



                .Where(o => o.MaTaiKhoan == user.MaTaiKhoan && o.Email.ToLower() == cleanEmail && !o.DaSuDung)



                .OrderByDescending(o => o.ThoiGianTao)



                .FirstOrDefaultAsync();







            if (otpRecord == null)



            {



                ModelState.AddModelError("MaOtp", "Không tìm thấy mã OTP hoặc mã đã được sử dụng. Vui lòng nhấn 'Gửi lại mã OTP'!");



                model.RemainingSeconds = 0;



                return View(model);



            }







            // Kiểm tra hạn sử dụng chính xác 5 phút



            if (DateTime.Now > otpRecord.ThoiGianHetHan)



            {



                ModelState.AddModelError("MaOtp", "Mã OTP đã hết hạn sử dụng (quá 5 phút tính từ lúc gửi mã). Vui lòng nhấn 'Gửi lại mã OTP' để lấy mã mới!");



                model.RemainingSeconds = 0;



                return View(model);



            }







            // Kiểm tra tính chính xác của mã OTP



            if (otpRecord.MaOtp.Trim() != model.MaOtp.Trim())



            {



                ModelState.AddModelError("MaOtp", "Mã OTP không chính xác! Vui lòng kiểm tra lại hòm thư Gmail của bạn.");



                model.RemainingSeconds = Math.Max(0, (int)(otpRecord.ThoiGianHetHan - DateTime.Now).TotalSeconds);



                return View(model);



            }







            // ĐÚNG MÃ OTP -> TIẾN HÀNH ĐỔI MẬT KHẨU VÀ CẬP NHẬT TRONG HỆ THỐNG



            user.MatKhau = model.MatKhauMoi;



            otpRecord.DaSuDung = true;







            // Ghi nhận lịch sử đổi mật khẩu thành công



            var lichSu = new LichSuDoiMatKhau



            {



                MaTaiKhoan = user.MaTaiKhoan,



                Email = user.Email,



                NgayDoi = DateTime.Today,



                ThoiGianDoi = DateTime.Now



            };



            _context.LichSuDoiMatKhaus.Add(lichSu);







            await _context.SaveChangesAsync();







            TempData["Success"] = $"Chúc mừng {user.HoTen}! Bạn đã đổi mật khẩu thành công. Mật khẩu mới đã được cập nhật, vui lòng đăng nhập lại.";



            return RedirectToAction(nameof(Login));



        }







        // POST: /TaiKhoan/GuiLaiOtp (Hỗ trợ nút 'Gửi lại mã' khi đang ở trang nhập OTP)



        [HttpPost]



        [ValidateAntiForgeryToken]



        public async Task<IActionResult> GuiLaiOtp(string email)



        {



            if (string.IsNullOrEmpty(email))



            {



                return RedirectToAction(nameof(DoiMatKhau));



            }







            var cleanEmail = email.Trim().ToLower();



            var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == cleanEmail);



            if (user == null)



            {



                TempData["Error"] = "Email không tồn tại trong hệ thống!";



                return RedirectToAction(nameof(DoiMatKhau));



            }







            var today = DateTime.Today;



            var tomorrow = today.AddDays(1);



            var soLanGuiOtpHomNay = await _context.OtpDoiMatKhaus



                .CountAsync(o => o.MaTaiKhoan == user.MaTaiKhoan && o.ThoiGianTao >= today && o.ThoiGianTao < tomorrow);







            if (soLanGuiOtpHomNay >= 5)



            {



                TempData["Error"] = "Tài khoản của bạn đã đạt giới hạn tối đa 5 lần gửi mã OTP trong ngày hôm nay (5/5 lần). Vui lòng quay lại vào ngày mai!";



                return RedirectToAction(nameof(XacNhanDoiMatKhau), new { email = user.Email });



            }







            // Vô hiệu hóa các mã OTP cũ



            var oldOtps = await _context.OtpDoiMatKhaus



                .Where(o => o.MaTaiKhoan == user.MaTaiKhoan && !o.DaSuDung)



                .ToListAsync();



            foreach (var o in oldOtps)



            {



                o.DaSuDung = true;



            }







            // Sinh DUY NHẤT 1 mã OTP mới



            var otpCode = Random.Shared.Next(100000, 999999).ToString();



            var now = DateTime.Now;







            var otpRecord = new OtpDoiMatKhau



            {



                MaTaiKhoan = user.MaTaiKhoan,



                Email = user.Email,



                MaOtp = otpCode,



                ThoiGianTao = now,



                ThoiGianHetHan = now.AddMinutes(5),



                DaSuDung = false



            };







            _context.OtpDoiMatKhaus.Add(otpRecord);



            await _context.SaveChangesAsync();







            bool sent = await _emailService.SendPasswordResetOtpEmailAsync(user.Email, user.HoTen, otpCode, 5);



            if (!sent)



            {



                TempData["OfflineOtpAlert"] = $"[THÔNG BÁO]: Mã OTP mới của bạn là: {otpCode} (Hết hạn sau 5 phút)";



            }







            int lanMoi = soLanGuiOtpHomNay + 1;



            TempData["Success"] = $"Mã OTP mới đã được gửi về Gmail của bạn! (Lượt gửi: {lanMoi}/5 lần hôm nay). Mã có hiệu lực trong 5 phút.";



            return RedirectToAction(nameof(XacNhanDoiMatKhau), new { email = user.Email });



        }







    }

}

