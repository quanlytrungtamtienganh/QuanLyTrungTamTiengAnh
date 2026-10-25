using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TrungTamNgoaiNgu.Services
{
    public interface IEmailService
    {
        Task SendAccountCreatedEmailAsync(string toEmail, string toName, string username, string password, string role);
        Task<bool> SendPasswordResetOtpEmailAsync(string toEmail, string toName, string otpCode, int expiryMinutes = 5);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAccountCreatedEmailAsync(string toEmail, string toName, string username, string password, string role)
        {
            try
            {
                var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var smtpUser = _config["Email:Username"] ?? "";
                var smtpPass = _config["Email:Password"] ?? "";
                var fromName = _config["Email:FromName"] ?? "LanguageBook";

                if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                {
                    _logger.LogWarning("Email chưa được cấu hình. Bỏ qua gửi mail tới {Email}", toEmail);
                    return;
                }

                string roleVi = role == "GiaoVien" ? "Giáo viên" : (role == "Admin" ? "Quản lý" : "Học viên");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, smtpUser));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = $"[LanguageBook] Tài khoản của bạn đã được tạo thành công!";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html lang='vi'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='font-family: Segoe UI, Arial, sans-serif; background: #f7f8fa; margin:0; padding:0;'>
  <div style='max-width: 560px; margin: 40px auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08);'>
    <!-- Header -->
    <div style='background: linear-gradient(135deg, #0866ff, #0050d0); padding: 32px 40px; text-align: center;'>
      <h1 style='color: #fff; font-size: 24px; margin: 0; font-weight: 800;'>📚 LanguageBook</h1>
      <p style='color: rgba(255,255,255,0.85); margin: 8px 0 0; font-size: 14px;'>Trung tâm Ngoại ngữ hàng đầu</p>
    </div>
    <!-- Body -->
    <div style='padding: 36px 40px;'>
      <h2 style='color: #050505; font-size: 20px; margin: 0 0 16px;'>Chào mừng {toName}! 🎉</h2>
      <p style='color: #444; font-size: 15px; line-height: 1.6; margin: 0 0 24px;'>
        Tài khoản <strong>{roleVi}</strong> của bạn tại hệ thống <strong>LanguageBook</strong> đã được tạo thành công. 
        Dưới đây là thông tin đăng nhập của bạn:
      </p>
      <!-- Credentials Box -->
      <div style='background: #f0f7ff; border: 1px solid #c3d9ff; border-radius: 12px; padding: 24px; margin-bottom: 24px;'>
        <div style='margin-bottom: 14px;'>
          <span style='color: #65676b; font-size: 12px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>Tên đăng nhập</span>
          <div style='background: #fff; border: 1px solid #e4e6eb; border-radius: 8px; padding: 10px 16px; margin-top: 6px; font-family: monospace; font-size: 16px; font-weight: 700; color: #0866ff; letter-spacing: 1px;'>{username}</div>
        </div>
        <div>
          <span style='color: #65676b; font-size: 12px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>Mật khẩu</span>
          <div style='background: #fff; border: 1px solid #e4e6eb; border-radius: 8px; padding: 10px 16px; margin-top: 6px; font-family: monospace; font-size: 16px; font-weight: 700; color: #e03131; letter-spacing: 2px;'>{password}</div>
        </div>
      </div>
      <a href='http://localhost:5000/TaiKhoan/Login' 
         style='display: block; background: #0866ff; color: #ffffff; text-decoration: none; text-align: center; padding: 14px 32px; border-radius: 10px; font-size: 16px; font-weight: 700; margin-bottom: 24px;'>
        Đăng nhập ngay →
      </a>
      <div style='background: #fff8e1; border-left: 4px solid #f59f00; border-radius: 0 8px 8px 0; padding: 12px 16px; font-size: 13px; color: #666;'>
        <strong>⚠️ Lưu ý bảo mật:</strong> Vui lòng đổi mật khẩu sau khi đăng nhập lần đầu. Không chia sẻ thông tin đăng nhập với người khác.
      </div>
    </div>
    <!-- Footer -->
    <div style='background: #f7f8fa; padding: 20px 40px; text-align: center; border-top: 1px solid #e4e6eb;'>
      <p style='color: #888; font-size: 12px; margin: 0;'>© 2024 LanguageBook · 123 Nguyễn Văn Linh, Q.7, TP.HCM · admin@languagebook.edu.vn</p>
    </div>
  </div>
</body>
</html>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Đã gửi email tài khoản tới {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email tới {Email}", toEmail);
                // Không throw - lỗi email không nên làm hỏng luồng tạo tài khoản
            }
        }

        public async Task<bool> SendPasswordResetOtpEmailAsync(string toEmail, string toName, string otpCode, int expiryMinutes = 5)
        {
            try
            {
                var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var smtpUser = _config["Email:Username"] ?? "";
                var smtpPass = _config["Email:Password"] ?? "";
                var fromName = _config["Email:FromName"] ?? "Trung Tâm Ngoại Ngữ";

                if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                {
                    _logger.LogWarning("Email chưa được cấu hình. Mã OTP cho {Email} là: {Otp}", toEmail, otpCode);
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, smtpUser));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = $"[{fromName}] Mã OTP xác nhận đổi mật khẩu: {otpCode}";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html lang='vi'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; background: #f0f2f5; margin:0; padding:20px 0;'>
  <div style='max-width: 560px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 16px rgba(0,0,0,0.08); border: 1px solid #e4e6eb;'>
    <div style='background: linear-gradient(135deg, #1e3a8a, #2563eb); padding: 32px 30px; text-align: center; color: #ffffff;'>
      <div style='font-size: 32px; margin-bottom: 8px;'>🔐</div>
      <h1 style='font-size: 22px; font-weight: 800; margin: 0;'>Trung Tâm Ngoại Ngữ</h1>
      <p style='margin: 6px 0 0; font-size: 14px; opacity: 0.9;'>Yêu cầu đổi mật khẩu tài khoản</p>
    </div>
    <div style='padding: 32px 30px;'>
      <p style='font-size: 15px; color: #1c1e21; margin: 0 0 16px; line-height: 1.6;'>
        Xin chào <strong>{toName}</strong>,
      </p>
      <p style='font-size: 15px; color: #4b5563; margin: 0 0 20px; line-height: 1.6;'>
        Hệ thống nhận được yêu cầu đổi mật khẩu cho tài khoản liên kết với địa chỉ Gmail này. Dưới đây là mã xác thực OTP của bạn:
      </p>
      <div style='background: #f8fafc; border: 2px dashed #2563eb; border-radius: 10px; padding: 20px; text-align: center; margin: 24px 0;'>
        <div style='font-size: 12px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>MÃ XÁC THỰC OTP</div>
        <div style='font-size: 38px; font-weight: 900; color: #1e3a8a; letter-spacing: 8px; font-family: monospace;'>{otpCode}</div>
      </div>
      <div style='background: #fffbeb; border-left: 4px solid #f59e0b; padding: 14px 16px; border-radius: 0 8px 8px 0; margin-bottom: 24px;'>
        <p style='margin: 0; font-size: 13.5px; color: #92400e; line-height: 1.5;'>
          ⏱️ <strong>Thời hạn hiệu lực:</strong> Mã OTP có hiệu lực trong vòng <strong>{expiryMinutes} phút</strong> kể từ thời điểm gửi mã. Sau thời gian này mã sẽ tự động vô hiệu hóa.
        </p>
        <p style='margin: 6px 0 0; font-size: 13px; color: #b45309;'>
          🔒 <strong>Bảo mật:</strong> Mã OTP này chỉ có tác dụng riêng cho tài khoản của bạn và giới hạn tối đa 5 lần đổi mật khẩu mỗi ngày. Tuyệt đối không chia sẻ mã này cho bất kỳ ai!
        </p>
      </div>
      <p style='font-size: 13px; color: #6b7280; line-height: 1.5; margin: 0;'>
        Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email hoặc thông báo ngay cho ban quản trị trung tâm để bảo đảm an toàn cho tài khoản.
      </p>
    </div>
    <div style='background: #f9fafb; padding: 18px 30px; text-align: center; border-top: 1px solid #e5e7eb; font-size: 12px; color: #9ca3af;'>
      Hệ Thống Quản Lý Trung Tâm Ngoại Ngữ &bull; Hỗ trợ: admin@educenter.edu.vn
    </div>
  </div>
</body>
</html>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Đã gửi email OTP đổi mật khẩu thành công tới {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email OTP tới {Email}", toEmail);
                return false;
            }
        }
    }
}
