# BẢNG PHÂN CHIA CHỨC NĂNG & ÁNH XẠ TỆP (FILE MAPPING) ĐỒ ÁN TRUNG TÂM NGOẠI NGỮ

> **Hệ thống**: Website Quản Lý & Học Tập Trực Tuyến Trung Tâm Ngoại Ngữ  
> **Công nghệ**: ASP.NET Core MVC .NET 10, Entity Framework Core, SQL Server, SignalR & WebRTC  
> **Cấu trúc phân chia**: 04 Module chính (tương ứng 04 thành viên) + Các Module mở rộng nâng cao.

---

## 📌 BẢNG TỔNG HỢP ÁNH XẠ MODULE - TỆP TIN & CƠ SỞ DỮ LIỆU

```
+--------------------------------------------------------------------------------------------------------------------+
| MODULE                 | THÀNH VIÊN ĐẢM NHIỆM | BẢNG DATABASE           | CONTROLLERS & SERVICES  | TỆP GIAO DIỆN  |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module 1               | VŨ TRƯỜNG AN         | TaiKhoan, KhoaHoc       | TaiKhoanController.cs   | DangNhap.cshtml|
| (Tài khoản - Khóa học) |                      |                         | KhoaHocController.cs    | DangKy.cshtml  |
|                        |                      |                         |                         | Index.cshtml   |
|                        |                      |                         |                         | Edit.cshtml    |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module 2               | TRẦN TRỌNG TRÁNG     | LopHoc, GiaoVien        | LopHocController.cs     | Index.cshtml   |
| (Lớp học - Tìm/Lọc/SX) |                      |                         | GiaoVienController.cs   | Create.cshtml  |
|                        |                      |                         |                         | Edit.cshtml    |
|                        |                      |                         |                         | Delete.cshtml  |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module 3               | PHẠM THANH TÙNG      | HocVien, DangKyHoc      | HocVienController.cs    | Index.cshtml   |
| (Học viên - Đăng ký)   |                      |                         | DangKyHocController.cs  | DangKy.cshtml  |
|                        |                      |                         |                         | XacNhanDangKy  |
|                        |                      |                         |                         | LichSuDangKy   |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module 4               | KHIẾU QUỐC TOẢN      | YeuCauDangKy,           | YeuCauDangKyController  | Index.cshtml   |
| (QL ĐK - Thống kê)     |                      | CauHinhHeThong          | ThongKeController.cs    | Details.cshtml |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module Mở Rộng 1       | Nâng cao             | DeThi, PhanThi,         | LuyenThiController.cs   | Index.cshtml   |
| (Luyện thi trắc nghiệm)|                      | CauHoiLuyenThi,         |                         | LamBai.cshtml  |
|                        |                      | KetQuaLuyenThi, BinhLuan|                         | KetQua.cshtml  |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module Mở Rộng 2       | Nâng cao             | NhomChat, ThanhVienNhom,| ChatController.cs       | Index.cshtml   |
| (Chat & Video Call)    |                      | TinNhan                 | CallHub.cs, ChatHub.cs  | Room.cshtml    |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
| Module Mở Rộng 3       | Nâng cao             | BaiViet, BinhLuan,      | BaiVietController.cs    | Index.cshtml   |
| (Mạng xã hội diễn đàn) |                      | ThichBaiViet, BaoCao    | BinhLuanController.cs   | ChiTiet.cshtml |
+------------------------+----------------------+-------------------------+-------------------------+----------------+
```

---

## 4.1. VŨ TRƯỜNG AN – MODULE 1
### 🎯 Mục tiêu: Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học – Đổi mật khẩu OTP Gmail – Xóa tài khoản an toàn

- **Chi tiết các chức năng phụ trách**:
  1. **Đăng nhập, đăng ký và phân quyền hệ thống**: Xác thực phiên (`Session`), phân 3 quyền hạn (`Admin`, `GiaoVien`, `HocVien`).
  2. **Chức năng Đổi mật khẩu / Quên mật khẩu qua mã OTP Gmail**:
     - Người dùng nhập Gmail $\rightarrow$ đối chiếu hệ thống (báo lỗi nếu không tồn tại).
     - Gửi duy nhất **01 mã OTP** 6 số ngẫu nhiên qua SMTP MailKit (khắc phục hoàn toàn lỗi gửi nhiều mã cùng lúc).
     - Mã OTP có hiệu lực chính xác **5 phút**; quá 5 phút mã tự vô hiệu hóa.
     - Giới hạn nghiêm ngặt tối đa **5 lần gửi mã/ngày** cho mỗi tài khoản (kiểm soát qua bảng `OtpDoiMatKhau` và `LichSuDoiMatKhau`).
     - Giao diện có bộ đếm ngược 5 phút thời gian thực, nút gửi lại mã tích hợp cơ chế Anti-Double-Click và tự động khóa khi hết lượt 5 lần/ngày.
     - Xác nhận mã OTP đúng và mật khẩu mới hợp lệ ($\ge 6$ ký tự) sẽ cập nhật ngay vào hệ thống.
  3. **Chức năng Xóa tài khoản an toàn cho Admin (Safe Cascade Delete)**:
     - Tích hợp Modal xác nhận xóa nhanh trực tiếp trên danh sách `Index.cshtml` và trang `Delete.cshtml`.
     - Tự động dọn dẹp an toàn toàn bộ dữ liệu phụ thuộc để tránh lỗi khóa ngoại (Foreign Key Restrict): `DangKyHoc`, `BinhLuan`, `BaiViet`, `ThichBaiViet`, `ThanhVienNhom`, `TinNhan`, `OtpDoiMatKhau`, `LichSuDoiMatKhau`.
     - Gỡ phân công giảng dạy (`MaGiaoVien = null`) để bảo toàn dữ liệu lớp học và có thể phân công giảng viên mới.
     - Cơ chế chặn Admin tự xóa chính mình đang đăng nhập.
  4. **Quản lý khóa học ngoại ngữ**: Thêm, Sửa, Xóa, Chi tiết khóa học tiếng Anh, Hàn, Trung, Nhật.
- **Tệp Controller**: `TaiKhoanController.cs`, `KhoaHocController.cs`
- **Tệp Model & ViewModel**: `TaiKhoan.cs`, `KhoaHoc.cs`, `OtpDoiMatKhau.cs`, `LichSuDoiMatKhau.cs`, `DoiMatKhauViewModel.cs`
- **Tệp Dịch vụ**: `EmailService.cs`, `IEmailService.cs`
- **Tệp Giao diện**: `Login.cshtml`, `DangKy.cshtml`, `DoiMatKhau.cshtml`, `XacNhanDoiMatKhau.cshtml`, `Profile.cshtml`, `Index.cshtml`, `Edit.cshtml`, `Create.cshtml`, `Details.cshtml`, `Delete.cshtml`
- **Bảng CSDL**: `TaiKhoan`, `KhoaHoc`, `OtpDoiMatKhau`, `LichSuDoiMatKhau`

---

## 4.2. TRẦN TRỌNG TRÁNG – MODULE 2
### 🎯 Mục tiêu: Quản lý lớp học – Tìm kiếm – Lọc – Sắp xếp – Phân trang

- **Tệp Controller**: `LopHocController.cs`, `GiaoVienController.cs`
- **Tệp Model**: `LopHoc.cs`, `GiaoVien.cs`
- **Tệp Giao diện**: `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`, `Details.cshtml`
- **Bảng CSDL**: `LopHoc`, `GiaoVien`

---

## 4.3. PHẠM THANH TÙNG – MODULE 3
### 🎯 Mục tiêu: Quản lý học viên – Đăng ký lớp học – Lịch sử đăng ký

- **Tệp Controller**: `HocVienController.cs`, `DangKyHocController.cs`
- **Tệp Model**: `HocVien.cs`, `DangKyHoc.cs`
- **Tệp Giao diện**: `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`, `DangKy.cshtml`, `XacNhanDangKy.cshtml`, `LichSuDangKy.cshtml`
- **Bảng CSDL**: `HocVien`, `DangKyHoc`

---

## 4.4. KHIẾU QUỐC TOẢN – MODULE 4
### 🎯 Mục tiêu: Quản lý đăng ký – Dashboard – Thống kê – Tối ưu hóa yêu cầu tạo tài khoản & Duyệt tài khoản tức thì

- **Chi tiết các chức năng phụ trách**:
  1. **Tối ưu hóa chức năng Yêu cầu tạo tài khoản (Landing Page)**:
     - Gửi biểu mẫu yêu cầu qua AJAX bất đồng bộ (`fetch` API), không cần tải lại trang.
     - Tốc độ phản hồi cực nhanh (~0.2s), hiển thị thông báo alert trạng thái tại chỗ.
     - Cơ chế Upsert thông minh: tự động cập nhật thông tin nếu đơn đang ở trạng thái `ChoDuyet`, chống rơi rụng yêu cầu và không tạo rác dữ liệu.
  2. **Duyệt tạo tài khoản tách gửi mail ngầm (Background Task)**:
     - Admin nhấn "Tạo tài khoản" $\rightarrow$ hệ thống lưu tài khoản và phản hồi ngay lập tức cho Admin (<0.1s).
     - Tác vụ gửi email thông tin đăng nhập được đẩy sang tiến trình nền (`Task.Run`), hoàn toàn không gây treo đơ giao diện khi mạng chậm.
     - Nút xác nhận có loading spinner và cơ chế khóa nút chống click lặp.
     - Tự động cập nhật số lượng yêu cầu mới (badge thông báo) mỗi 10 giây trên thanh điều hướng Admin (`_AdminLayout.cshtml`).
  3. **Quản lý danh sách đăng ký & Duyệt hồ sơ**: Hiển thị danh sách, duyệt học viên vào lớp hoặc từ chối đơn.
  4. **Dashboard & Báo cáo thống kê Admin**: Báo cáo doanh thu học phí theo thời gian và danh mục khóa học, biểu đồ học viên, thẻ thống kê tổng quan.
- **Tệp Controller**: `YeuCauDangKyController.cs`, `ThongKeController.cs`, `LandingController.cs`
- **Tệp Model**: `YeuCauDangKy.cs`, `CauHinhHeThong.cs`
- **Tệp Giao diện**: `Index.cshtml`, `Details.cshtml`, `_AdminLayout.cshtml`
- **Bảng CSDL**: `YeuCauDangKy`, `CauHinhHeThong`

---

## 🌟 CÁC MODULE MỞ RỘNG (NÂNG CAO)

### 1. Module Luyện Thi Trực Tuyến Đa Ngôn Ngữ
- **Tệp Controller**: `LuyenThiController.cs`
- **Tệp Model**: `DeThi.cs`, `PhanThi.cs`, `CauHoiLuyenThi.cs`, `KetQuaLuyenThi.cs`, `BinhLuanDeThi.cs`
- **Tệp Giao diện**: `Index.cshtml`, `LamBai.cshtml`, `KetQua.cshtml`, `LichSuThi.cshtml`
- **Bảng CSDL**: `DeThi`, `PhanThi`, `CauHoiLuyenThi`, `KetQuaLuyenThi`, `BinhLuanDeThi`

### 2. Module Hội Thoại Realtime & Video Call WebRTC
- **Tệp Controller & Hubs**: `ChatController.cs`, `CallHub.cs`, `ChatHub.cs`, `VideoCallHub.cs`
- **Tệp Model**: `NhomChat.cs`, `ThanhVienNhom.cs`, `TinNhan.cs`
- **Tệp Giao diện**: `Index.cshtml`, `Room.cshtml`
- **Bảng CSDL**: `NhomChat`, `ThanhVienNhom`, `TinNhan`

### 3. Module Mạng Xã Hội Ngoại Ngữ & Diễn Đàn Học Tập
- **Tệp Controller**: `BaiVietController.cs`, `BinhLuanController.cs`
- **Tệp Model**: `BaiViet.cs`, `BinhLuan.cs`, `ThichBaiViet.cs`, `BaoCaoBaiViet.cs`
- **Tệp Giao diện**: `Index.cshtml`, `ChiTiet.cshtml`, `Create.cshtml`
- **Bảng CSDL**: `BaiViet`, `BinhLuan`, `ThichBaiViet`, `BaoCaoBaiViet`

### 4. Module Hệ Thống Chung
- **Tệp Controller**: `HomeController.cs`, `LandingController.cs`
- **Tệp Giao diện**: `Index.cshtml`, `_Layout.cshtml`, `Error.cshtml`
