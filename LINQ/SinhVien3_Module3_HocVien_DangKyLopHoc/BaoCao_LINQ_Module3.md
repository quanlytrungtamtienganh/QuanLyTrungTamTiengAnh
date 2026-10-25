# BÁO CÁO CÂU LỆNH LINQ - SINH VIÊN 3 (MODULE 3)

## 📌 THÔNG TIN MODULE
* **Sinh viên phụ trách**: Sinh viên 3
* **Tên Module**: Module 3 - Quản lý học viên – Đăng ký lớp học – Lịch sử đăng ký
* **Các bảng dữ liệu liên quan**: `HocVien`, `TaiKhoan`, `DangKyHoc`, `LopHoc`, `KhoaHoc`
* **File mã nguồn C#**: `LINQ_Module3_HocVien_DangKyLopHoc.cs`

---

## 📋 TỔNG HỢP CÁC CÂU TRUY VẤN LINQ TRONG MODULE 3

### I. PHẦN QUẢN LÝ HỌC VIÊN (STUDENT MANAGEMENT)

#### 1. Lấy danh sách toàn bộ học viên kèm tài khoản liên kết
* **Mục đích**: Hiển thị bảng danh sách học viên tại trang quản lý của Giáo viên / Admin.
* **Cú pháp Method Syntax**:
```csharp
var hocViens = await _context.HocViens
    .AsNoTracking()
    .Include(h => h.TaiKhoan)
    .OrderBy(h => h.HoTen)
    .ToListAsync();
```
* **Cú pháp Query Syntax**:
```csharp
var query = from hv in _context.HocViens
            orderby hv.HoTen ascending
            select hv;
```

---

#### 2. Tìm kiếm học viên theo nhiều tiêu chí
* **Mục đích**: Tìm học viên theo họ tên, email, số điện thoại hoặc địa chỉ.
* **Cú pháp**:
```csharp
var result = await _context.HocViens
    .AsNoTracking()
    .Include(h => h.TaiKhoan)
    .Where(h => h.HoTen.ToLower().Contains(tuKhoa)
             || h.Email.ToLower().Contains(tuKhoa)
             || h.SoDienThoai.Contains(tuKhoa)
             || (h.DiaChi != null && h.DiaChi.ToLower().Contains(tuKhoa)))
    .OrderBy(h => h.HoTen)
    .ToListAsync();
```

---

#### 3. Thống kê học viên theo giới tính và độ tuổi trung bình
* **Mục đích**: Phân tích nhân khẩu học của học viên tại trung tâm.
* **Cú pháp**:
```csharp
var namHienTai = DateTime.Today.Year;
var stats = await _context.HocViens
    .GroupBy(h => h.GioiTinh)
    .Select(g => new
    {
        GioiTinh = g.Key,
        SoLuong = g.Count(),
        TuoiTrungBinh = g.Average(h => namHienTai - h.NgaySinh.Year)
    })
    .ToListAsync();
```

---

### II. PHẦN ĐĂNG KÝ LỚP HỌC (ENROLLMENT)

#### 4. Kiểm tra học viên đã đăng ký vào lớp học chưa (Tránh đăng ký trùng)
* **Mục đích**: Ngăn chặn học viên ấn đăng ký nhiều lần vào cùng một lớp.
* **Cú pháp**:
```csharp
bool daDangKy = await _context.DangKyHocs
    .AnyAsync(d => d.MaHocVien == maHocVien 
                && d.MaLop == maLop 
                && d.TrangThai != "Đã hủy");
```

---

#### 5. Lấy danh sách các lớp học viên CHƯA đăng ký và CÒN chỗ trống
* **Mục đích**: Gợi ý các lớp học mở mà học viên có thể đăng ký ngay.
* **Cú pháp**:
```csharp
// Subquery danh sách mã lớp học viên này đã đăng ký
var cacLopDaDangKyIds = _context.DangKyHocs
    .Where(d => d.MaHocVien == maHocVien && d.TrangThai != "Đã hủy")
    .Select(d => d.MaLop);

// Lấy các lớp đang tuyển sinh và chưa có trong danh sách trên
var lopCoTheDangKy = await _context.LopHocs
    .Where(l => l.TrangThai == "Đang tuyển sinh" 
             && !cacLopDaDangKyIds.Contains(l.MaLop))
    .OrderBy(l => l.NgayKhaiGiang)
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**: `!cacLopDaDangKyIds.Contains(l.MaLop)` sinh ra mệnh đề `NOT IN (SELECT MaLop FROM ...)` trong SQL, cực kỳ ngắn gọn và tối ưu.

---

### III. PHẦN LỊCH SỬ ĐĂNG KÝ (ENROLLMENT HISTORY)

#### 6. Lấy lịch sử đăng ký học của học viên
* **Mục đích**: Hiển thị bảng "Lịch sử đăng ký" trên trang cá nhân của học viên.
* **Cú pháp**:
```csharp
var lichSu = await _context.DangKyHocs
    .AsNoTracking()
    .Include(d => d.LopHoc)
        .ThenInclude(l => l.KhoaHoc)
    .Include(d => d.LopHoc)
        .ThenInclude(l => l.GiaoVien)
    .Where(d => d.MaHocVien == maHocVien)
    .OrderByDescending(d => d.NgayDangKy)
    .ToListAsync();
```

---

#### 7. Tính tổng số tiền học phí học viên đã nạp
* **Mục đích**: Thống kê tài chính cá nhân của học viên.
* **Cú pháp**:
```csharp
decimal tongHocPhi = await _context.DangKyHocs
    .Where(d => d.MaHocVien == maHocVien && d.TrangThai == "Đã xác nhận")
    .SumAsync(d => d.HocPhi);
```
* **Ý nghĩa toán tử LINQ**: `SumAsync` được EF Core dịch thẳng thành lệnh `SELECT SUM(HocPhi) FROM [DangKyHoc] WHERE ...`.

---

#### 8. Lấy danh sách tên các khóa học (không trùng lặp) đã tham gia
* **Mục đích**: Hiển thị các chứng chỉ / khóa học đã hoàn tất của học viên.
* **Cú pháp**:
```csharp
var danhSachKhoaHoc = await _context.DangKyHocs
    .Where(d => d.MaHocVien == maHocVien 
             && d.TrangThai == "Đã xác nhận" 
             && d.LopHoc != null 
             && d.LopHoc.KhoaHoc != null)
    .Select(d => d.LopHoc.KhoaHoc.TenKhoaHoc)
    .Distinct()
    .OrderBy(ten => ten)
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**: `Distinct()` loại bỏ các phần tử trùng lặp, tương đương `SELECT DISTINCT ...` trong SQL.
