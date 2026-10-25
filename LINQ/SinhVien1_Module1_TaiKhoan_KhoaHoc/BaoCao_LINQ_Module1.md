# BÁO CÁO CÂU LỆNH LINQ - VŨ TRƯỜNG AN (MODULE 1)

## 📌 THÔNG TIN MODULE
* **Sinh viên phụ trách**: VŨ TRƯỜNG AN
* **Tên Module**: Module 1 - Tài khoản – Đăng nhập và phân quyền – Quản lý khóa học
* **Các bảng dữ liệu liên quan**: `TaiKhoan`, `HocVien`, `GiaoVien`, `KhoaHoc`, `LopHoc`, `BinhLuan`
* **File mã nguồn C#**: `LINQ_Module1_TaiKhoan_KhoaHoc.cs`

---

## 📋 TỔNG HỢP CÁC CÂU TRUY VẤN LINQ TRONG MODULE 1

### I. PHẦN TÀI KHOẢN (ACCOUNT MANAGEMENT)

#### 1. Lấy danh sách tài khoản đang hoạt động kèm thông tin mở rộng
* **Mục đích**: Hiển thị bảng danh sách tài khoản trang quản trị Admin.
* **Cú pháp Method Syntax**:
```csharp
var taiKhoans = await _context.TaiKhoans
    .AsNoTracking()
    .Include(t => t.HocVien)
    .Include(t => t.GiaoVien)
    .Where(t => t.TrangThai == "HoatDong")
    .OrderBy(t => t.HoTen)
    .ToListAsync();
```
* **Cú pháp Query Syntax (SQL-like)**:
```csharp
var query = from tk in _context.TaiKhoans
            where tk.TrangThai == "HoatDong"
            orderby tk.HoTen ascending
            select tk;
```
* **Ý nghĩa toán tử LINQ**:
  * `Include`: Tải trước dữ liệu bảng liên kết 1-1 `HocVien` và `GiaoVien` (Eager Loading), chống lỗi N+1 Query.
  * `Where`: Lọc theo trường `TrangThai`.
  * `OrderBy`: Sắp xếp danh sách tên tài khoản theo thứ tự A-Z.

---

#### 2. Tìm kiếm tài khoản theo từ khóa đa trường
* **Mục đích**: Tìm tài khoản theo họ tên, tên đăng nhập hoặc email.
* **Cú pháp**:
```csharp
var result = await _context.TaiKhoans
    .Where(t => t.TenDangNhap.ToLower().Contains(tuKhoa)
             || t.HoTen.ToLower().Contains(tuKhoa)
             || t.Email.ToLower().Contains(tuKhoa))
    .OrderBy(t => t.TenDangNhap)
    .ToListAsync();
```

---

#### 3. Lọc tài khoản theo vai trò (Role-based Filter)
* **Mục đích**: Lọc danh sách chỉ gồm "Admin", "GiaoVien" hoặc "HocVien".
* **Cú pháp**:
```csharp
var result = await _context.TaiKhoans
    .Where(t => t.VaiTro.Equals(vaiTro, StringComparison.OrdinalIgnoreCase))
    .OrderByDescending(t => t.MaTaiKhoan)
    .ToListAsync();
```

---

#### 4. Kiểm tra tồn tại Tên đăng nhập và Email (Validation)
* **Mục đích**: Phục vụ chức năng Đăng ký tài khoản mới, kiểm tra trùng lặp.
* **Cú pháp**:
```csharp
bool daTonTaiUser = await _context.TaiKhoans
    .AnyAsync(t => t.TenDangNhap.ToLower() == tenDangNhap.Trim().ToLower());

bool daTonTaiEmail = await _context.TaiKhoans
    .AnyAsync(t => t.Email.ToLower() == email.Trim().ToLower());
```
* **Ý nghĩa toán tử LINQ**: `AnyAsync` sinh ra câu lệnh `EXISTS (SELECT 1 FROM ...)` trong SQL, ngừng quét ngay khi tìm thấy bản ghi đầu tiên, mang lại hiệu năng cao nhất.

---

### II. PHẦN ĐĂNG NHẬP VÀ PHÂN QUYỀN (AUTHENTICATION & AUTHORIZATION)

#### 5. Xác thực đăng nhập hệ thống
* **Mục đích**: Kiểm tra tên đăng nhập, mật khẩu và trạng thái hoạt động.
* **Cú pháp**:
```csharp
var user = await _context.TaiKhoans
    .Include(t => t.HocVien)
    .Include(t => t.GiaoVien)
    .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap
                           && t.MatKhau == matKhau
                           && t.TrangThai == "HoatDong");
```

---

#### 6. Lấy vai trò người dùng (Projection tối ưu)
* **Mục đích**: Lấy riêng cột `VaiTro` để kiểm tra phân quyền truy cập trang Admin/Giáo viên/Học viên.
* **Cú pháp**:
```csharp
string? vaiTro = await _context.TaiKhoans
    .Where(t => t.MaTaiKhoan == maTaiKhoan)
    .Select(t => t.VaiTro)
    .FirstOrDefaultAsync();
```
* **Ý nghĩa toán tử LINQ**: `Select(t => t.VaiTro)` chỉ truy vấn duy nhất 1 cột trong SQL (`SELECT TOP(1) [t].[VaiTro] FROM [TaiKhoan]...`), tiết kiệm băng thông và RAM.

---

#### 7. Thống kê số lượng tài khoản theo từng vai trò (GroupBy & Aggregation)
* **Mục đích**: Báo cáo tổng số tài khoản phân loại theo Admin, Giáo viên, Học viên.
* **Cú pháp Method Syntax**:
```csharp
var thongKe = await _context.TaiKhoans
    .GroupBy(t => t.VaiTro)
    .Select(g => new
    {
        VaiTro = g.Key,
        SoLuong = g.Count(),
        SoLuongHoatDong = g.Count(x => x.TrangThai == "HoatDong"),
        SoLuongBiKhoa = g.Count(x => x.TrangThai == "Khoa")
    })
    .ToListAsync();
```
* **Cú pháp Query Syntax**:
```csharp
var query = from tk in _context.TaiKhoans
            group tk by tk.VaiTro into g
            select new
            {
                VaiTro = g.Key,
                TongSoTaiKhoan = g.Count(),
                HoatDong = g.Count(x => x.TrangThai == "HoatDong")
            };
```

---

### III. PHẦN QUẢN LÝ KHÓA HỌC (COURSE MANAGEMENT)

#### 8. Lấy danh sách khóa học đang mở
* **Mục đích**: Hiển thị danh mục khóa học cho học viên chọn lựa trên trang chủ.
* **Cú pháp**:
```csharp
var khoaHocs = await _context.KhoaHocs
    .AsNoTracking()
    .Where(k => k.TrangThai == "Đang mở")
    .OrderBy(k => k.TenKhoaHoc)
    .ToListAsync();
```

---

#### 9. Tìm kiếm khóa học đa tiêu chí (Dynamic LINQ Chaining)
* **Mục đích**: Tìm kiếm kết hợp theo từ khóa, ngôn ngữ và cấp độ đào tạo.
* **Cú pháp**:
```csharp
var query = _context.KhoaHocs.AsNoTracking().AsQueryable();

if (!string.IsNullOrWhiteSpace(tuKhoa))
    query = query.Where(k => k.TenKhoaHoc.ToLower().Contains(tuKhoa) || k.MoTa.ToLower().Contains(tuKhoa));

if (!string.IsNullOrWhiteSpace(ngoaiNgu))
    query = query.Where(k => k.NgoaiNgu == ngoaiNgu);

if (!string.IsNullOrWhiteSpace(capDo))
    query = query.Where(k => k.CapDo == capDo);

var result = await query.OrderBy(k => k.TenKhoaHoc).ToListAsync();
```

---

#### 10. Lọc khóa học theo khoảng học phí (Price Range)
* **Mục đích**: Lọc khóa học phù hợp với ngân sách của học viên.
* **Cú pháp**:
```csharp
var result = await _context.KhoaHocs
    .Where(k => k.HocPhi >= giaMin && k.HocPhi <= giaMax)
    .OrderBy(k => k.HocPhi)
    .ToListAsync();
```

---

#### 11. Thống kê khóa học theo ngoại ngữ đào tạo
* **Mục đích**: Báo cáo tổng hợp số khóa học, học phí trung bình, học phí cao nhất / thấp nhất theo tiếng Anh, Trung, Nhật, Hàn.
* **Cú pháp**:
```csharp
var stats = await _context.KhoaHocs
    .GroupBy(k => k.NgoaiNgu)
    .Select(g => new
    {
        NgoaiNgu = g.Key,
        TongSoKhoaHoc = g.Count(),
        HocPhiTrungBinh = g.Average(k => k.HocPhi),
        HocPhiThapNhat = g.Min(k => k.HocPhi),
        HocPhiCaoNhat = g.Max(k => k.HocPhi)
    })
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**: `Average`, `Min`, `Max`, `Count` là các hàm tập hợp (Aggregate Functions) được EF Core dịch thẳng thành các hàm SQL tương ứng (`AVG`, `MIN`, `MAX`, `COUNT`).

---

#### 12. Lấy Top N khóa học có học phí cao nhất
* **Mục đích**: Tìm kiếm các khóa học chuyên sâu VIP / IELTS Intensive.
* **Cú pháp**:
```csharp
var topKhoaHoc = await _context.KhoaHocs
    .OrderByDescending(k => k.HocPhi)
    .Take(top)
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**: `OrderByDescending` sắp xếp giảm dần, `Take(N)` tương ứng với lệnh `SELECT TOP(N)` trong SQL Server.
