# HỆ THỐNG TRUY VẤN LINQ (LANGUAGE INTEGRATED QUERY) - TRUNG TÂM NGOẠI NGỮ

> **Học phần**: Phát triển ứng dụng Web / Lập trình .NET & CSDL  
> **Cơ sở dữ liệu & ORM**: Microsoft SQL Server & Entity Framework Core  
> **Dự án**: Website Quản lý & Học trực tuyến Trung Tâm Ngoại Ngữ  
> **Tài liệu phân chia chức năng**: 04 Module chính tương ứng với 04 thành viên (VŨ TRƯỜNG AN - KHIẾU QUỐC TOẢN)

---

## 📑 MỤC LỤC PHÂN CHIA 4 MODULE LINQ

| STT | Thư mục Module | Sinh viên phụ trách | Nhóm chức năng chính |
|:---:|:---|:---:|:---|
| **1** | [VŨ TRƯỜNG AN_Module1_TaiKhoan_KhoaHoc](./VŨ TRƯỜNG AN_Module1_TaiKhoan_KhoaHoc) | **VŨ TRƯỜNG AN** | **Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học** |
| **2** | [TRẦN TRỌNG TRÁNG_Module2_LopHoc_TimKiem_PhanTrang](./TRẦN TRỌNG TRÁNG_Module2_LopHoc_TimKiem_PhanTrang) | **TRẦN TRỌNG TRÁNG** | **Quản lý lớp học – Tìm kiếm – Lọc – Sắp xếp – Phân trang** |
| **3** | [PHẠM THANH TÙNG_Module3_HocVien_DangKyLopHoc](./PHẠM THANH TÙNG_Module3_HocVien_DangKyLopHoc) | **PHẠM THANH TÙNG** | **Quản lý học viên – Đăng ký lớp học – Lịch sử đăng ký** |
| **4** | [KHIẾU QUỐC TOẢN_Module4_QuanLyDangKy_Dashboard_ThongKe](./KHIẾU QUỐC TOẢN_Module4_QuanLyDangKy_Dashboard_ThongKe) | **KHIẾU QUỐC TOẢN** | **Quản lý đăng ký – Dashboard – Thống kê** |

---

## 🎯 CẤU TRÚC TRONG MỖI FOLDER MODULE

Trong mỗi thư mục của từng thành viên gồm có:
1. **File C# Source Code (`.cs`)**:
   - Chứa các class nghiệp vụ LINQ hoàn chỉnh kế thừa hoặc làm việc trực tiếp với `TrungTamDbContext`.
   - Cung cấp song song cả **Method Syntax (Lambda Expression)** và **Query Syntax (Cú pháp giống SQL)**.
   - Có comment tiếng Việt chi tiết từng dòng lệnh, giải thích tường minh từ khóa LINQ (`Where`, `Select`, `OrderBy`, `GroupBy`, `Join`, `Skip`, `Take`, `Any`, `All`, `Sum`, `Count`, `Average`...).
2. **File Báo cáo kỹ thuật (`.md`)**:
   - Bảng phân tích chi tiết: Tên chức năng $\rightarrow$ Tham số đầu vào $\rightarrow$ Kết quả trả về $\rightarrow$ Đoạn mã LINQ $\rightarrow$ Ý nghĩa trong báo cáo đồ án.
   - Sẵn sàng sao chép trực tiếp vào báo cáo Word đồ án môn học.

---

## 🚀 KIẾN TRÚC & CÁCH SỬ DỤNG TRONG PROJECT

Tất cả các class LINQ được đặt trong namespace:
```csharp
namespace TrungTamNgoaiNgu.LINQ.Module1
namespace TrungTamNgoaiNgu.LINQ.Module2
namespace TrungTamNgoaiNgu.LINQ.Module3
namespace TrungTamNgoaiNgu.LINQ.Module4
```

Có thể dễ dàng Dependency Injection hoặc khởi tạo với `TrungTamDbContext`:
```csharp
var module1 = new LINQ_Module1_TaiKhoan_KhoaHoc(dbContext);
var danhSachKhoaHoc = await module1.Q11_LayDanhSachKhoaHocDangMoAsync();
```
