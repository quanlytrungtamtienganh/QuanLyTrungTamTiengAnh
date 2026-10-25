using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Data;
using TrungTamNgoaiNgu.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Kestrel lắng nghe trên tất cả card mạng (0.0.0.0) để điện thoại trong mạng Wi-Fi truy cập được
builder.WebHost.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");

// 1. Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<TrungTamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions =>
    {
        sqlServerOptions.CommandTimeout(120);
    }));

// 2. Cấu hình HttpContextAccessor & Session cho xác thực và phân quyền
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".TrungTamNgoaiNgu.Session";
});

// 3. Đăng ký Controllers và Views
builder.Services.AddControllersWithViews();

// 4. Đăng ký Email Service
builder.Services.AddSingleton<IEmailService, EmailService>();

// 5. Đăng ký SignalR cho chức năng Gọi thoại & Gọi Video Call thời gian thực
builder.Services.AddSignalR();

var app = builder.Build();

// Tự động khởi tạo và nạp dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TrungTamDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi khởi tạo cơ sở dữ liệu và nạp dữ liệu mẫu.");
    }
}

// Cấu hình HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng Session trước Authorization
app.UseSession();
app.UseAuthorization();

// Đăng ký SignalR Hub endpoint cho Video & Voice Call
app.MapHub<TrungTamNgoaiNgu.Hubs.CallHub>("/callHub");

// Route mặc định: Landing page (trang quảng cáo) → người dùng chưa đăng nhập sẽ thấy trước
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");

app.Run();

