// ============================================================================
// [MODULE MỞ RỘNG (NÂNG CAO): HỘI THOẠI REALTIME & VIDEO CALL WEBRTC]
// Tên Tệp: ChatController.cs
// Mô tả: Chức năng: Nhóm chat thời gian thực SignalR cho lớp học, Trò chuyện 1-1, Gọi video call trực tuyến chuẩn WebRTC
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Hubs;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Controllers
{
    public class ChatController : Controller
    {
        private readonly TrungTamDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<CallHub> _hubContext;

        public ChatController(TrungTamDbContext context, IWebHostEnvironment env, IHubContext<CallHub> hubContext)
        {
            _context = context;
            _env = env;
            _hubContext = hubContext;
        }

        // =========================================================================
        // 1. DANH SÁCH GIÁO VIÊN TRUNG TÂM (CHAT 1-1)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetTeachers()
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");

            var teachers = await _context.GiaoViens
                .Include(g => g.TaiKhoan)
                .OrderBy(g => g.HoTen)
                .ToListAsync();

            var rawTeachers = new List<dynamic>();

            foreach (var gv in teachers)
            {
                TinNhan? lastMsg = null;
                int unreadCount = 0;

                if (currentUserId.HasValue)
                {
                    lastMsg = await _context.TinNhans
                        .Where(m => m.MaNhom == null &&
                                    ((m.NguoiGuiId == currentUserId && m.NguoiNhanId == gv.MaTaiKhoan) ||
                                     (m.NguoiGuiId == gv.MaTaiKhoan && m.NguoiNhanId == currentUserId)))
                        .OrderByDescending(m => m.ThoiGian)
                        .FirstOrDefaultAsync();

                    unreadCount = await _context.TinNhans
                        .CountAsync(m => m.MaNhom == null && m.NguoiGuiId == gv.MaTaiKhoan && m.NguoiNhanId == currentUserId && !m.DaXem);
                }

                rawTeachers.Add(new
                {
                    maGiaoVien = gv.MaGiaoVien,
                    maTaiKhoan = gv.MaTaiKhoan,
                    hoTen = gv.HoTen,
                    chuyenMon = gv.ChuyenMon,
                    hocVi = gv.HocVi ?? "Giảng viên trung tâm",
                    anhDaiDien = !string.IsNullOrWhiteSpace(gv.AnhDaiDien) ? gv.AnhDaiDien : "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
                    isOnline = true,
                    lastMessage = lastMsg != null ? (string.IsNullOrEmpty(lastMsg.NoiDung) ? "[Đã gửi một ảnh]" : lastMsg.NoiDung) : "Bấm vào để bắt đầu cuộc trò chuyện...",
                    lastMessageTime = lastMsg != null ? lastMsg.ThoiGian.ToString("HH:mm") : "",
                    unreadCount = unreadCount,
                    lastTime = lastMsg?.ThoiGian
                });
            }

            // Sắp xếp người có tin nhắn gần nhất lên đầu
            var teacherList = rawTeachers
                .OrderByDescending(t => t.lastTime != null)
                .ThenByDescending(t => (DateTime?)t.lastTime)
                .ThenBy(t => (string)t.hoTen)
                .ToList();

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool canCreateGroup = vaiTro == "Admin" || vaiTro == "GiaoVien";

            return Json(new { 
                success = true, 
                teachers = teacherList, 
                isLoggedIn = currentUserId.HasValue,
                canCreateGroup = canCreateGroup,
                userRole = vaiTro
            });
        }

        // =========================================================================
        // 2. TIN NHẮN 1-1 VỚI GIÁO VIÊN
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetMessages(int targetUserId)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để trò chuyện với giảng viên!" });
            }

            var messages = await _context.TinNhans
                .Where(m => m.MaNhom == null &&
                            ((m.NguoiGuiId == currentUserId && m.NguoiNhanId == targetUserId) ||
                             (m.NguoiGuiId == targetUserId && m.NguoiNhanId == currentUserId)))
                .OrderBy(m => m.ThoiGian)
                .Select(m => new
                {
                    maTinNhan = m.MaTinNhan,
                    nguoiGuiId = m.NguoiGuiId,
                    nguoiNhanId = m.NguoiNhanId,
                    noiDung = m.NoiDung,
                    hinhAnhUrl = m.HinhAnhUrl,
                    loaiTinNhan = m.LoaiTinNhan,
                    thoiGian = m.ThoiGian.ToString("HH:mm - dd/MM"),
                    isMine = m.NguoiGuiId == currentUserId.Value
                })
                .ToListAsync();

            var unread = await _context.TinNhans
                .Where(m => m.MaNhom == null && m.NguoiGuiId == targetUserId && m.NguoiNhanId == currentUserId && !m.DaXem)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var msg in unread)
                {
                    msg.DaXem = true;
                }
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, messages });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int targetUserId, string? noiDung, IFormFile? imageFile)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập tài khoản học viên để gửi tin nhắn cho giáo viên!" });
            }

            if (string.IsNullOrWhiteSpace(noiDung) && imageFile == null)
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung hoặc chọn ảnh đính kèm!" });
            }

            string? uploadedImageUrl = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    return Json(new { success = false, message = "Định dạng ảnh không được hỗ trợ (chỉ nhận .jpg, .png, .gif, .webp)!" });
                }

                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "chat");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"chat_{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                uploadedImageUrl = $"/uploads/chat/{fileName}";
            }

            var message = new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                NguoiNhanId = targetUserId,
                NoiDung = noiDung?.Trim(),
                HinhAnhUrl = uploadedImageUrl,
                LoaiTinNhan = string.IsNullOrEmpty(uploadedImageUrl) ? "VanBan" : "HinhAnh",
                ThoiGian = DateTime.Now,
                DaXem = false
            };

            _context.TinNhans.Add(message);
            await _context.SaveChangesAsync();

            var sender = await _context.TaiKhoans.Include(t => t.GiaoVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);

            // Gửi thông báo SignalR thời gian thực đến người nhận
            try
            {
                await _hubContext.Clients.Group($"user_{targetUserId}").SendAsync("ReceiveDirectMessage", new
                {
                    targetUserId = targetUserId,
                    senderUserId = currentUserId.Value,
                    senderName = sender?.HoTen ?? "Người dùng",
                    senderAvatar = sender?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = message.NoiDung,
                    hinhAnhUrl = message.HinhAnhUrl,
                    loaiTinNhan = message.LoaiTinNhan,
                    thoiGian = message.ThoiGian.ToString("HH:mm - dd/MM"),
                    maTinNhan = message.MaTinNhan
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR Error] SendMessage: {ex.Message}");
            }

            return Json(new
            {
                success = true,
                message = new
                {
                    maTinNhan = message.MaTinNhan,
                    nguoiGuiId = message.NguoiGuiId,
                    nguoiNhanId = message.NguoiNhanId,
                    noiDung = message.NoiDung,
                    hinhAnhUrl = message.HinhAnhUrl,
                    loaiTinNhan = message.LoaiTinNhan,
                    thoiGian = message.ThoiGian.ToString("HH:mm - dd/MM"),
                    isMine = true
                }
            });
        }

        // =========================================================================
        // 3. QUẢN LÝ NHÓM CHAT (GROUP CHAT)
        // =========================================================================
        // GET: /Chat/GetGroups (Lấy danh sách nhóm chat của người dùng hiện tại)
        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = true, groups = new List<object>() });
            }

            var groupsQuery = _context.NhomChats
                .Include(n => n.NguoiTao)
                .Include(n => n.NguoiGoi)
                .Include(n => n.ThanhViens)
                    .ThenInclude(tv => tv.TaiKhoan)
                .AsQueryable();

            if (vaiTro != "Admin")
            {
                // Chỉ lấy nhóm mà user là thành viên
                groupsQuery = groupsQuery.Where(n => n.ThanhViens.Any(tv => tv.MaTaiKhoan == currentUserId.Value));
            }

            var groups = await groupsQuery.ToListAsync();
            var rawGroups = new List<dynamic>();

            foreach (var n in groups)
            {
                var lastMsg = await _context.TinNhans
                    .Where(m => m.MaNhom == n.MaNhom)
                    .OrderByDescending(m => m.ThoiGian)
                    .FirstOrDefaultAsync();

                var myRole = n.ThanhViens.FirstOrDefault(tv => tv.MaTaiKhoan == currentUserId.Value)?.VaiTroTrongNhom ?? "ThanhVien";
                bool canManage = vaiTro == "Admin" || vaiTro == "GiaoVien" || n.NguoiTaoId == currentUserId.Value || myRole == "TruongNhom" || myRole == "QuanTriVien";

                rawGroups.Add(new
                {
                    maNhom = n.MaNhom,
                    tenNhom = n.TenNhom,
                    anhNhom = !string.IsNullOrWhiteSpace(n.AnhNhom) ? n.AnhNhom : "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=150&auto=format&fit=crop&q=80",
                    memberCount = n.ThanhViens.Count,
                    nguoiTaoTen = n.NguoiTao?.HoTen ?? "Ban Quản Trị",
                    canManage = canManage,
                    cuocGoiDangDienRa = n.CuocGoiDangDienRa,
                    loaiCuocGoi = n.LoaiCuocGoi,
                    nguoiGoiId = n.NguoiGoiId,
                    nguoiGoiTen = n.NguoiGoi?.HoTen,
                    lastMessage = lastMsg != null ? (string.IsNullOrEmpty(lastMsg.NoiDung) ? "[Hình ảnh]" : lastMsg.NoiDung) : "Chưa có tin nhắn nào",
                    lastMessageTime = lastMsg != null ? lastMsg.ThoiGian.ToString("HH:mm") : "",
                    lastTime = lastMsg?.ThoiGian
                });
            }

            // Sắp xếp nhóm có tin nhắn mới nhất lên đầu
            var groupList = rawGroups
                .OrderByDescending(g => g.lastTime != null)
                .ThenByDescending(g => (DateTime?)g.lastTime)
                .ThenByDescending(g => (int)g.maNhom)
                .ToList();

            return Json(new { success = true, groups = groupList });
        }

        // POST: /Chat/CreateGroup (Chỉ Giáo viên, Quản lý và Admin mới được tạo nhóm)
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto? body)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để thực hiện!" });
            }

            // KIỂM TRA QUYỀN: Chỉ Giáo viên hoặc Quản lý / Admin
            if (vaiTro != "Admin" && vaiTro != "GiaoVien")
            {
                return Json(new { success = false, message = "Chỉ Giáo viên, Quản lý hoặc Admin mới có quyền tạo nhóm chat mới!" });
            }

            string groupName = (body?.TenNhom ?? "").Trim();
            string? groupAvatar = body?.AnhNhom?.Trim();
            List<int> memberList = body?.MemberIds ?? new List<int>();

            // Hỗ trợ fallback từ Query hoặc Form nếu không gửi JSON body
            if (string.IsNullOrWhiteSpace(groupName))
            {
                if (Request.Query.ContainsKey("tenNhom"))
                {
                    groupName = Request.Query["tenNhom"].ToString().Trim();
                    groupAvatar = Request.Query["anhNhom"].ToString().Trim();
                }
                else if (Request.HasFormContentType && Request.Form.ContainsKey("tenNhom"))
                {
                    groupName = Request.Form["tenNhom"].ToString().Trim();
                    groupAvatar = Request.Form["anhNhom"].ToString().Trim();
                    var fMemberIds = Request.Form["memberIds"].ToString();
                    if (!string.IsNullOrEmpty(fMemberIds))
                    {
                        memberList = fMemberIds.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s.Trim(), out int id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return Json(new { success = false, message = "Vui lòng nhập tên nhóm chat!" });
            }

            var creator = await _context.TaiKhoans.FindAsync(currentUserId.Value);

            var nhom = new NhomChat
            {
                TenNhom = groupName,
                AnhNhom = !string.IsNullOrWhiteSpace(groupAvatar) ? groupAvatar : "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=150&auto=format&fit=crop&q=80",
                NguoiTaoId = currentUserId.Value,
                NgayTao = DateTime.Now,
                CuocGoiDangDienRa = false
            };

            _context.NhomChats.Add(nhom);
            await _context.SaveChangesAsync();

            // Thêm người tạo làm Trưởng nhóm
            _context.ThanhVienNhoms.Add(new ThanhVienNhom
            {
                MaNhom = nhom.MaNhom,
                MaTaiKhoan = currentUserId.Value,
                VaiTroTrongNhom = "TruongNhom",
                NgayThamGia = DateTime.Now
            });

            // Thêm các thành viên được chọn
            if (memberList != null && memberList.Any())
            {
                foreach (var uid in memberList.Distinct())
                {
                    if (uid != currentUserId.Value && await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan == uid))
                    {
                        _context.ThanhVienNhoms.Add(new ThanhVienNhom
                        {
                            MaNhom = nhom.MaNhom,
                            MaTaiKhoan = uid,
                            VaiTroTrongNhom = "ThanhVien",
                            NgayThamGia = DateTime.Now
                        });
                    }
                }
            }

            // Gửi tin nhắn hệ thống thông báo tạo nhóm
            _context.TinNhans.Add(new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = nhom.MaNhom,
                LoaiTinNhan = "HeThong",
                NoiDung = $"{creator?.HoTen ?? "Quản trị viên"} đã tạo nhóm chat '{nhom.TenNhom}'.",
                ThoiGian = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, maNhom = nhom.MaNhom, tenNhom = nhom.TenNhom });
        }

        // GET: /Chat/GetGroupMembers?maNhom=...
        [HttpGet]
        public async Task<IActionResult> GetGroupMembers(int maNhom)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats
                .Include(n => n.ThanhViens)
                    .ThenInclude(tv => tv.TaiKhoan)
                        .ThenInclude(t => t!.GiaoVien)
                .FirstOrDefaultAsync(n => n.MaNhom == maNhom);

            if (nhom == null) return NotFound();

            bool canManage = vaiTro == "Admin" || vaiTro == "GiaoVien" || nhom.NguoiTaoId == currentUserId.Value;

            var members = nhom.ThanhViens.Select(tv => new
            {
                maThanhVien = tv.MaThanhVien,
                maTaiKhoan = tv.MaTaiKhoan,
                hoTen = tv.TaiKhoan?.HoTen ?? "Người dùng",
                email = tv.TaiKhoan?.Email,
                vaiTro = tv.TaiKhoan?.VaiTro ?? "HocVien",
                vaiTroTrongNhom = tv.VaiTroTrongNhom,
                anhDaiDien = tv.TaiKhoan?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                canKick = canManage && tv.MaTaiKhoan != currentUserId.Value && tv.VaiTroTrongNhom != "TruongNhom"
            }).ToList();

            return Json(new { success = true, members, canManage });
        }

        // GET: /Chat/GetCandidateMembers?maNhom=... (Danh sách user có thể thêm vào nhóm)
        [HttpGet]
        public async Task<IActionResult> GetCandidateMembers(int maNhom)
        {
            var existingMemberUserIds = await _context.ThanhVienNhoms
                .Where(tv => tv.MaNhom == maNhom)
                .Select(tv => tv.MaTaiKhoan)
                .ToListAsync();

            var candidates = await _context.TaiKhoans
                .Include(t => t.GiaoVien)
                .Include(t => t.HocVien)
                .Where(t => !existingMemberUserIds.Contains(t.MaTaiKhoan))
                .OrderBy(t => t.VaiTro)
                .ThenBy(t => t.HoTen)
                .Select(t => new
                {
                    maTaiKhoan = t.MaTaiKhoan,
                    hoTen = t.HoTen,
                    vaiTro = t.VaiTro,
                    chuyenMon = t.GiaoVien != null ? t.GiaoVien.ChuyenMon : (t.VaiTro == "Admin" ? "Quản trị viên" : "Học viên"),
                    anhDaiDien = t.GiaoVien != null ? t.GiaoVien.AnhDaiDien : "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80"
                })
                .ToListAsync();

            return Json(new { success = true, candidates });
        }

        // POST: /Chat/AddGroupMember (Giáo viên, Quản lý, Admin có quyền thêm thành viên)
        [HttpPost]
        public async Task<IActionResult> AddGroupMember(int maNhom, int maTaiKhoan)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats.FindAsync(maNhom);
            if (nhom == null) return NotFound();

            bool canManage = vaiTro == "Admin" || vaiTro == "GiaoVien" || nhom.NguoiTaoId == currentUserId.Value;
            if (!canManage)
            {
                return Json(new { success = false, message = "Bạn không có quyền thêm thành viên vào nhóm này!" });
            }

            var already = await _context.ThanhVienNhoms.AnyAsync(tv => tv.MaNhom == maNhom && tv.MaTaiKhoan == maTaiKhoan);
            if (already)
            {
                return Json(new { success = false, message = "Người dùng này đã có mặt trong nhóm!" });
            }

            var targetUser = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            var actor = await _context.TaiKhoans.FindAsync(currentUserId.Value);

            _context.ThanhVienNhoms.Add(new ThanhVienNhom
            {
                MaNhom = maNhom,
                MaTaiKhoan = maTaiKhoan,
                VaiTroTrongNhom = "ThanhVien",
                NgayThamGia = DateTime.Now
            });

            _context.TinNhans.Add(new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = maNhom,
                LoaiTinNhan = "HeThong",
                NoiDung = $"{actor?.HoTen ?? "Quản trị"} đã thêm {targetUser?.HoTen ?? "thành viên"} vào nhóm.",
                ThoiGian = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã thêm {targetUser?.HoTen} vào nhóm thành công!" });
        }

        // POST: /Chat/KickGroupMember (Giáo viên, Quản lý, Admin có quyền kick thành viên)
        [HttpPost]
        public async Task<IActionResult> KickGroupMember(int maNhom, int maTaiKhoan)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats.FindAsync(maNhom);
            if (nhom == null) return NotFound();

            bool canManage = vaiTro == "Admin" || vaiTro == "GiaoVien" || nhom.NguoiTaoId == currentUserId.Value;
            if (!canManage)
            {
                return Json(new { success = false, message = "Bạn không có quyền mời thành viên ra khỏi nhóm!" });
            }

            var member = await _context.ThanhVienNhoms
                .Include(m => m.TaiKhoan)
                .FirstOrDefaultAsync(tv => tv.MaNhom == maNhom && tv.MaTaiKhoan == maTaiKhoan);

            if (member == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thành viên này trong nhóm!" });
            }

            var targetName = member.TaiKhoan?.HoTen ?? "Thành viên";
            var actor = await _context.TaiKhoans.FindAsync(currentUserId.Value);

            _context.ThanhVienNhoms.Remove(member);

            _context.TinNhans.Add(new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = maNhom,
                LoaiTinNhan = "HeThong",
                NoiDung = $"{actor?.HoTen ?? "Quản trị"} đã mời {targetName} rời khỏi nhóm.",
                ThoiGian = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã kick {targetName} khỏi nhóm thành công!" });
        }

        // POST: /Chat/DeleteGroup (Giáo viên, Quản lý, Admin có quyền xóa nhóm chat)
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int maNhom)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats.FindAsync(maNhom);
            if (nhom == null) return NotFound();

            bool canManage = vaiTro == "Admin" || vaiTro == "GiaoVien" || nhom.NguoiTaoId == currentUserId.Value;
            if (!canManage)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa nhóm chat này!" });
            }

            _context.NhomChats.Remove(nhom);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xóa nhóm chat '{nhom.TenNhom}' thành công!" });
        }

        // =========================================================================
        // 4. TIN NHẮN NHÓM (GROUP MESSAGES)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetGroupMessages(int maNhom)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để xem tin nhắn nhóm!" });
            }

            // Kiểm tra user có trong nhóm không (hoặc là Admin)
            if (vaiTro != "Admin")
            {
                var isMember = await _context.ThanhVienNhoms.AnyAsync(tv => tv.MaNhom == maNhom && tv.MaTaiKhoan == currentUserId.Value);
                if (!isMember)
                {
                    return Json(new { success = false, message = "Bạn không phải là thành viên của nhóm này!" });
                }
            }

            var nhom = await _context.NhomChats
                .Include(n => n.NguoiGoi)
                .FirstOrDefaultAsync(n => n.MaNhom == maNhom);

            var messages = await _context.TinNhans
                .Include(m => m.NguoiGui)
                    .ThenInclude(t => t!.GiaoVien)
                .Where(m => m.MaNhom == maNhom)
                .OrderBy(m => m.ThoiGian)
                .Select(m => new
                {
                    maTinNhan = m.MaTinNhan,
                    nguoiGuiId = m.NguoiGuiId,
                    nguoiGuiTen = m.NguoiGui != null ? m.NguoiGui.HoTen : "Thành viên",
                    nguoiGuiAvatar = m.NguoiGui != null && m.NguoiGui.GiaoVien != null ? m.NguoiGui.GiaoVien.AnhDaiDien : "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = m.NoiDung,
                    hinhAnhUrl = m.HinhAnhUrl,
                    loaiTinNhan = m.LoaiTinNhan,
                    thoiGian = m.ThoiGian.ToString("HH:mm - dd/MM"),
                    isMine = m.NguoiGuiId == currentUserId.Value
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                messages,
                groupInfo = new
                {
                    maNhom = nhom?.MaNhom,
                    tenNhom = nhom?.TenNhom,
                    anhNhom = nhom?.AnhNhom,
                    cuocGoiDangDienRa = nhom?.CuocGoiDangDienRa ?? false,
                    loaiCuocGoi = nhom?.LoaiCuocGoi,
                    nguoiGoiId = nhom?.NguoiGoiId,
                    nguoiGoiTen = nhom?.NguoiGoi?.HoTen,
                    thoiGianBatDauGoi = nhom?.ThoiGianBatDauGoi?.ToString("o")
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendGroupMessage(int maNhom, string? noiDung, IFormFile? imageFile)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentUserId.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để gửi tin nhắn nhóm!" });
            }

            if (string.IsNullOrWhiteSpace(noiDung) && imageFile == null)
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung hoặc chọn ảnh!" });
            }

            string? uploadedImageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    return Json(new { success = false, message = "Định dạng ảnh không được hỗ trợ!" });
                }

                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "chat");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"chat_group_{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                uploadedImageUrl = $"/uploads/chat/{fileName}";
            }

            var sender = await _context.TaiKhoans.Include(t => t.GiaoVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);

            var message = new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = maNhom,
                NoiDung = noiDung?.Trim(),
                HinhAnhUrl = uploadedImageUrl,
                LoaiTinNhan = string.IsNullOrEmpty(uploadedImageUrl) ? "VanBan" : "HinhAnh",
                ThoiGian = DateTime.Now,
                DaXem = true
            };

            _context.TinNhans.Add(message);
            await _context.SaveChangesAsync();

            // Gửi thông báo SignalR thời gian thực đến tất cả thành viên trong nhóm
            try
            {
                var nhom = await _context.NhomChats.FindAsync(maNhom);
                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("ReceiveGroupMessage", new
                {
                    groupId = maNhom,
                    groupName = nhom?.TenNhom ?? "Nhóm chat",
                    senderUserId = currentUserId.Value,
                    senderName = sender?.HoTen ?? "Thành viên",
                    senderAvatar = sender?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = message.NoiDung,
                    hinhAnhUrl = message.HinhAnhUrl,
                    loaiTinNhan = message.LoaiTinNhan,
                    thoiGian = message.ThoiGian.ToString("HH:mm - dd/MM"),
                    maTinNhan = message.MaTinNhan
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR Error] SendGroupMessage: {ex.Message}");
            }

            return Json(new
            {
                success = true,
                message = new
                {
                    maTinNhan = message.MaTinNhan,
                    nguoiGuiId = message.NguoiGuiId,
                    nguoiGuiTen = sender?.HoTen ?? "Tôi",
                    nguoiGuiAvatar = sender?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = message.NoiDung,
                    hinhAnhUrl = message.HinhAnhUrl,
                    loaiTinNhan = message.LoaiTinNhan,
                    thoiGian = message.ThoiGian.ToString("HH:mm - dd/MM"),
                    isMine = true
                }
            });
        }

        // =========================================================================
        // 5. CHỨC NĂNG GỌI THOẠI & GỌI VIDEO NHÓM (VOICE & VIDEO CALL)
        // =========================================================================
        // POST: /Chat/StartGroupCall (Bắt đầu cuộc gọi thoại/video trong nhóm)
        [HttpPost]
        public async Task<IActionResult> StartGroupCall(int maNhom, string loaiCuocGoi)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats.FindAsync(maNhom);
            if (nhom == null) return NotFound();

            var caller = await _context.TaiKhoans
                .Include(t => t.GiaoVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);

            string callerAvatar = caller?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80";

            nhom.CuocGoiDangDienRa = true;
            nhom.LoaiCuocGoi = (loaiCuocGoi == "voice") ? "voice" : "video";
            nhom.NguoiGoiId = currentUserId.Value;
            nhom.ThoiGianBatDauGoi = DateTime.Now;

            string callTypeText = (loaiCuocGoi == "voice") ? "gọi thoại (Voice Call)" : "gọi video (Video Call)";

            // Thông báo trong đoạn chat nhóm
            var callMsg = new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = maNhom,
                LoaiTinNhan = "CuocGoi",
                NoiDung = $"{caller?.HoTen ?? "Một thành viên"} đã bắt đầu cuộc {callTypeText} nhóm.",
                ThoiGian = DateTime.Now,
                DaXem = true
            };
            _context.TinNhans.Add(callMsg);

            await _context.SaveChangesAsync();

            // Phát SignalR thông báo tới tất cả thành viên trong nhóm
            try
            {
                // 1. Gửi tin nhắn cuộc gọi vào đoạn chat realtime
                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("ReceiveGroupMessage", new
                {
                    groupId = maNhom,
                    groupName = nhom.TenNhom,
                    senderUserId = currentUserId.Value,
                    senderName = caller?.HoTen ?? "Thành viên",
                    senderAvatar = callerAvatar,
                    noiDung = callMsg.NoiDung,
                    hinhAnhUrl = (string?)null,
                    loaiTinNhan = callMsg.LoaiTinNhan,
                    thoiGian = callMsg.ThoiGian.ToString("HH:mm - dd/MM"),
                    maTinNhan = callMsg.MaTinNhan
                });

                // 2. Phát thông báo chuông cuộc gọi đến cho tất cả thành viên
                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("GroupIncomingCall", new
                {
                    groupId = maNhom,
                    groupName = nhom.TenNhom,
                    callerUserId = currentUserId.Value,
                    callType = nhom.LoaiCuocGoi,
                    callerName = caller?.HoTen ?? "Thành viên",
                    callerAvatar = callerAvatar
                });

                // 3. Cập nhật banner cuộc gọi đang diễn ra
                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("GroupCallStateChanged", new
                {
                    groupId = maNhom,
                    isCallActive = true,
                    callType = nhom.LoaiCuocGoi,
                    callerUserId = currentUserId.Value,
                    callerName = caller?.HoTen ?? "Thành viên"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR StartGroupCall Error]: {ex.Message}");
            }

            return Json(new
            {
                success = true,
                maNhom = nhom.MaNhom,
                tenNhom = nhom.TenNhom,
                loaiCuocGoi = nhom.LoaiCuocGoi,
                nguoiGoiTen = caller?.HoTen,
                callerAvatar = callerAvatar
            });
        }

        // POST: /Chat/EndGroupCall (Kết thúc cuộc gọi nhóm)
        [HttpPost]
        public async Task<IActionResult> EndGroupCall(int maNhom)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue) return Unauthorized();

            var nhom = await _context.NhomChats.FindAsync(maNhom);
            if (nhom == null) return NotFound();

            var caller = await _context.TaiKhoans.FindAsync(currentUserId.Value);

            nhom.CuocGoiDangDienRa = false;
            nhom.NguoiGoiId = null;
            nhom.LoaiCuocGoi = null;
            nhom.ThoiGianBatDauGoi = null;

            var endMsg = new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                MaNhom = maNhom,
                LoaiTinNhan = "HeThong",
                NoiDung = $"Cuộc gọi nhóm đã kết thúc.",
                ThoiGian = DateTime.Now,
                DaXem = true
            };
            _context.TinNhans.Add(endMsg);

            await _context.SaveChangesAsync();

            // Phát SignalR thông báo kết thúc cuộc gọi nhóm
            try
            {
                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("GroupCallEnded", new
                {
                    groupId = maNhom,
                    endedByUserId = currentUserId.Value
                });

                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("ReceiveGroupMessage", new
                {
                    groupId = maNhom,
                    groupName = nhom.TenNhom,
                    senderUserId = currentUserId.Value,
                    senderName = caller?.HoTen ?? "Hệ thống",
                    senderAvatar = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = endMsg.NoiDung,
                    hinhAnhUrl = (string?)null,
                    loaiTinNhan = endMsg.LoaiTinNhan,
                    thoiGian = endMsg.ThoiGian.ToString("HH:mm - dd/MM"),
                    maTinNhan = endMsg.MaTinNhan
                });

                await _hubContext.Clients.Group($"group_{maNhom}").SendAsync("GroupCallStateChanged", new
                {
                    groupId = maNhom,
                    isCallActive = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR EndGroupCall Error]: {ex.Message}");
            }

            return Json(new { success = true });
        }

        // =========================================================================
        // 5.1 CUỘC GỌI 1-1 (DIRECT VOICE & VIDEO CALL LOGGING)
        // =========================================================================

        // POST: /Chat/StartDirectCall
        [HttpPost]
        public async Task<IActionResult> StartDirectCall(int targetUserId, string loaiCuocGoi)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue) return Unauthorized();

            var caller = await _context.TaiKhoans.Include(t => t.GiaoVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);
            var target = await _context.TaiKhoans.Include(t => t.GiaoVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == targetUserId);
            if (caller == null || target == null) return NotFound();

            return Json(new
            {
                success = true,
                callerUserId = currentUserId.Value,
                callerName = caller.HoTen,
                callerAvatar = caller.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                targetUserId,
                targetName = target.HoTen,
                targetAvatar = target.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                loaiCuocGoi = loaiCuocGoi == "voice" ? "voice" : "video"
            });
        }

        // POST: /Chat/EndDirectCall
        [HttpPost]
        public async Task<IActionResult> EndDirectCall(int targetUserId, string loaiCuocGoi, int durationSeconds = 0, string status = "ended")
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue) return Unauthorized();

            string icon = (loaiCuocGoi == "voice") ? "📞" : "📹";
            string typeName = (loaiCuocGoi == "voice") ? "gọi thoại" : "gọi video";
            string msgContent;

            if (status == "ended" && durationSeconds > 0)
            {
                int mins = durationSeconds / 60;
                int secs = durationSeconds % 60;
                string timeStr = $"{mins:D2}:{secs:D2}";
                msgContent = $"{icon} Cuộc {typeName} kết thúc · {timeStr}";
            }
            else if (status == "rejected")
            {
                msgContent = $"{icon} Cuộc {typeName} bị từ chối";
            }
            else if (status == "missed" || durationSeconds == 0)
            {
                msgContent = $"{icon} Cuộc {typeName} nhỡ";
            }
            else
            {
                msgContent = $"{icon} Cuộc {typeName} đã kết thúc";
            }

            var message = new TinNhan
            {
                NguoiGuiId = currentUserId.Value,
                NguoiNhanId = targetUserId,
                LoaiTinNhan = "CuocGoi",
                NoiDung = msgContent,
                ThoiGian = DateTime.Now,
                DaXem = false
            };

            _context.TinNhans.Add(message);
            await _context.SaveChangesAsync();

            var sender = await _context.TaiKhoans.Include(t => t.GiaoVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == currentUserId.Value);

            return Json(new
            {
                success = true,
                message = new
                {
                    maTinNhan = message.MaTinNhan,
                    nguoiGuiId = message.NguoiGuiId,
                    nguoiGuiTen = sender?.HoTen ?? "Tôi",
                    nguoiGuiAvatar = sender?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80",
                    noiDung = message.NoiDung,
                    hinhAnhUrl = (string?)null,
                    loaiTinNhan = "CuocGoi",
                    thoiGian = message.ThoiGian.ToString("HH:mm - dd/MM"),
                    isMine = true
                }
            });
        }


        // GET: /Chat/CheckIncomingCalls (Polling thông báo cuộc gọi đến các thành viên có mặt trong nhóm)
        [HttpGet]
        public async Task<IActionResult> CheckIncomingCalls()
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { hasIncomingCall = false });
            }

            // Tìm nhóm nào user là thành viên mà đang có cuộc gọi diễn ra (người gọi khác user)
            var activeCall = await _context.NhomChats
                .Include(n => n.NguoiGoi)
                    .ThenInclude(g => g!.GiaoVien)
                .Where(n => n.CuocGoiDangDienRa && 
                            n.NguoiGoiId != currentUserId.Value &&
                            n.ThanhViens.Any(tv => tv.MaTaiKhoan == currentUserId.Value))
                .OrderByDescending(n => n.ThoiGianBatDauGoi)
                .FirstOrDefaultAsync();

            if (activeCall != null)
            {
                return Json(new
                {
                    hasIncomingCall = true,
                    maNhom = activeCall.MaNhom,
                    tenNhom = activeCall.TenNhom,
                    loaiCuocGoi = activeCall.LoaiCuocGoi,
                    nguoiGoiTen = activeCall.NguoiGoi?.HoTen ?? "Giảng viên",
                    nguoiGoiAvatar = activeCall.NguoiGoi?.GiaoVien?.AnhDaiDien ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80"
                });
            }

            return Json(new { hasIncomingCall = false });
        }

        // GET: /Chat/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!currentUserId.HasValue)
            {
                return Json(new { count = 0 });
            }

            // Chỉ đếm tin nhắn 1-1 chưa đọc (không đếm nhóm)
            var count = await _context.TinNhans
                .CountAsync(m => m.NguoiNhanId == currentUserId.Value && !m.DaXem && m.MaNhom == null);

            return Json(new { count });
        }

        // GET: /Chat/GetAllUsers - Lấy tất cả người dùng có thể nhắn tin 1-1
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");

            var users = await _context.TaiKhoans
                .Include(t => t.GiaoVien)
                .Where(t => t.TrangThai == "HoatDong" && t.MaTaiKhoan != (currentUserId ?? -1))
                .OrderBy(t => t.VaiTro)
                .ThenBy(t => t.HoTen)
                .ToListAsync();

            var rawUsers = new List<dynamic>();

            foreach (var u in users)
            {
                TinNhan? lastMsg = null;
                int unreadCount = 0;

                if (currentUserId.HasValue)
                {
                    lastMsg = await _context.TinNhans
                        .Where(m => m.MaNhom == null &&
                                    ((m.NguoiGuiId == currentUserId && m.NguoiNhanId == u.MaTaiKhoan) ||
                                     (m.NguoiGuiId == u.MaTaiKhoan && m.NguoiNhanId == currentUserId)))
                        .OrderByDescending(m => m.ThoiGian)
                        .FirstOrDefaultAsync();

                    unreadCount = await _context.TinNhans
                        .CountAsync(m => m.MaNhom == null && m.NguoiGuiId == u.MaTaiKhoan && m.NguoiNhanId == currentUserId && !m.DaXem);
                }

                string roleLabel = u.VaiTro switch
                {
                    "Admin" => "Quản lý",
                    "GiaoVien" => u.GiaoVien?.ChuyenMon ?? "Giáo viên",
                    _ => "Học viên"
                };

                string avatar = u.GiaoVien?.AnhDaiDien
                    ?? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80";

                rawUsers.Add(new
                {
                    maTaiKhoan = u.MaTaiKhoan,
                    hoTen = u.HoTen,
                    vaiTro = u.VaiTro,
                    chuyenMon = roleLabel,
                    anhDaiDien = avatar,
                    isOnline = true,
                    lastMessage = lastMsg != null ? (string.IsNullOrEmpty(lastMsg.NoiDung) ? "[Đã gửi một ảnh]" : lastMsg.NoiDung) : "Bấm vào để bắt đầu trò chuyện...",
                    lastMessageTime = lastMsg != null ? lastMsg.ThoiGian.ToString("HH:mm") : "",
                    unreadCount = unreadCount,
                    lastTime = lastMsg?.ThoiGian
                });
            }

            // Sắp xếp người có tin nhắn gần nhất lên đầu danh sách
            var userList = rawUsers
                .OrderByDescending(u => u.lastTime != null)
                .ThenByDescending(u => (DateTime?)u.lastTime)
                .ThenBy(u => (string)u.hoTen)
                .ToList();

            return Json(new { success = true, users = userList });
        }

        // =========================================================================
        // ICE SERVERS - Cung cấp danh sách STUN/TURN servers cho WebRTC
        // =========================================================================
        [HttpGet]
        public IActionResult GetIceServers()
        {
            // TURN servers công khai miễn phí để hỗ trợ NAT traversal
            // Khi gọi qua LAN, ngrok, hoặc 3G/4G cần có TURN server
            var iceServers = new object[]
            {
                new { urls = new[] { "stun:stun.l.google.com:19302" } },
                new { urls = new[] { "stun:stun1.l.google.com:19302" } },
                new { urls = new[] { "stun:stun2.l.google.com:19302" } },
                new { urls = new[] { "stun:stun.cloudflare.com:3478" } },
                new {
                    urls = new[] { "turn:openrelay.metered.ca:80", "turn:openrelay.metered.ca:443", "turns:openrelay.metered.ca:443" },
                    username = "openrelayproject",
                    credential = "openrelayproject"
                }
            };
            return Json(iceServers);
        }
    }

    public class CreateGroupDto
    {
        public string? TenNhom { get; set; }
        public string? AnhNhom { get; set; }
        public List<int>? MemberIds { get; set; }
    }
}
