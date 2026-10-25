# BÁO CÁO CÂU LỆNH LINQ - KHIẾU QUỐC TOẢN (MODULE 4)

## 📌 THÔNG TIN MODULE
* **Sinh viên phụ trách**: KHIẾU QUỐC TOẢN
* **Tên Module**: Module 4 - Quản lý đăng ký – Dashboard – Thống kê
* **Các bảng dữ liệu liên quan**: `DangKyHoc`, `HocVien`, `LopHoc`, `KhoaHoc`, `GiaoVien`
* **File mã nguồn C#**: `LINQ_Module4_QuanLyDangKy_Dashboard_ThongKe.cs`

---

## 📋 TỔNG HỢP CÁC CÂU TRUY VẤN LINQ TRONG MODULE 4

### I. PHẦN QUẢN LÝ ĐĂNG KÝ (ENROLLMENT MANAGEMENT)

#### 1. Lấy danh sách toàn bộ phiếu đăng ký kèm thông tin liên quan
* **Mục đích**: Hiển thị bảng duyệt đơn đăng ký của trung tâm.
* **Cú pháp Method Syntax**:
```csharp
var dangKys = await _context.DangKyHocs
    .AsNoTracking()
    .Include(d => d.HocVien)
    .Include(d => d.LopHoc)
        .ThenInclude(l => l.KhoaHoc)
    .OrderByDescending(d => d.NgayDangKy)
    .ToListAsync();
```
* **Cú pháp Query Syntax**:
```csharp
var query = from d in _context.DangKyHocs
            orderby d.NgayDangKy descending
            select d;
```

---

#### 2. Lọc danh sách đăng ký theo trạng thái
* **Mục đích**: Lọc đơn "Chờ xác nhận" để nhân viên tư vấn gọi điện xác nhận học viên.
* **Cú pháp**:
```csharp
var result = await _context.DangKyHocs
    .Where(d => d.TrangThai == "Chờ xác nhận")
    .OrderByDescending(d => d.NgayDangKy)
    .ToListAsync();
```

---

#### 3. Tìm kiếm phiếu đăng ký đa trường
* **Mục đích**: Tìm kiếm theo tên học viên, số điện thoại hoặc tên lớp học.
* **Cú pháp**:
```csharp
var result = await _context.DangKyHocs
    .Where(d => (d.HocVien != null && d.HocVien.HoTen.ToLower().Contains(tuKhoa))
             || (d.HocVien != null && d.HocVien.SoDienThoai.Contains(tuKhoa))
             || (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(tuKhoa)))
    .OrderByDescending(d => d.NgayDangKy)
    .ToListAsync();
```

---

### II. PHẦN DASHBOARD TỔNG QUAN (ADMIN DASHBOARD)

#### 4. Tổng hợp các chỉ số KPI trên Dashboard
* **Mục đích**: Đếm và tính toán nhanh các chỉ số quan trọng trên màn hình quản trị.
* **Cú pháp**:
```csharp
int tongHocVien = await _context.HocViens.CountAsync();
int tongKhoaHoc = await _context.KhoaHocs.CountAsync();
int tongLopHoc = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang tuyển sinh" || l.TrangThai == "Đang học");
int dangKyChoDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ xác nhận");
decimal tongDoanhThu = await _context.DangKyHocs
    .Where(d => d.TrangThai == "Đã xác nhận")
    .SumAsync(d => (decimal?)d.HocPhi) ?? 0m;
```

---

#### 5. Thống kê tỷ lệ phần trăm đơn đăng ký theo trạng thái (Pie Chart)
* **Mục đích**: Vẽ biểu đồ tròn cơ cấu trạng thái đơn đăng ký.
* **Cú pháp**:
```csharp
int tongDangKy = await _context.DangKyHocs.CountAsync();

var tyLe = await _context.DangKyHocs
    .GroupBy(d => d.TrangThai)
    .Select(g => new
    {
        TrangThai = g.Key,
        SoLuong = g.Count(),
        TyLePhanTram = Math.Round((double)g.Count() * 100.0 / tongDangKy, 2)
    })
    .ToListAsync();
```

---

#### 6. Top 5 đơn đăng ký gần đây nhất (Recent Activities Widget)
* **Mục đích**: Hiển thị danh sách 5 học viên mới đăng ký gần nhất.
* **Cú pháp**:
```csharp
var top5MoiNhat = await _context.DangKyHocs
    .OrderByDescending(d => d.NgayDangKy)
    .Take(5)
    .ToListAsync();
```

---

### III. PHẦN BÁO CÁO THỐNG KÊ DOANH THU (STATISTICS & REVENUE)

#### 7. Thống kê doanh thu theo từng khóa học
* **Mục đích**: Đánh giá khóa học nào mang lại nguồn thu lớn nhất cho trung tâm.
* **Cú pháp**:
```csharp
var doanhThuKhoaHoc = await _context.DangKyHocs
    .Where(d => d.TrangThai == "Đã xác nhận" && d.LopHoc != null && d.LopHoc.KhoaHoc != null)
    .GroupBy(d => new
    {
        d.LopHoc.KhoaHoc.MaKhoaHoc,
        d.LopHoc.KhoaHoc.TenKhoaHoc,
        d.LopHoc.KhoaHoc.NgoaiNgu
    })
    .Select(g => new
    {
        MaKhoaHoc = g.Key.MaKhoaHoc,
        TenKhoaHoc = g.Key.TenKhoaHoc,
        NgoaiNgu = g.Key.NgoaiNgu,
        TongLuotDangKy = g.Count(),
        TongDoanhThu = g.Sum(d => d.HocPhi),
        HocPhiTrungBinh = g.Average(d => d.HocPhi)
    })
    .OrderByDescending(x => x.TongDoanhThu)
    .ToListAsync();
```

---

#### 8. Thống kê doanh thu 12 tháng trong năm (Bar Chart / Line Chart)
* **Mục đích**: Vẽ biểu đồ biến động doanh thu theo từng tháng trong năm để ban giám đốc theo dõi.
* **Cú pháp**:
```csharp
var doanhThuThang = await _context.DangKyHocs
    .Where(d => d.TrangThai == "Đã xác nhận" && d.NgayDangKy.Year == nam)
    .GroupBy(d => d.NgayDangKy.Month)
    .Select(g => new
    {
        Thang = g.Key,
        SoLuongDon = g.Count(),
        DoanhThu = g.Sum(d => d.HocPhi)
    })
    .OrderBy(x => x.Thang)
    .ToListAsync();
```

---

#### 9. Thống kê số lượng lớp học và học viên theo giáo viên phụ trách
* **Mục đích**: Đánh giá hiệu suất giảng dạy và khối lượng công việc của từng giáo viên.
* **Cú pháp**:
```csharp
var statsGiaoVien = await _context.GiaoViens
    .Select(gv => new
    {
        gv.MaGiaoVien,
        gv.HoTen,
        gv.ChuyenMon,
        TongSoLop = gv.LopHocs.Count(),
        TongSoHocVien = gv.LopHocs.SelectMany(l => l.DangKyHocs).Count(d => d.TrangThai == "Đã xác nhận")
    })
    .OrderByDescending(x => x.TongSoHocVien)
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**: `SelectMany` giúp làm phẳng tập hợp (Flatten Collection) từ danh sách nhiều lớp học thành danh sách tất cả các đơn đăng ký học, sau đó dùng `Count(...)` có điều kiện.
