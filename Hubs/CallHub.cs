using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;

namespace TrungTamNgoaiNgu.Hubs
{
    public class CallHub : Hub
    {
        private readonly TrungTamDbContext _context;

        public CallHub(TrungTamDbContext context)
        {
            _context = context;
        }

        // Khi user mở trang, đăng ký kết nối vào nhóm định danh cá nhân và các nhóm chat đã tham gia
        public async Task RegisterUser(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            // Tự động thêm kết nối vào tất cả nhóm chat mà user là thành viên
            try
            {
                var userGroupIds = await _context.ThanhVienNhoms
                    .Where(tv => tv.MaTaiKhoan == userId)
                    .Select(tv => tv.MaNhom)
                    .ToListAsync();

                foreach (var gid in userGroupIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{gid}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CallHub] Error registering groups for user {userId}: {ex.Message}");
            }
        }

        // Đăng ký vào một nhóm chat cụ thể
        public async Task JoinGroupChat(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
        }

        // =========================================================================
        // 1. CUỘC GỌI 1-1 (DIRECT VOICE & VIDEO CALL)
        // =========================================================================
        
        // Người A gọi cho Người B
        public async Task CallUser(int targetUserId, int callerUserId, string callType, string callerName, string callerAvatar)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("IncomingCall", new
            {
                callerUserId,
                callType,
                callerName,
                callerAvatar
            });
        }

        // Người B chấp nhận cuộc gọi của Người A
        public async Task AcceptCall(int callerUserId, int targetUserId, string callType)
        {
            await Clients.Group($"user_{callerUserId}").SendAsync("CallAccepted", new
            {
                acceptedByUserId = targetUserId,
                callType
            });
        }

        // Người B từ chối cuộc gọi của Người A
        public async Task RejectCall(int callerUserId, int targetUserId, string reason)
        {
            await Clients.Group($"user_{callerUserId}").SendAsync("CallRejected", new
            {
                rejectedByUserId = targetUserId,
                reason = string.IsNullOrEmpty(reason) ? "Đối phương đã từ chối cuộc gọi." : reason
            });
        }

        // Một trong 2 người gác máy / kết thúc cuộc gọi
        public async Task EndCall(int targetUserId, int senderUserId)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("CallEnded", new
            {
                endedByUserId = senderUserId
            });
        }

        // Chuyển tiếp tín hiệu WebRTC P2P (Offer, Answer, ICE Candidates)
        public async Task SendSignal(int targetUserId, int senderUserId, object signalData)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceiveSignal", new
            {
                senderUserId,
                signalData
            });
        }

        // =========================================================================
        // 2. CUỘC GỌI NHÓM (GROUP VOICE & VIDEO CALL)
        // =========================================================================
        
        public async Task StartGroupCall(int groupId, int callerUserId, string callType, string groupName, string callerName, string callerAvatar)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
            await Clients.Group($"group_{groupId}").SendAsync("GroupIncomingCall", new
            {
                groupId,
                groupName,
                callerUserId,
                callType,
                callerName,
                callerAvatar
            });
        }

        // Thành viên tham gia vào cuộc gọi nhóm đang diễn ra
        public async Task JoinGroupCall(int groupId, int userId, string userName, string userAvatar)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
            await Clients.OthersInGroup($"group_{groupId}").SendAsync("UserJoinedGroupCall", new
            {
                groupId,
                userId,
                userName,
                userAvatar
            });
        }

        // Thành viên rời khỏi cuộc gọi nhóm
        public async Task LeaveGroupCall(int groupId, int userId)
        {
            await Clients.OthersInGroup($"group_{groupId}").SendAsync("UserLeftGroupCall", new
            {
                groupId,
                userId
            });
        }

        public async Task EndGroupCall(int groupId, int senderUserId)
        {
            await Clients.Group($"group_{groupId}").SendAsync("GroupCallEnded", new
            {
                groupId,
                endedByUserId = senderUserId
            });
        }

        public async Task SendGroupSignal(int groupId, int senderUserId, object signalData)
        {
            await Clients.OthersInGroup($"group_{groupId}").SendAsync("ReceiveGroupSignal", new
            {
                groupId,
                senderUserId,
                signalData
            });
        }

        // Gửi tín hiệu WebRTC P2P trong nhóm tới một thành viên cụ thể (Offer/Answer/Candidate)
        public async Task SendGroupSignalToPeer(int groupId, int senderUserId, int targetUserId, object signalData)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceiveGroupSignal", new
            {
                groupId,
                senderUserId,
                targetUserId,
                signalData
            });
        }

        // =========================================================================
        // 3. THÔNG BÁO TIN NHẮN REALTIME (DIRECT & GROUP MESSAGES)
        // =========================================================================

        public async Task SendDirectMessage(int targetUserId, int senderUserId, string senderName, string senderAvatar, string noiDung, string thoiGian, int maTinNhan, string loaiTinNhan, string? hinhAnhUrl)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceiveDirectMessage", new
            {
                targetUserId,
                senderUserId,
                senderName,
                senderAvatar,
                noiDung,
                thoiGian,
                maTinNhan,
                loaiTinNhan,
                hinhAnhUrl
            });
        }

        public async Task SendGroupMessage(int groupId, int senderUserId, string senderName, string senderAvatar, string groupName, string noiDung, string thoiGian, int maTinNhan, string loaiTinNhan, string? hinhAnhUrl)
        {
            await Clients.OthersInGroup($"group_{groupId}").SendAsync("ReceiveGroupMessage", new
            {
                groupId,
                groupName,
                senderUserId,
                senderName,
                senderAvatar,
                noiDung,
                thoiGian,
                maTinNhan,
                loaiTinNhan,
                hinhAnhUrl
            });
        }
    }
}
