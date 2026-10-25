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
### 🎯 Mục tiêu: Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học

- **Tệp Controller**: `TaiKhoanController.cs`, `KhoaHocController.cs`
- **Tệp Model**: `TaiKhoan.cs`, `KhoaHoc.cs`
- **Tệp Giao diện**: `DangNhap.cshtml`, `DangKy.cshtml`, `Index.cshtml`, `Edit.cshtml`, `Create.cshtml`, `Details.cshtml`, `Delete.cshtml`
- **Bảng CSDL**: `TaiKhoan`, `KhoaHoc`

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
### 🎯 Mục tiêu: Quản lý đăng ký – Dashboard – Thống kê

- **Tệp Controller**: `YeuCauDangKyController.cs`, `ThongKeController.cs`
- **Tệp Model**: `YeuCauDangKy.cs`, `CauHinhHeThong.cs`
- **Tệp Giao diện**: `Index.cshtml`, `Details.cshtml`
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
