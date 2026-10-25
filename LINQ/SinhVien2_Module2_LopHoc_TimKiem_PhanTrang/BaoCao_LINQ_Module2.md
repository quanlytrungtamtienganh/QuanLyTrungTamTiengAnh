# BÁO CÁO CÂU LỆNH LINQ - TRẦN TRỌNG TRÁNG (MODULE 2)

## 📌 THÔNG TIN MODULE
* **Sinh viên phụ trách**: TRẦN TRỌNG TRÁNG
* **Tên Module**: Module 2 - Quản lý lớp học – Tìm kiếm – Lọc – Sắp xếp – Phân trang
* **Các bảng dữ liệu liên quan**: `LopHoc`, `KhoaHoc`, `GiaoVien`, `DangKyHoc`
* **File mã nguồn C#**: `LINQ_Module2_LopHoc_TimKiem_PhanTrang.cs`

---

## 📋 TỔNG HỢP CÁC CÂU TRUY VẤN LINQ TRONG MODULE 2

### I. PHẦN QUẢN LÝ LỚP HỌC (CLASSROOM MANAGEMENT)

#### 1. Lấy danh sách toàn bộ lớp học kèm khóa học và giáo viên
* **Mục đích**: Hiển thị bảng danh sách các lớp học tại trung tâm kèm thông tin Khóa học và Giáo viên phụ trách.
* **Cú pháp Method Syntax**:
```csharp
var lopHocs = await _context.LopHocs
    .AsNoTracking()
    .Include(l => l.KhoaHoc)
    .Include(l => l.GiaoVien)
    .OrderByDescending(l => l.NgayKhaiGiang)
    .ToListAsync();
```
* **Cú pháp Query Syntax**:
```csharp
var query = from l in _context.LopHocs
            orderby l.NgayKhaiGiang descending
            select l;
```

---

#### 2. Chi tiết lớp học và tính sĩ số học viên thực tế
* **Mục đích**: Kiểm tra sĩ số hiện tại so với sĩ số tối đa của lớp học.
* **Cú pháp**:
```csharp
var lop = await _context.LopHocs
    .Include(l => l.KhoaHoc)
    .Include(l => l.GiaoVien)
    .Include(l => l.DangKyHocs)
    .FirstOrDefaultAsync(l => l.MaLop == maLop);

int soHocVienHienTai = lop.DangKyHocs.Count(d => d.TrangThai == "Đã xác nhận");
int choConLai = Math.Max(0, lop.SoLuongToiDa - soHocVienHienTai);
```

---

#### 3. Kiểm tra lớp học còn chỗ trống hay không
* **Mục đích**: Khóa nút đăng ký khi lớp học đã đầy sĩ số.
* **Cú pháp**:
```csharp
int daDangKy = lop.DangKyHocs.Count(d => d.TrangThai == "Đã xác nhận");
bool conCho = daDangKy < lop.SoLuongToiDa;
```

---

### II. PHẦN TÌM KIẾM VÀ LỌC (SEARCH & FILTER)

#### 4. Tìm kiếm lớp học tổng hợp đa trường
* **Mục đích**: Thanh tìm kiếm linh hoạt hỗ trợ người dùng gõ tên lớp, khóa học, giáo viên hoặc phòng học.
* **Cú pháp**:
```csharp
var result = await _context.LopHocs
    .AsNoTracking()
    .Include(l => l.KhoaHoc)
    .Include(l => l.GiaoVien)
    .Where(l => l.TenLop.ToLower().Contains(tuKhoa)
             || (l.KhoaHoc != null && l.KhoaHoc.TenKhoaHoc.ToLower().Contains(tuKhoa))
             || (l.GiaoVien != null && l.GiaoVien.HoTen.ToLower().Contains(tuKhoa))
             || l.PhongHoc.ToLower().Contains(tuKhoa)
             || l.LichHoc.ToLower().Contains(tuKhoa))
    .OrderByDescending(l => l.NgayKhaiGiang)
    .ToListAsync();
```

---

#### 5. Lọc lớp học sắp khai giảng trong N ngày tới
* **Mục đích**: Nhắc nhở và ưu tiên hiển thị các lớp học chuẩn bị mở trong tháng.
* **Cú pháp**:
```csharp
var homNay = DateTime.Today;
var denNgay = homNay.AddDays(soNgayToi);

var lopSapMo = await _context.LopHocs
    .Where(l => l.NgayKhaiGiang >= homNay 
             && l.NgayKhaiGiang <= denNgay 
             && l.TrangThai == "Đang tuyển sinh")
    .OrderBy(l => l.NgayKhaiGiang)
    .ToListAsync();
```

---

### III. PHẦN SẮP XẾP VÀ PHÂN TRANG (SORTING & PAGING)

#### 6. Sắp xếp danh sách lớp học (Sorting Switch)
* **Mục đích**: Người dùng có thể chọn sắp xếp theo tên, ngày khai giảng, học phí.
* **Cú pháp**:
```csharp
query = sortBy switch
{
    "ten_desc" => query.OrderByDescending(l => l.TenLop),
    "ngay_asc" => query.OrderBy(l => l.NgayKhaiGiang),
    "ngay_desc" => query.OrderByDescending(l => l.NgayKhaiGiang),
    "hocphi_asc" => query.OrderBy(l => l.HocPhi),
    "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
    _ => query.OrderBy(l => l.TenLop)
};
```

---

#### 7. Phân trang dữ liệu với Skip và Take (Pagination)
* **Mục đích**: Giới hạn số lượng bản ghi hiển thị trên mỗi trang, tránh tràn bộ nhớ.
* **Cú pháp**:
```csharp
// Đếm tổng số bản ghi
int totalCount = await query.CountAsync();
int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

// Bỏ qua các trang trước và lấy đúng số lượng trang hiện tại
var items = await query
    .Skip((pageIndex - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```
* **Ý nghĩa toán tử LINQ**:
  * `Skip((pageIndex - 1) * pageSize)`: Dịch sang mệnh đề `OFFSET ... ROWS` trong SQL Server.
  * `Take(pageSize)`: Dịch sang mệnh đề `FETCH NEXT ... ROWS ONLY` trong SQL Server.

---

#### 8. Pipeline hoàn chỉnh: Tìm kiếm + Lọc + Sắp xếp + Phân trang kết hợp
* **Mục đích**: Mẫu chuẩn công nghiệp xử lý trọn gói request từ Client với hiệu năng tối ưu (chỉ thực thi 1 câu lệnh COUNT và 1 câu lệnh SELECT phân trang).
* **Cú pháp**: Xem phương thức `Q12_PipelineTimKiemLocSapXepPhanTrangAsync` trong file mã nguồn đính kèm.
