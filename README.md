# 🌐 HỆ THỐNG QUẢN LÝ & HỌC TẬP TRỰC TUYẾN TRUNG TÂM NGOẠI NGỮ (EDUCENTER)

> **Dự án Đồ Án Chuyên Ngành / Khóa Luận**: Xây dựng nền tảng quản trị trung tâm ngoại ngữ, đào tạo và luyện thi trực tuyến đa ngôn ngữ (Tiếng Anh, Tiếng Hàn, Tiếng Trung, Tiếng Nhật).  
> **Framework & Nền tảng**: ASP.NET Core MVC .NET 10, Entity Framework Core 10, Microsoft SQL Server, SignalR & WebRTC.

---

## 📑 MỤC LỤC
1. [Giới Thiệu Dự Án](#-giới-thiệu-dự-án)
2. [Công Nghệ Sử Dụng](#-công-nghệ-sử-dụng)
3. [Phân Chia Module Theo 4 Thành Viên](#-phân-chia-module-theo-04-thành-viên)
4. [Các Module Mở Rộng Nâng Cao](#-các-module-mở-rộng-nâng-cao)
5. [Danh Sách Tệp Tin Trong Dự Án](#-danh-sách-tệp-tin-trong-dự-án)
6. [Cơ Sở Dữ Liệu & Khôi Phục](#-cơ-sở-dữ-liệu--khôi-phục)
7. [Hướng Dẫn Cài Đặt & Chạy Ứng Dụng](#-hướng-dẫn-cài-đặt--chạy-ứng-dụng)
8. [Tài Khoản Thử Nghiệm](#-tài-khoản-thử-nghiệm-demo-accounts)
9. [Tài Liệu Kèm Theo](#-tài-liệu-kèm-theo)

---

## 🎯 GIỚI THIỆU DỰ ÁN

**Hệ Thống Quản Lý Trung Tâm Ngoại Ngữ EduCenter** là giải pháp phần mềm toàn diện giải quyết bài toán quản trị vận hành trung tâm và nâng cao trải nghiệm học tập của học viên:
- **Dành cho Ban Quản Trị (Admin)**: Dashboard chỉ số thời gian thực, thống kê doanh thu học phí, quản lý tài khoản người dùng, phân quyền vai trò, quản lý khóa học, lớp học, phân công giáo viên, duyệt đơn đăng ký học.
- **Dành cho Giảng Viên (Teacher)**: Quản lý danh sách lớp phụ trách, theo dõi học viên, tương tác qua kênh chat lớp học và phòng hội đàm video call trực tuyến.
- **Dành cho Học Viên (Student)**: Xem danh mục khóa học, đăng ký lớp học với quy trình kiểm tra điều kiện tự động, theo dõi lịch sử đăng ký, tham gia cộng đồng ngoại ngữ, luyện thi trắc nghiệm chứng chỉ quốc tế (IELTS, TOEIC, TOPIK, HSK, JLPT).

---

## 🛠 CÔNG NGHỆ SỬ DỤNG

| Lĩnh Vực | Công Nghệ / Thư Viện | Phiên Bản | Ghi Chú |
|:---|:---|:---:|:---|
| **Nền tảng chính** | ASP.NET Core MVC | **.NET 10.0** | Cấu trúc MVC chuẩn công nghiệp, hiệu năng cao |
| **Ngôn ngữ lập trình** | C# | **14.0** | Hỗ trợ cú pháp hiện đại, Pattern Matching, Record, Nullable Ref |
| **ORM & Truy cập dữ liệu** | Entity Framework Core | **10.0** | Code-First & Database Script, LINQ to Entities |
| **Hệ quản trị CSDL** | Microsoft SQL Server | **LocalDB / 2019+** | 20 bảng thực thể, quan hệ ràng buộc PK/FK toàn vẹn |
| **Giao tiếp thời gian thực** | ASP.NET Core SignalR | **Built-in** | WebSockets cho Chatbox, thông báo và kênh đàm thoại |
| **Truyền thông đa phương tiện** | WebRTC (Peer-to-Peer) | **HTML5 WebRTC** | Đàm thoại Video Call độ trễ thấp trực tiếp trên trình duyệt |
| **Giao diện người dùng** | Razor Views + Bootstrap 5 | **5.3+** | Responsive toàn diện trên PC, Tablet và Mobile |
| **Icon & Đồ họa** | Font Awesome 6 + Boxicons | **6.5+** | Bộ icon trực quan sinh động |
| **Biểu đồ thống kê** | Chart.js | **4.4+** | Vẽ biểu đồ doanh thu dạng cột và tròn tại Dashboard |
| **Dịch vụ Email** | MailKit & MimeKit | **4.8+** | Gửi email thông báo tự động qua giao thức SMTP Google |

---

## 👥 PHÂN CHIA MODULE THEO 04 THÀNH VIÊN

Dự án được phân chia thành **04 Module chính** tương ứng với 04 sinh viên phụ trách theo đúng đề cương đồ án:

### 4.1. VŨ TRƯỜNG AN – Module 1: Tài Khoản – Đăng Nhập & Phân Quyền – Quản Lý Khóa Học
- **Nghiệp vụ chi tiết**:
  - Đăng ký tài khoản, đăng nhập hệ thống, xác thực phiên làm việc (`Session`).
  - **Đổi mật khẩu / Quên mật khẩu qua mã OTP Gmail**:
    + Người dùng nhập Gmail tài khoản cần đổi mật khẩu $ightarrow$ hệ thống so sánh với CSDL.
    + Nếu Gmail không tồn tại: báo lỗi rõ ràng `"Địa chỉ Gmail không tồn tại trong hệ thống! Vui lòng kiểm tra lại."`.
    + Nếu đúng: sinh mã OTP 6 chữ số ngẫu nhiên gửi về hòm thư Gmail qua dịch vụ SMTP.
    + Mã OTP chỉ có tác dụng cho đúng tài khoản đang yêu cầu đổi.
    + Hạn sử dụng mã OTP chính xác **5 phút** tính từ thời gian gửi; quá 5 phút mã tự động hết hạn và không thể sử dụng.
    + Giới hạn số lần đổi mật khẩu chỉ **5 lần 1 ngày** (quản lý qua bảng `LichSuDoiMatKhau`).
    + Màn hình xác nhận có bộ đếm ngược thời gian thực 5 phút (JavaScript countdown) và nút gửi lại mã.
    + Xác nhận đúng mã OTP và mật khẩu mới hợp lệ ($\ge 6$ ký tự) sẽ cập nhật ngay mật khẩu trong hệ thống.
  - Phân quyền người dùng dựa trên 3 vai trò: `Admin`, `GiaoVien`, `HocVien`.
  - Quản lý danh sách tài khoản: hiển thị STT, Mã tài khoản, Tên đăng nhập, Họ tên, Email, Vai trò, Trạng thái; Chỉnh sửa thông tin và Xóa tài khoản.
  - Quản lý danh mục khóa học ngoại ngữ: Thêm, Sửa, Xóa, Xem chi tiết khóa học, quản lý học phí và số buổi.
- **Tên tệp tin thực hiện**:
  - Tệp Controller: `TaiKhoanController.cs`, `KhoaHocController.cs`
  - Tệp Model: `TaiKhoan.cs`, `KhoaHoc.cs`, `OtpDoiMatKhau.cs`, `LichSuDoiMatKhau.cs`
  - Tệp ViewModel: `DoiMatKhauViewModel.cs`
  - Tệp Dịch vụ: `EmailService.cs`, `IEmailService.cs`
  - Tệp Giao diện: `Login.cshtml`, `DangKy.cshtml`, `DoiMatKhau.cshtml`, `XacNhanDoiMatKhau.cshtml`, `Profile.cshtml`, `Index.cshtml`, `Edit.cshtml`, `Create.cshtml`, `Details.cshtml`, `Delete.cshtml`
  - Bảng CSDL: `TaiKhoan`, `KhoaHoc`, `OtpDoiMatKhau`, `LichSuDoiMatKhau`

### 4.2. TRẦN TRỌNG TRÁNG – Module 2: Quản Lý Lớp Học – Tìm Kiếm – Lọc – Sắp Xếp – Phân Trang
- **Nghiệp vụ chi tiết**:
  - Quản lý lớp học: Thêm lớp mới, chỉnh sửa thông tin lớp, **Xóa lớp học (Mục 4.13)**.
  - **Bộ lọc tìm kiếm giáo viên (Mục 4.8)**: Tìm kiếm lớp học theo giáo viên phụ trách, trạng thái lớp (Đang mở, Đang học, Kết thúc).
  - **Sắp xếp dữ liệu (Mục 4.9)**: Sắp xếp theo Tên lớp tăng/giảm dần, Học phí tăng/giảm dần, Ngày tạo mới nhất/cũ nhất.
  - **Tìm kiếm & Phân trang**: Thanh tìm kiếm từ khóa đa năng kết hợp phân trang linh hoạt.
  - Quản lý hồ sơ giảng viên: Chuyên môn, học vị, phân công giảng dạy.
- **Tên tệp tin thực hiện**:
  - Tệp Controller: `LopHocController.cs`, `GiaoVienController.cs`
  - Tệp Model: `LopHoc.cs`, `GiaoVien.cs`
  - Tệp Giao diện: `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`, `Details.cshtml`
  - Bảng CSDL: `LopHoc`, `GiaoVien`

### 4.3. PHẠM THANH TÙNG – Module 3: Quản Lý Học Viên – Đăng Ký Lớp Học – Lịch Sử Đăng Ký
- **Nghiệp vụ chi tiết**:
  - **Quản lý học viên (Mục 4.14)**: Hiển thị danh sách, thông tin Họ tên, Email, SĐT, **Địa chỉ**, Thêm học viên mới, Chỉnh sửa thông tin học viên, Xóa học viên.
  - **Quy trình đăng ký khóa học (Mục 4.16)**:
    - Luồng thao tác: Chọn lớp $\rightarrow$ Xem thông tin lớp $\rightarrow$ Nhấn "Đăng ký" $\rightarrow$ Xác nhận đăng ký $\rightarrow$ Đăng ký thành công.
    - **Kiểm tra 04 điều kiện nghiệp vụ trước khi lưu**:
      1. Học viên đã đăng nhập hệ thống.
      2. Lớp học đang ở trạng thái tuyển sinh (`MoDangKy` / `DangMo`).
      3. Lớp học chưa đủ số lượng sĩ số tối đa.
      4. Học viên chưa đăng ký lớp học đó trước đây.
  - **Lịch sử đăng ký**: Xem danh sách các lớp học đã đăng ký, trạng thái học phí, ngày ghi danh.
- **Tên tệp tin thực hiện**:
  - Tệp Controller: `HocVienController.cs`, `DangKyHocController.cs`
  - Tệp Model: `HocVien.cs`, `DangKyHoc.cs`
  - Tệp Giao diện: `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`, `DangKy.cshtml`, `XacNhanDangKy.cshtml`, `LichSuDangKy.cshtml`
  - Bảng CSDL: `HocVien`, `DangKyHoc`

### 4.4. KHIẾU QUỐC TOẢN – Module 4: Quản Lý Đăng Ký – Dashboard – Thống Kê
- **Nghiệp vụ chi tiết**:
  - **Quản lý đăng ký (Mục 4.18)**: Hiển thị danh sách đơn đăng ký gồm Mã đăng ký, Khóa học, Lớp học, Trạng thái (Chờ duyệt, Đã duyệt, Đã hủy), Học phí; Thực hiện duyệt hồ sơ hoặc từ chối đơn.
  - **Dashboard & Thống kê Admin (Mục 4.20)**:
    - Báo cáo **Doanh thu học phí** (theo tháng, năm, danh mục khóa học).
    - Biểu đồ tăng trưởng học viên và tỷ lệ đăng ký các ngoại ngữ (Anh, Hàn, Trung, Nhật).
    - Thẻ thống kê tổng số: Học viên, Giảng viên, Lớp học đang mở, Doanh thu lũy kế.
- **Tên tệp tin thực hiện**:
  - Tệp Controller: `YeuCauDangKyController.cs`, `ThongKeController.cs`
  - Tệp Model: `YeuCauDangKy.cs`, `CauHinhHeThong.cs`
  - Tệp Giao diện: `Index.cshtml`, `Details.cshtml`
  - Bảng CSDL: `YeuCauDangKy`, `CauHinhHeThong`

---

## 🚀 CÁC MODULE MỞ RỘNG NÂNG CAO

Ngoài 4 module bắt buộc, hệ thống được trang bị thêm các phân hệ nâng cao:

### 1. Phân Hệ Luyện Thi Trắc Nghiệm Trực Tuyến Đa Ngôn Ngữ
- **Tính năng**: Luyện thi thử theo format đề quốc tế (IELTS, TOEIC 200 câu, TOPIK I/II, HSK 1-6, JLPT N1-N5). Hỗ trợ phát file Audio bản ngữ, đồng hồ đếm ngược, nộp bài, tự động chấm điểm, hiển thị giải thích đáp án chi tiết và thảo luận đề thi.
- **Tên tệp tin**: `LuyenThiController.cs`, `DeThi.cs`, `PhanThi.cs`, `CauHoiLuyenThi.cs`, `KetQuaLuyenThi.cs`, `BinhLuanDeThi.cs`, `Index.cshtml`, `LamBai.cshtml`, `KetQua.cshtml`, `LichSuThi.cshtml`.
- **Bảng CSDL**: `DeThi`, `PhanThi`, `CauHoiLuyenThi`, `KetQuaLuyenThi`, `BinhLuanDeThi`.

### 2. Phân Hệ Hội Thoại Realtime & Gọi Video Call WebRTC
- **Tính năng**: Kênh chat lớp học thời gian thực qua SignalR và phòng đàm thoại Video Call trực tiếp giữa học viên và giáo viên chuẩn WebRTC P2P (hỗ trợ chuyển đổi Camera, Micro, kiểm tra tín hiệu STUN/TURN).
- **Tên tệp tin**: `ChatController.cs`, `CallHub.cs`, `NhomChat.cs`, `ThanhVienNhom.cs`, `TinNhan.cs`, `Index.cshtml`, `Room.cshtml`.
- **Bảng CSDL**: `NhomChat`, `ThanhVienNhom`, `TinNhan`.

### 3. Phân Hệ Mạng Xã Hội Ngoại Ngữ & Diễn Đàn Học Tập
- **Tính năng**: Bảng tin trung tâm, chia sẻ mẹo học tập, đặt câu hỏi, thích bài viết, bình luận đa cấp, báo cáo vi phạm nội dung.
- **Tên tệp tin**: `BaiVietController.cs`, `BinhLuanController.cs`, `BaiViet.cs`, `BinhLuan.cs`, `ThichBaiViet.cs`, `BaoCaoBaiViet.cs`, `Index.cshtml`, `ChiTiet.cshtml`, `Create.cshtml`.
- **Bảng CSDL**: `BaiViet`, `BinhLuan`, `ThichBaiViet`, `BaoCaoBaiViet`.

---

## 📁 DANH SÁCH TỆP TIN TRONG DỰ ÁN

| Nhóm Tệp Tin | Danh Sách Tên Tệp |
|:---|:---|
| **Bộ điều khiển (Controllers)** | `TaiKhoanController.cs`<br/>`KhoaHocController.cs`<br/>`LopHocController.cs`<br/>`GiaoVienController.cs`<br/>`HocVienController.cs`<br/>`DangKyHocController.cs`<br/>`YeuCauDangKyController.cs`<br/>`ThongKeController.cs`<br/>`LuyenThiController.cs`<br/>`ChatController.cs`<br/>`BaiVietController.cs`<br/>`BinhLuanController.cs`<br/>`HomeController.cs`<br/>`LandingController.cs` |
| **Thực thể dữ liệu (Models)** | `TaiKhoan.cs`<br/>`KhoaHoc.cs`<br/>`LopHoc.cs`<br/>`GiaoVien.cs`<br/>`HocVien.cs`<br/>`DangKyHoc.cs`<br/>`YeuCauDangKy.cs`<br/>`CauHinhHeThong.cs`<br/>`DeThi.cs`<br/>`PhanThi.cs`<br/>`CauHoiLuyenThi.cs`<br/>`KetQuaLuyenThi.cs`<br/>`BinhLuanDeThi.cs`<br/>`NhomChat.cs`<br/>`ThanhVienNhom.cs`<br/>`TinNhan.cs`<br/>`BaiViet.cs`<br/>`BinhLuan.cs`<br/>`ThichBaiViet.cs`<br/>`BaoCaoBaiViet.cs` |
| **Tín hiệu thời gian thực (Hubs)** | `CallHub.cs`<br/>`ChatHub.cs`<br/>`VideoCallHub.cs` |
| **Dịch vụ nền & Dữ liệu (Services & Data)** | `EmailService.cs`<br/>`ImageHelper.cs`<br/>`TrungTamDbContext.cs`<br/>`DbInitializer.cs`<br/>`LuyenThiDataGenerator.cs` |
| **Cơ sở dữ liệu (Database Files)** | `TrungTamNgoaiNguDb.sql`<br/>`TrungTamNgoaiNguDb.bak` |
| **Cấu hình & Tài liệu (Config & Docs)** | `appsettings.json`<br/>`Program.cs`<br/>`README.md`<br/>`DESIGN.md`<br/>`PHAN_CHIA_MODULE_DU_AN.md` |

---

## 🗄 CƠ SỞ DỮ LIỆU & KHÔI PHỤC

Hệ thống cung cấp **02 tệp cơ sở dữ liệu hoàn chỉnh** tại thư mục dự án:
- **`TrungTamNgoaiNguDb.sql`** (~5.5 MB): File kịch bản SQL chứa định nghĩa 20 bảng, khóa chính, khóa ngoại, chỉ mục và hơn 8,000 dòng dữ liệu mẫu (được chú thích rõ ràng theo từng Module 1, 2, 3, 4).
- **`TrungTamNgoaiNguDb.bak`** (~15.8 MB): File sao lưu nhị phân chuẩn SQL Server.

### Cách Khôi Phục:
- **Cách 1 (Khuyên dùng - Chạy Script SQL)**: Mở **SQL Server Management Studio (SSMS)**, kết nối tới SQL Server $\rightarrow$ Mở file `TrungTamNgoaiNguDb.sql` $\rightarrow$ Nhấn **Execute (F5)**.
- **Cách 2 (Restore file .BAK)**: Trong SSMS, chuột phải vào **Databases** $\rightarrow$ Chọn **Restore Database...** $\rightarrow$ Chọn file `TrungTamNgoaiNguDb.bak` $\rightarrow$ Nhấn **OK**.

---

## 💻 HƯỚNG DẪN CÀI ĐẶT & CHẠY ỨNG DỤNG

### 1. Yêu cầu môi trường
- Hệ điều hành: Windows 10/11 (khuyên dùng) hoặc macOS/Linux.
- .NET 10 SDK đã được cài đặt (`dotnet --version` kiểm tra $\ge$ 10.0).
- SQL Server LocalDB hoặc SQL Server Express / Developer.

### 2. Các bước khởi chạy
1. **Mở Command Prompt / PowerShell** tại thư mục dự án:
   ```powershell
   cd TrungTamNgoaiNgu
   ```

2. **Khôi phục thư viện phụ thuộc (NuGet packages)**:
   ```powershell
   dotnet restore
   ```

3. **Biên dịch dự án**:
   ```powershell
   dotnet build
   ```

4. **Khởi chạy ứng dụng**:
   ```powershell
   dotnet run
   ```

5. **Truy cập ứng dụng**:
   - Trình duyệt máy chủ: `http://localhost:5000` hoặc `https://localhost:5001`
   - Thiết bị trong cùng mạng Wi-Fi (điện thoại/laptop khác để test gọi video call): `http://[Địa_Chỉ_IP_Máy_Bạn]:5000`

---

## 🔑 TÀI KHOẢN THỬ NGHIỆM (DEMO ACCOUNTS)

Hệ thống đã nạp sẵn các tài khoản mẫu cho từng nhóm quyền:

| Quyền Hạn (Role) | Tên Đăng Nhập | Mật Khẩu | Họ Tên Đại Diện | Quyền Hạn Truy Cập |
|:---|:---:|:---:|:---|:---|
| **Admin** | `admin` | `admin123` | Ban Quản Trị Trung Tâm | Toàn quyền Dashboard, Quản lý tài khoản, Khóa học, Lớp, Học viên, Duyệt đơn, Thống kê |
| **Giảng Viên** | `giaovien1`<br/>`giaovien2`<br/>`giaovien3` | `123456` | Cô Mai Anh (IELTS)<br/>Thầy Kenji Sato (JLPT)<br/>Cô Thu Trang (HSK) | Quản lý lớp phụ trách, Kênh chat lớp học, Phòng hội đàm Video Call |
| **Học Viên** | `hocvien1`<br/>`hocvien2`<br/>`hocvien3` | `123456` | Nguyễn Văn An<br/>Trần Thị Bích<br/>Lê Hoàng Cường | Đăng ký khóa học, Lịch sử học, Luyện thi trắc nghiệm, Diễn đàn bài viết, Chat |

---

## 📖 TÀI LIỆU KÈM THEO

- Xem chi tiết kiến trúc hệ thống, lược đồ cơ sở dữ liệu ERD, sơ đồ tuần tự nghiệp vụ và design system tại tệp: `DESIGN.md`.
- Xem bảng phân chia module và ánh xạ từng chức năng theo thành viên tại: `PHAN_CHIA_MODULE_DU_AN.md`.
