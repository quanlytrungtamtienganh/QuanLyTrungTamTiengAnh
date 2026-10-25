# HỆ THỐNG TRUY VẤN LINQ (LANGUAGE INTEGRATED QUERY) - TRUNG TÂM NGOẠI NGỮ

> **Học phần**: Phát triển ứng dụng Web / Lập trình .NET & CSDL  
> **Cơ sở dữ liệu & ORM**: Microsoft SQL Server & Entity Framework Core  
> **Dự án**: Website Quản lý & Học trực tuyến Trung Tâm Ngoại Ngữ  
> **Tài liệu phân chia chức năng**: 04 Module chính tương ứng với 04 thành viên (Sinh viên 1 - Sinh viên 4)

---

## 📑 MỤC LỤC PHÂN CHIA 4 MODULE LINQ

| STT | Thư mục Module | Sinh viên phụ trách | Nhóm chức năng chính |
|:---:|:---|:---:|:---|
| **1** | [SinhVien1_Module1_TaiKhoan_KhoaHoc](./SinhVien1_Module1_TaiKhoan_KhoaHoc) | **Sinh viên 1** | **Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học** |
| **2** | [SinhVien2_Module2_LopHoc_TimKiem_PhanTrang](./SinhVien2_Module2_LopHoc_TimKiem_PhanTrang) | **Sinh viên 2** | **Quản lý lớp học – Tìm kiếm – Lọc – Sắp xếp – Phân trang** |
| **3** | [SinhVien3_Module3_HocVien_DangKyLopHoc](./SinhVien3_Module3_HocVien_DangKyLopHoc) | **Sinh viên 3** | **Quản lý học viên – Đăng ký lớp học – Lịch sử đăng ký** |
| **4** | [SinhVien4_Module4_QuanLyDangKy_Dashboard_ThongKe](./SinhVien4_Module4_QuanLyDangKy_Dashboard_ThongKe) | **Sinh viên 4** | **Quản lý đăng ký – Dashboard – Thống kê** |

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
