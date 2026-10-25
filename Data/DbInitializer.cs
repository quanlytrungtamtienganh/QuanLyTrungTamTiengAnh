using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Data
{
    public static partial class DbInitializer
    {
        public static void Initialize(TrungTamDbContext context)
        {
            // Tự động tạo cơ sở dữ liệu nếu chưa tồn tại
            context.Database.EnsureCreated();

            // Đảm bảo các bảng BinhLuan, GiaoVien, TinNhan và cột MaGiaoVien tồn tại
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BinhLuan' and xtype='U')
                    BEGIN
                        CREATE TABLE [BinhLuan] (
                            [MaBinhLuan] int NOT NULL IDENTITY,
                            [NoiDung] nvarchar(1000) NOT NULL,
                            [NgayBinhLuan] datetime2 NOT NULL DEFAULT GETDATE(),
                            [MaTaiKhoan] int NOT NULL,
                            [MaKhoaHoc] int NULL,
                            CONSTRAINT [PK_BinhLuan] PRIMARY KEY ([MaBinhLuan]),
                            CONSTRAINT [FK_BinhLuan_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE,
                            CONSTRAINT [FK_BinhLuan_KhoaHoc_MaKhoaHoc] FOREIGN KEY ([MaKhoaHoc]) REFERENCES [KhoaHoc] ([MaKhoaHoc]) ON DELETE SET NULL
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='GiaoVien' and xtype='U')
                    BEGIN
                        CREATE TABLE [GiaoVien] (
                            [MaGiaoVien] int NOT NULL IDENTITY,
                            [MaTaiKhoan] int NOT NULL,
                            [HoTen] nvarchar(100) NOT NULL,
                            [ChuyenMon] nvarchar(50) NOT NULL,
                            [HocVi] nvarchar(100) NULL,
                            [SoDienThoai] nvarchar(20) NULL,
                            [Email] nvarchar(100) NULL,
                            [GioiThieu] nvarchar(max) NULL,
                            [AnhDaiDien] nvarchar(255) NULL,
                            CONSTRAINT [PK_GiaoVien] PRIMARY KEY ([MaGiaoVien]),
                            CONSTRAINT [FK_GiaoVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OtpDoiMatKhau' and xtype='U')
                    BEGIN
                        CREATE TABLE [OtpDoiMatKhau] (
                            [Id] int NOT NULL IDENTITY,
                            [MaTaiKhoan] int NOT NULL,
                            [Email] nvarchar(100) NOT NULL,
                            [MaOtp] nvarchar(10) NOT NULL,
                            [ThoiGianTao] datetime2 NOT NULL DEFAULT GETDATE(),
                            [ThoiGianHetHan] datetime2 NOT NULL,
                            [DaSuDung] bit NOT NULL DEFAULT 0,
                            CONSTRAINT [PK_OtpDoiMatKhau] PRIMARY KEY ([Id]),
                            CONSTRAINT [FK_OtpDoiMatKhau_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LichSuDoiMatKhau' and xtype='U')
                    BEGIN
                        CREATE TABLE [LichSuDoiMatKhau] (
                            [Id] int NOT NULL IDENTITY,
                            [MaTaiKhoan] int NOT NULL,
                            [Email] nvarchar(100) NOT NULL,
                            [NgayDoi] date NOT NULL DEFAULT CAST(GETDATE() AS DATE),
                            [ThoiGianDoi] datetime2 NOT NULL DEFAULT GETDATE(),
                            CONSTRAINT [PK_LichSuDoiMatKhau] PRIMARY KEY ([Id]),
                            CONSTRAINT [FK_LichSuDoiMatKhau_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[LopHoc]') AND name = 'MaGiaoVien'
                    )
                    BEGIN
                        ALTER TABLE [LopHoc] ADD [MaGiaoVien] int NULL;
                        ALTER TABLE [LopHoc] ADD CONSTRAINT [FK_LopHoc_GiaoVien_MaGiaoVien] FOREIGN KEY ([MaGiaoVien]) REFERENCES [GiaoVien] ([MaGiaoVien]) ON DELETE SET NULL;
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TinNhan' and xtype='U')
                    BEGIN
                        CREATE TABLE [TinNhan] (
                            [MaTinNhan] int NOT NULL IDENTITY,
                            [NguoiGuiId] int NOT NULL,
                            [NguoiNhanId] int NOT NULL,
                            [NoiDung] nvarchar(2000) NULL,
                            [HinhAnhUrl] nvarchar(500) NULL,
                            [ThoiGian] datetime2 NOT NULL DEFAULT GETDATE(),
                            [DaXem] bit NOT NULL DEFAULT 0,
                            CONSTRAINT [PK_TinNhan] PRIMARY KEY ([MaTinNhan]),
                            CONSTRAINT [FK_TinNhan_TaiKhoan_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]),
                            CONSTRAINT [FK_TinNhan_TaiKhoan_NguoiNhanId] FOREIGN KEY ([NguoiNhanId]) REFERENCES [TaiKhoan] ([MaTaiKhoan])
                        );
                    END
                ");

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NhomChat' and xtype='U')
                        BEGIN
                            CREATE TABLE [NhomChat] (
                                [MaNhom] int NOT NULL IDENTITY,
                                [TenNhom] nvarchar(150) NOT NULL,
                                [AnhNhom] nvarchar(255) NULL,
                                [NguoiTaoId] int NOT NULL,
                                [NgayTao] datetime2 NOT NULL DEFAULT GETDATE(),
                                [CuocGoiDangDienRa] bit NOT NULL DEFAULT 0,
                                [LoaiCuocGoi] nvarchar(20) NULL,
                                [NguoiGoiId] int NULL,
                                [ThoiGianBatDauGoi] datetime2 NULL,
                                CONSTRAINT [PK_NhomChat] PRIMARY KEY ([MaNhom]),
                                CONSTRAINT [FK_NhomChat_TaiKhoan_NguoiTaoId] FOREIGN KEY ([NguoiTaoId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create NhomChat: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ThanhVienNhom' and xtype='U')
                        BEGIN
                            CREATE TABLE [ThanhVienNhom] (
                                [MaThanhVien] int NOT NULL IDENTITY,
                                [MaNhom] int NOT NULL,
                                [MaTaiKhoan] int NOT NULL,
                                [VaiTroTrongNhom] nvarchar(30) NOT NULL DEFAULT 'ThanhVien',
                                [NgayThamGia] datetime2 NOT NULL DEFAULT GETDATE(),
                                CONSTRAINT [PK_ThanhVienNhom] PRIMARY KEY ([MaThanhVien]),
                                CONSTRAINT [FK_ThanhVienNhom_NhomChat_MaNhom] FOREIGN KEY ([MaNhom]) REFERENCES [NhomChat] ([MaNhom]) ON DELETE CASCADE,
                                CONSTRAINT [FK_ThanhVienNhom_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create ThanhVienNhom: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[TinNhan]') AND name = 'MaNhom'
                        )
                        BEGIN
                            ALTER TABLE [TinNhan] ADD [MaNhom] int NULL;
                            ALTER TABLE [TinNhan] ADD CONSTRAINT [FK_TinNhan_NhomChat_MaNhom] FOREIGN KEY ([MaNhom]) REFERENCES [NhomChat] ([MaNhom]) ON DELETE CASCADE;
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter TinNhan MaNhom: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[TinNhan]') AND name = 'LoaiTinNhan'
                        )
                        BEGIN
                            ALTER TABLE [TinNhan] ADD [LoaiTinNhan] nvarchar(20) NOT NULL DEFAULT 'VanBan';
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter TinNhan LoaiTinNhan: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[TinNhan]') AND name = 'NguoiNhanId' AND is_nullable = 0
                        )
                        BEGIN
                            ALTER TABLE [TinNhan] ALTER COLUMN [NguoiNhanId] int NULL;
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter TinNhan NguoiNhanId NULL: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CauHinhHeThong' and xtype='U')
                        BEGIN
                            CREATE TABLE [CauHinhHeThong] (
                                [Khoa] nvarchar(100) NOT NULL,
                                [GiaTri] nvarchar(500) NOT NULL,
                                [MoTa] nvarchar(255) NULL,
                                [NgayCapNhat] datetime2 NOT NULL DEFAULT GETDATE(),
                                CONSTRAINT [PK_CauHinhHeThong] PRIMARY KEY ([Khoa])
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create CauHinhHeThong: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BaiViet' and xtype='U')
                        BEGIN
                            CREATE TABLE [BaiViet] (
                                [MaBaiViet] int NOT NULL IDENTITY,
                                [MaTaiKhoan] int NOT NULL,
                                [NoiDung] nvarchar(4000) NOT NULL,
                                [HinhAnhUrl] nvarchar(500) NULL,
                                [VideoUrl] nvarchar(500) NULL,
                                [PhongNen] nvarchar(50) NULL,
                                [NgayDang] datetime2 NOT NULL DEFAULT GETDATE(),
                                [TrangThai] nvarchar(30) NOT NULL DEFAULT 'DaDuyet',
                                [NguoiDuyetId] int NULL,
                                [NgayDuyet] datetime2 NULL,
                                CONSTRAINT [PK_BaiViet] PRIMARY KEY ([MaBaiViet]),
                                CONSTRAINT [FK_BaiViet_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION,
                                CONSTRAINT [FK_BaiViet_TaiKhoan_NguoiDuyetId] FOREIGN KEY ([NguoiDuyetId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create BaiViet: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BaoCaoBaiViet' and xtype='U')
                        BEGIN
                            CREATE TABLE [BaoCaoBaiViet] (
                                [MaBaoCao] int NOT NULL IDENTITY,
                                [MaBaiViet] int NOT NULL,
                                [NguoiBaoCaoId] int NOT NULL,
                                [LyDo] nvarchar(200) NOT NULL,
                                [ChiTiet] nvarchar(1000) NULL,
                                [NgayBaoCao] datetime2 NOT NULL DEFAULT GETDATE(),
                                [TrangThai] nvarchar(30) NOT NULL DEFAULT 'ChoXuLy',
                                CONSTRAINT [PK_BaoCaoBaiViet] PRIMARY KEY ([MaBaoCao]),
                                CONSTRAINT [FK_BaoCaoBaiViet_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE,
                                CONSTRAINT [FK_BaoCaoBaiViet_TaiKhoan_NguoiBaoCaoId] FOREIGN KEY ([NguoiBaoCaoId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create BaoCaoBaiViet: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[BinhLuan]') AND name = 'MaBaiViet'
                        )
                        BEGIN
                            ALTER TABLE [BinhLuan] ADD [MaBaiViet] int NULL;
                            ALTER TABLE [BinhLuan] ADD CONSTRAINT [FK_BinhLuan_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE;
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter BinhLuan MaBaiViet: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ThichBaiViet' and xtype='U')
                        BEGIN
                            CREATE TABLE [ThichBaiViet] (
                                [MaThich] int NOT NULL IDENTITY,
                                [MaBaiViet] int NOT NULL,
                                [MaTaiKhoan] int NOT NULL,
                                [NgayThich] datetime2 NOT NULL DEFAULT GETDATE(),
                                CONSTRAINT [PK_ThichBaiViet] PRIMARY KEY ([MaThich]),
                                CONSTRAINT [FK_ThichBaiViet_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE,
                                CONSTRAINT [FK_ThichBaiViet_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
                            );
                            CREATE UNIQUE INDEX [IX_ThichBaiViet_MaBaiViet_MaTaiKhoan] ON [ThichBaiViet] ([MaBaiViet], [MaTaiKhoan]);
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create ThichBaiViet: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[BaiViet]') AND name = 'MaBaiVietGoc'
                        )
                        BEGIN
                            ALTER TABLE [BaiViet] ADD [MaBaiVietGoc] int NULL;
                            ALTER TABLE [BaiViet] ADD CONSTRAINT [FK_BaiViet_BaiViet_MaBaiVietGoc] FOREIGN KEY ([MaBaiVietGoc]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE NO ACTION;
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter BaiViet MaBaiVietGoc: " + ex.Message); }

                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'[BaiViet]') AND name = 'SoChiaSe'
                        )
                        BEGIN
                            ALTER TABLE [BaiViet] ADD [SoChiaSe] int NOT NULL DEFAULT 0;
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Alter BaiViet SoChiaSe: " + ex.Message); }

                // Tạo bảng YeuCauDangKy (yêu cầu đăng ký tài khoản từ Landing page)
                try
                {
                    context.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='YeuCauDangKy' and xtype='U')
                        BEGIN
                            CREATE TABLE [YeuCauDangKy] (
                                [MaYeuCau] int NOT NULL IDENTITY,
                                [HoTen] nvarchar(150) NOT NULL,
                                [Email] nvarchar(150) NOT NULL,
                                [SoDienThoai] nvarchar(20) NOT NULL DEFAULT '',
                                [VaiTroMongMuon] nvarchar(30) NOT NULL DEFAULT 'HocVien',
                                [GhiChu] nvarchar(500) NULL,
                                [NgayGui] datetime2 NOT NULL DEFAULT GETDATE(),
                                [TrangThai] nvarchar(30) NOT NULL DEFAULT 'ChoXuLy',
                                CONSTRAINT [PK_YeuCauDangKy] PRIMARY KEY ([MaYeuCau])
                            );
                        END
                    ");
                }
                catch (Exception ex) { Console.WriteLine("Create YeuCauDangKy: " + ex.Message); }
            }
            catch (Exception ex) { Console.WriteLine("DbInitializer General: " + ex.Message); }

            // Nếu đã có dữ liệu tài khoản
            if (context.TaiKhoans.Any())
            {
                SeedCommentsIfEmpty(context);
                SeedTeachersAndMessagesIfEmpty(context);
                SeedGroupsIfEmpty(context);
                SeedPostsAndConfigIfEmpty(context);
                SeedLuyenThiIfEmpty(context);
                return;
            }

            // 1. NẠP TÀI KHOẢN (TaiKhoan)
            var taiKhoanAdmin = new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = "admin123", // Để sinh viên dễ test và chấm bài
                HoTen = "Ban Quản Trị Trung Tâm",
                Email = "admin@educenter.edu.vn",
                VaiTro = "Admin",
                TrangThai = "HoatDong"
            };

            var studentAccounts = new List<TaiKhoan>
            {
                new TaiKhoan { TenDangNhap = "hocvien1", MatKhau = "123456", HoTen = "Nguyễn Văn An", Email = "an.nguyen@gmail.com", VaiTro = "HocVien", TrangThai = "HoatDong" },
                new TaiKhoan { TenDangNhap = "hocvien2", MatKhau = "123456", HoTen = "Trần Thị Bích", Email = "bich.tran@gmail.com", VaiTro = "HocVien", TrangThai = "HoatDong" },
                new TaiKhoan { TenDangNhap = "hocvien3", MatKhau = "123456", HoTen = "Lê Hoàng Cường", Email = "cuong.le@gmail.com", VaiTro = "HocVien", TrangThai = "HoatDong" },
                new TaiKhoan { TenDangNhap = "hocvien4", MatKhau = "123456", HoTen = "Phạm Mai Dung", Email = "dung.pham@gmail.com", VaiTro = "HocVien", TrangThai = "HoatDong" },
                new TaiKhoan { TenDangNhap = "hocvien5", MatKhau = "123456", HoTen = "Vũ Đức Em", Email = "em.vu@gmail.com", VaiTro = "HocVien", TrangThai = "HoatDong" }
            };

            context.TaiKhoans.Add(taiKhoanAdmin);
            context.TaiKhoans.AddRange(studentAccounts);
            context.SaveChanges();

            // 2. NẠP HỒ SƠ HỌC VIÊN (HocVien)
            var hocViens = new List<HocVien>
            {
                new HocVien { MaTaiKhoan = studentAccounts[0].MaTaiKhoan, HoTen = "Nguyễn Văn An", NgaySinh = new DateTime(2002, 5, 14), GioiTinh = "Nam", SoDienThoai = "0912345671", Email = "an.nguyen@gmail.com", DiaChi = "Số 15 Cầu Giấy, Hà Nội" },
                new HocVien { MaTaiKhoan = studentAccounts[1].MaTaiKhoan, HoTen = "Trần Thị Bích", NgaySinh = new DateTime(2003, 8, 22), GioiTinh = "Nữ", SoDienThoai = "0912345672", Email = "bich.tran@gmail.com", DiaChi = "Số 48 Nguyễn Trãi, Thanh Xuân, Hà Nội" },
                new HocVien { MaTaiKhoan = studentAccounts[2].MaTaiKhoan, HoTen = "Lê Hoàng Cường", NgaySinh = new DateTime(2001, 11, 3), GioiTinh = "Nam", SoDienThoai = "0912345673", Email = "cuong.le@gmail.com", DiaChi = "Số 102 Hoàng Hoa Thám, Ba Đình, Hà Nội" },
                new HocVien { MaTaiKhoan = studentAccounts[3].MaTaiKhoan, HoTen = "Phạm Mai Dung", NgaySinh = new DateTime(2004, 3, 19), GioiTinh = "Nữ", SoDienThoai = "0912345674", Email = "dung.pham@gmail.com", DiaChi = "Tòa B3 Làng Quốc Tế Thăng Long, Cầu Giấy" },
                new HocVien { MaTaiKhoan = studentAccounts[4].MaTaiKhoan, HoTen = "Vũ Đức Em", NgaySinh = new DateTime(2002, 9, 29), GioiTinh = "Nam", SoDienThoai = "0912345675", Email = "em.vu@gmail.com", DiaChi = "Số 78 Giải Phóng, Hai Bà Trưng, Hà Nội" }
            };
            context.HocViens.AddRange(hocViens);
            context.SaveChanges();

            // 3. NẠP KHÓA HỌC (KhoaHoc)
            var khoaHocs = new List<KhoaHoc>
            {
                new KhoaHoc
                {
                    TenKhoaHoc = "IELTS Intensive Mục Tiêu 6.5 - 7.5+",
                    NgoaiNgu = "Tiếng Anh",
                    CapDo = "Cao cấp (C1/C2)",
                    HocPhi = 6500000m,
                    SoBuoi = 36,
                    MoTa = "Khóa học luyện thi IELTS chuyên sâu 4 kỹ năng Nghe - Nói - Đọc - Viết với giảng viên 8.5 IELTS và chấm bài trực tiếp.",
                    TrangThai = "Đang mở"
                },
                new KhoaHoc
                {
                    TenKhoaHoc = "Tiếng Anh Giao Tiếp Quốc Tế Phản Xạ Nhanh",
                    NgoaiNgu = "Tiếng Anh",
                    CapDo = "Trung cấp (B1/B2)",
                    HocPhi = 3800000m,
                    SoBuoi = 24,
                    MoTa = "Rèn luyện kỹ năng phát âm chuẩn IPA, tăng vốn từ vựng thực tế và phản xạ giao tiếp tự tin trong môi trường làm việc toàn cầu.",
                    TrangThai = "Đang mở"
                },
                new KhoaHoc
                {
                    TenKhoaHoc = "Tiếng Hàn Giao Tiếp & Luyện Thi TOPIK II",
                    NgoaiNgu = "Tiếng Hàn",
                    CapDo = "Trung cấp (B1/B2)",
                    HocPhi = 4200000m,
                    SoBuoi = 30,
                    MoTa = "Trang bị ngữ pháp trung cấp và chiến thuật giải đề thi TOPIK cấp 3-4, nâng cao kỹ năng đàm thoại phong cách chuẩn Hàn.",
                    TrangThai = "Đang mở"
                },
                new KhoaHoc
                {
                    TenKhoaHoc = "Tiếng Hàn Nhập Môn Cho Người Mới Bắt Đầu",
                    NgoaiNgu = "Tiếng Hàn",
                    CapDo = "Sơ cấp (A1/A2)",
                    HocPhi = 2500000m,
                    SoBuoi = 20,
                    MoTa = "Bảng chữ cái Hangeul, cách ghép âm, từ vựng cơ bản và các mẫu câu giao tiếp đời sống hàng ngày dễ hiểu.",
                    TrangThai = "Đang mở"
                },
                new KhoaHoc
                {
                    TenKhoaHoc = "Tiếng Nhật Cấp Tốc JLPT N3",
                    NgoaiNgu = "Tiếng Nhật",
                    CapDo = "Trung cấp (B1/B2)",
                    HocPhi = 4800000m,
                    SoBuoi = 32,
                    MoTa = "Khóa học tổng hợp Kanji, Ngữ pháp, Đọc hiểu và Nghe hiểu theo chuẩn cấu trúc đề thi JLPT N3 thực tế.",
                    TrangThai = "Đang mở"
                },
                new KhoaHoc
                {
                    TenKhoaHoc = "Tiếng Trung Toàn Diện HSK 3 - HSK 4",
                    NgoaiNgu = "Tiếng Trung",
                    CapDo = "Trung cấp (B1/B2)",
                    HocPhi = 4500000m,
                    SoBuoi = 30,
                    MoTa = "Phát triển toàn diện từ vựng, ngữ pháp và khẩu ngữ tiếng Trung thương mại, cam kết đầu ra chứng chỉ HSK 3/4.",
                    TrangThai = "Đang mở"
                }
            };
            context.KhoaHocs.AddRange(khoaHocs);
            context.SaveChanges();

            // 4. NẠP LỚP HỌC (LopHoc)
            var lopHocs = new List<LopHoc>
            {
                new LopHoc
                {
                    TenLop = "IELTS-K36-A1",
                    MaKhoaHoc = khoaHocs[0].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(5),
                    LichHoc = "Tối 2-4-6 (18:00 - 20:00)",
                    PhongHoc = "Phòng Lab A.201",
                    SoLuongToiDa = 20,
                    HocPhi = 6500000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "IELTS-K36-A2",
                    MaKhoaHoc = khoaHocs[0].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(12),
                    LichHoc = "Tối 3-5-7 (19:30 - 21:30)",
                    PhongHoc = "Phòng A.203",
                    SoLuongToiDa = 18,
                    HocPhi = 6500000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "ENG-COMM-24B",
                    MaKhoaHoc = khoaHocs[1].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(7),
                    LichHoc = "Tối 3-5-7 (18:00 - 19:30)",
                    PhongHoc = "Phòng B.102",
                    SoLuongToiDa = 25,
                    HocPhi = 3800000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "KOR-TOPIK-12",
                    MaKhoaHoc = khoaHocs[2].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(10),
                    LichHoc = "Tối 2-4-6 (19:30 - 21:00)",
                    PhongHoc = "Phòng C.301",
                    SoLuongToiDa = 22,
                    HocPhi = 4200000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "KOR-BASIC-08",
                    MaKhoaHoc = khoaHocs[3].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(3),
                    LichHoc = "Sáng Thứ 7 & CN (08:30 - 11:30)",
                    PhongHoc = "Phòng C.305",
                    SoLuongToiDa = 25,
                    HocPhi = 2500000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "JPN-N3-K15",
                    MaKhoaHoc = khoaHocs[4].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(15),
                    LichHoc = "Tối 2-4-6 (18:30 - 20:30)",
                    PhongHoc = "Phòng D.101",
                    SoLuongToiDa = 20,
                    HocPhi = 4800000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "CHN-HSK4-05",
                    MaKhoaHoc = khoaHocs[5].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(8),
                    LichHoc = "Tối 3-5-7 (18:00 - 20:00)",
                    PhongHoc = "Phòng D.202",
                    SoLuongToiDa = 20,
                    HocPhi = 4500000m,
                    TrangThai = "Đang tuyển sinh"
                },
                new LopHoc
                {
                    TenLop = "ENG-COMM-23A",
                    MaKhoaHoc = khoaHocs[1].MaKhoaHoc,
                    NgayKhaiGiang = DateTime.Today.AddDays(-20),
                    LichHoc = "Tối 2-4-6 (18:00 - 19:30)",
                    PhongHoc = "Phòng B.101",
                    SoLuongToiDa = 25,
                    HocPhi = 3800000m,
                    TrangThai = "Đang học"
                }
            };
            context.LopHocs.AddRange(lopHocs);
            context.SaveChanges();

            // 5. NẠP ĐĂNG KÝ HỌC (DangKyHoc)
            var dangKys = new List<DangKyHoc>
            {
                new DangKyHoc
                {
                    MaHocVien = hocViens[0].MaHocVien,
                    MaLop = lopHocs[0].MaLop,
                    NgayDangKy = DateTime.Now.AddDays(-2),
                    HocPhi = lopHocs[0].HocPhi,
                    TrangThai = "Đã xác nhận"
                },
                new DangKyHoc
                {
                    MaHocVien = hocViens[1].MaHocVien,
                    MaLop = lopHocs[0].MaLop,
                    NgayDangKy = DateTime.Now.AddDays(-1),
                    HocPhi = lopHocs[0].HocPhi,
                    TrangThai = "Chờ xác nhận"
                },
                new DangKyHoc
                {
                    MaHocVien = hocViens[2].MaHocVien,
                    MaLop = lopHocs[2].MaLop,
                    NgayDangKy = DateTime.Now.AddDays(-3),
                    HocPhi = lopHocs[2].HocPhi,
                    TrangThai = "Đã xác nhận"
                },
                new DangKyHoc
                {
                    MaHocVien = hocViens[3].MaHocVien,
                    MaLop = lopHocs[3].MaLop,
                    NgayDangKy = DateTime.Now.AddHours(-10),
                    HocPhi = lopHocs[3].HocPhi,
                    TrangThai = "Chờ xác nhận"
                },
                new DangKyHoc
                {
                    MaHocVien = hocViens[4].MaHocVien,
                    MaLop = lopHocs[4].MaLop,
                    NgayDangKy = DateTime.Now.AddDays(-5),
                    HocPhi = lopHocs[4].HocPhi,
                    TrangThai = "Đã hủy"
                }
            };
            context.DangKyHocs.AddRange(dangKys);
            context.SaveChanges();

            SeedCommentsIfEmpty(context);
            SeedTeachersAndMessagesIfEmpty(context);
            SeedGroupsIfEmpty(context);
            SeedPostsAndConfigIfEmpty(context);
            SeedLuyenThiIfEmpty(context);
        }

        private static void SeedCommentsIfEmpty(TrungTamDbContext context)
        {
            try
            {
                if (!context.BinhLuans.Any())
                {
                    var adminUser = context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin");
                    var students = context.TaiKhoans.Where(t => t.VaiTro == "HocVien").Take(5).ToList();
                    var courses = context.KhoaHocs.Take(4).ToList();

                    if (students.Count > 0 && courses.Count > 0)
                    {
                        var sampleComments = new List<BinhLuan>();

                        // Comment 1: Học viên 1 hỏi về khóa học 1
                        sampleComments.Add(new BinhLuan
                        {
                            NoiDung = "Khóa học này lộ trình rất rõ ràng và bài bản, em học thử buổi đầu thấy thầy cô phát âm cực chuẩn ạ!",
                            NgayBinhLuan = DateTime.Now.AddHours(-18),
                            MaTaiKhoan = students[0].MaTaiKhoan,
                            MaKhoaHoc = courses[0].MaKhoaHoc
                        });

                        // Comment 2: Admin giải đáp
                        if (adminUser != null)
                        {
                            sampleComments.Add(new BinhLuan
                            {
                                NoiDung = "Chào bạn, trung tâm luôn hỗ trợ tài liệu độc quyền và bài thi thử mock-test miễn phí hàng tuần nhé!",
                                NgayBinhLuan = DateTime.Now.AddHours(-16),
                                MaTaiKhoan = adminUser.MaTaiKhoan,
                                MaKhoaHoc = courses[0].MaKhoaHoc
                            });
                        }

                        // Comment 3: Học viên 2 hỏi về lịch học khóa 2
                        if (students.Count >= 2 && courses.Count >= 2)
                        {
                            sampleComments.Add(new BinhLuan
                            {
                                NoiDung = "Lớp học buổi tối có còn suất không ạ? Mình muốn đăng ký theo nhóm 2 người để nhận ưu đãi.",
                                NgayBinhLuan = DateTime.Now.AddHours(-10),
                                MaTaiKhoan = students[1].MaTaiKhoan,
                                MaKhoaHoc = courses[1].MaKhoaHoc
                            });
                        }

                        // Comment 4: Học viên 3 đánh giá
                        if (students.Count >= 3)
                        {
                            sampleComments.Add(new BinhLuan
                            {
                                NoiDung = "Giáo trình rất sát với format đề thi thật. Mình vừa thi đạt mục tiêu tháng trước, cảm ơn trung tâm nhiều!",
                                NgayBinhLuan = DateTime.Now.AddHours(-6),
                                MaTaiKhoan = students[2].MaTaiKhoan,
                                MaKhoaHoc = courses[0].MaKhoaHoc
                            });
                        }

                        // Comment 5: Học viên 1 bình luận thêm ở khóa 2
                        if (courses.Count >= 2)
                        {
                            sampleComments.Add(new BinhLuan
                            {
                                NoiDung = "Phòng học hiện đại, có điều hòa và máy chiếu xịn xò lắm các bạn ơi.",
                                NgayBinhLuan = DateTime.Now.AddHours(-2),
                                MaTaiKhoan = students[0].MaTaiKhoan,
                                MaKhoaHoc = courses[1].MaKhoaHoc
                            });
                        }

                        // Comment 6: Học viên 4 hỏi về học phí
                        if (students.Count >= 4 && courses.Count >= 3)
                        {
                            sampleComments.Add(new BinhLuan
                            {
                                NoiDung = "Có chính sách chia đợt đóng học phí không ạ?",
                                NgayBinhLuan = DateTime.Now.AddMinutes(-45),
                                MaTaiKhoan = students[3].MaTaiKhoan,
                                MaKhoaHoc = courses[2].MaKhoaHoc
                            });
                        }

                        context.BinhLuans.AddRange(sampleComments);
                        context.SaveChanges();
                    }
                }
            }
            catch { }
        }

        private static void SeedTeachersAndMessagesIfEmpty(TrungTamDbContext context)
        {
            try
            {
                if (!context.TaiKhoans.Any(t => t.VaiTro == "GiaoVien"))
                {
                    var gv1Account = new TaiKhoan
                    {
                        TenDangNhap = "giaovien1",
                        MatKhau = "123456",
                        HoTen = "Cô Mai Anh",
                        Email = "maianh.ielts@educenter.edu.vn",
                        VaiTro = "GiaoVien",
                        TrangThai = "HoatDong"
                    };
                    var gv2Account = new TaiKhoan
                    {
                        TenDangNhap = "giaovien2",
                        MatKhau = "123456",
                        HoTen = "Thầy Kenji Sato",
                        Email = "kenji.sato@educenter.edu.vn",
                        VaiTro = "GiaoVien",
                        TrangThai = "HoatDong"
                    };
                    var gv3Account = new TaiKhoan
                    {
                        TenDangNhap = "giaovien3",
                        MatKhau = "123456",
                        HoTen = "Cô Thu Trang",
                        Email = "thutrang.chinese@educenter.edu.vn",
                        VaiTro = "GiaoVien",
                        TrangThai = "HoatDong"
                    };

                    context.TaiKhoans.AddRange(gv1Account, gv2Account, gv3Account);
                    context.SaveChanges();

                    var gv1 = new GiaoVien
                    {
                        MaTaiKhoan = gv1Account.MaTaiKhoan,
                        HoTen = "Cô Mai Anh",
                        ChuyenMon = "Tiếng Anh (IELTS)",
                        HocVi = "Thạc sĩ TESOL / IELTS 8.5",
                        SoDienThoai = "0987654321",
                        Email = "maianh.ielts@educenter.edu.vn",
                        GioiThieu = "Kinh nghiệm 8 năm giảng dạy luyện thi IELTS chuyên sâu Speaking & Writing tại các trung tâm quốc tế.",
                        AnhDaiDien = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80"
                    };

                    var gv2 = new GiaoVien
                    {
                        MaTaiKhoan = gv2Account.MaTaiKhoan,
                        HoTen = "Thầy Kenji Sato",
                        ChuyenMon = "Tiếng Nhật (JLPT)",
                        HocVi = "Cử nhân ĐH Tokyo / JLPT N1",
                        SoDienThoai = "0987654322",
                        Email = "kenji.sato@educenter.edu.vn",
                        GioiThieu = "Giảng viên bản ngữ với phương pháp phản xạ thực tế, giúp học viên đỗ JLPT N3 - N1 nhanh chóng.",
                        AnhDaiDien = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80"
                    };

                    var gv3 = new GiaoVien
                    {
                        MaTaiKhoan = gv3Account.MaTaiKhoan,
                        HoTen = "Cô Thu Trang",
                        ChuyenMon = "Tiếng Trung (HSK)",
                        HocVi = "Thạc sĩ Ngôn ngữ / HSK 6",
                        SoDienThoai = "0987654323",
                        Email = "thutrang.chinese@educenter.edu.vn",
                        GioiThieu = "Tốt nghiệp ĐH Ngoại ngữ Bắc Kinh, chuyên luyện thi HSK 4-6 và khẩu ngữ thương mại.",
                        AnhDaiDien = "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&auto=format&fit=crop&q=80"
                    };

                    context.GiaoViens.AddRange(gv1, gv2, gv3);
                    context.SaveChanges();

                    // Gán lịch dạy mẫu cho các lớp học hiện có
                    var classes = context.LopHocs.ToList();
                    if (classes.Any())
                    {
                        for (int i = 0; i < classes.Count; i++)
                        {
                            if (i % 3 == 0) classes[i].MaGiaoVien = gv1.MaGiaoVien;
                            else if (i % 3 == 1) classes[i].MaGiaoVien = gv2.MaGiaoVien;
                            else classes[i].MaGiaoVien = gv3.MaGiaoVien;
                        }
                        context.SaveChanges();
                    }

                    // Tạo tin nhắn mẫu giữa học viên 1 và cô Mai Anh
                    var hv1 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "hocvien1");
                    if (hv1 != null)
                    {
                        context.TinNhans.AddRange(
                            new TinNhan
                            {
                                NguoiGuiId = gv1Account.MaTaiKhoan,
                                NguoiNhanId = hv1.MaTaiKhoan,
                                NoiDung = "Chào em! Cô là Mai Anh phụ trách lớp IELTS. Em có thắc mắc gì về tài liệu bài giảng tuần này không?",
                                ThoiGian = DateTime.Now.AddHours(-3),
                                DaXem = true
                            },
                            new TinNhan
                            {
                                NguoiGuiId = hv1.MaTaiKhoan,
                                NguoiNhanId = gv1Account.MaTaiKhoan,
                                NoiDung = "Dạ chào cô ạ! Cô cho em hỏi thêm về bài tập viết Task 2 trang 45 với ạ.",
                                ThoiGian = DateTime.Now.AddHours(-2),
                                DaXem = true
                            },
                            new TinNhan
                            {
                                NguoiGuiId = gv1Account.MaTaiKhoan,
                                NguoiNhanId = hv1.MaTaiKhoan,
                                NoiDung = "Được em nhé! Em chú ý triển khai luận điểm 2 ý chính rõ ràng và dùng collocations cô đã phát nhé 👍",
                                ThoiGian = DateTime.Now.AddMinutes(-30),
                                DaXem = false
                            }
                        );
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error seeding teachers: " + ex.Message);
            }
        }

        private static void SeedGroupsIfEmpty(TrungTamDbContext context)
        {
            try
            {
                var gv1 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "giaovien1");
                var admin = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "admin");
                var students = context.TaiKhoans.Where(t => t.VaiTro == "HocVien").Take(3).ToList();

                if (gv1 == null) return;

                var nhom1 = context.NhomChats.FirstOrDefault();
                if (nhom1 == null)
                {
                    nhom1 = new NhomChat
                    {
                        TenNhom = "Lớp IELTS Intensive 6.5 - K37",
                        AnhNhom = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=150&auto=format&fit=crop&q=80",
                        NguoiTaoId = gv1.MaTaiKhoan,
                        NgayTao = DateTime.Now.AddDays(-5),
                        CuocGoiDangDienRa = false
                    };
                    context.NhomChats.Add(nhom1);
                    context.SaveChanges();
                }

                // Kiểm tra xem nhóm đã có thành viên chưa
                if (!context.ThanhVienNhoms.Any(tv => tv.MaNhom == nhom1.MaNhom))
                {
                    context.ThanhVienNhoms.Add(new ThanhVienNhom { MaNhom = nhom1.MaNhom, MaTaiKhoan = gv1.MaTaiKhoan, VaiTroTrongNhom = "TruongNhom" });
                    if (admin != null)
                    {
                        context.ThanhVienNhoms.Add(new ThanhVienNhom { MaNhom = nhom1.MaNhom, MaTaiKhoan = admin.MaTaiKhoan, VaiTroTrongNhom = "QuanTriVien" });
                    }
                    foreach (var st in students)
                    {
                        context.ThanhVienNhoms.Add(new ThanhVienNhom { MaNhom = nhom1.MaNhom, MaTaiKhoan = st.MaTaiKhoan, VaiTroTrongNhom = "ThanhVien" });
                    }
                    context.SaveChanges();
                }

                // Kiểm tra tin nhắn nhóm
                if (!context.TinNhans.Any(m => m.MaNhom == nhom1.MaNhom))
                {
                    context.TinNhans.AddRange(
                        new TinNhan
                        {
                            NguoiGuiId = gv1.MaTaiKhoan,
                            MaNhom = nhom1.MaNhom,
                            LoaiTinNhan = "HeThong",
                            NoiDung = "Cô Mai Anh đã tạo nhóm chat 'Lớp IELTS Intensive 6.5 - K37'",
                            ThoiGian = DateTime.Now.AddDays(-5)
                        },
                        new TinNhan
                        {
                            NguoiGuiId = gv1.MaTaiKhoan,
                            MaNhom = nhom1.MaNhom,
                            LoaiTinNhan = "VanBan",
                            NoiDung = "Chào cả lớp! Đây là nhóm trao đổi bài tập và thông báo lịch học của lớp chúng ta nhé. Các bạn có câu hỏi gì cứ nhắn vào đây ạ 👍",
                            ThoiGian = DateTime.Now.AddDays(-5).AddMinutes(10)
                        }
                    );
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error seeding groups: " + ex.Message);
            }
        }

        private static void SeedPostsAndConfigIfEmpty(TrungTamDbContext context)
        {
            try
            {
                // 1. Cấu hình hệ thống mặc định (Bật/Tắt xét duyệt bài đăng)
                var config = context.CauHinhHeThongs.FirstOrDefault(c => c.Khoa == "BatXetDuyetBaiViet");
                if (config == null)
                {
                    context.CauHinhHeThongs.Add(new CauHinhHeThong
                    {
                        Khoa = "BatXetDuyetBaiViet",
                        GiaTri = "false", // Mặc định không cần duyệt để bài hiển thị ngay, quản lý có thể bật/tắt bất kỳ lúc nào
                        MoTa = "Bật/Tắt chế độ kiểm duyệt bài viết của học viên và giáo viên trước khi hiển thị công khai",
                        NgayCapNhat = DateTime.Now
                    });
                    context.SaveChanges();
                }

                // 2. Bài viết mẫu nếu chưa có
                if (!context.BaiViets.Any())
                {
                    var gv1 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "giaovien1");
                    var gv2 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "giaovien2");
                    var hv1 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "hocvien1");

                    if (gv1 != null)
                    {
                        context.BaiViets.Add(new BaiViet
                        {
                            MaTaiKhoan = gv1.MaTaiKhoan,
                            NoiDung = "🎉 Chào mừng các bạn học viên mới đến với kỳ học mùa Thu 2026 tại **LanguageBook Academy**!\n\nTuần này các lớp **IELTS Intensive 6.5+** và **Tiếng Anh Giao Tiếp Thực Chiến** sẽ bắt đầu buổi học đầu tiên. Chúc cả nhà một tuần học tập tràn đầy năng lượng và bứt phá mục tiêu nhé! 📚💪",
                            HinhAnhUrl = "/images/anhtienganh/03189ca0b0afb55ec9642fe00fa5b885_1628568387.jpg",
                            PhongNen = null,
                            NgayDang = DateTime.Now.AddHours(-3),
                            TrangThai = "DaDuyet"
                        });
                    }

                    if (hv1 != null)
                    {
                        context.BaiViets.Add(new BaiViet
                        {
                            MaTaiKhoan = hv1.MaTaiKhoan,
                            NoiDung = "Hôm nay mình vừa nhận kết quả thi thử **IELTS 7.0 Overall** sau khóa học 3 tháng! Cảm ơn **cô Mai Anh** và trung tâm LanguageBook rất nhiều ạ ❤️",
                            HinhAnhUrl = null,
                            PhongNen = "sunset",
                            NgayDang = DateTime.Now.AddHours(-1),
                            TrangThai = "DaDuyet"
                        });
                    }

                    if (gv2 != null)
                    {
                        context.BaiViets.Add(new BaiViet
                        {
                            MaTaiKhoan = gv2.MaTaiKhoan,
                            NoiDung = "📢 Thông báo lịch thi thử **JLPT N3 - N2** định kỳ tháng tới!\n\nCác bạn học viên lớp tiếng Nhật chú ý ôn tập kỹ phần **Từ vựng Kanji & Ngữ pháp Dokkai** nhé. Bạn nào cần thêm tài liệu luyện đề cứ inbox trực tiếp cho Sensei nha! 🇯🇵🎌",
                            HinhAnhUrl = "/images/anhtienganh/tiengtrung3.jpg",
                            PhongNen = null,
                            NgayDang = DateTime.Now.AddMinutes(-40),
                            TrangThai = "DaDuyet"
                        });
                    }

                    context.SaveChanges();
                }

                // 3. Seed lượt thích và bình luận mẫu nếu chưa có
                var samplePost = context.BaiViets.FirstOrDefault();
                if (samplePost != null && !context.ThichBaiViets.Any(t => t.MaBaiViet == samplePost.MaBaiViet))
                {
                    var sampleUsers = context.TaiKhoans.Take(3).ToList();
                    foreach (var u in sampleUsers)
                    {
                        context.ThichBaiViets.Add(new ThichBaiViet
                        {
                            MaBaiViet = samplePost.MaBaiViet,
                            MaTaiKhoan = u.MaTaiKhoan,
                            NgayThich = DateTime.Now.AddMinutes(-20)
                        });
                    }

                    if (!context.BinhLuans.Any(b => b.MaBaiViet == samplePost.MaBaiViet))
                    {
                        var commentUser = sampleUsers.Last();
                        context.BinhLuans.Add(new BinhLuan
                        {
                            MaBaiViet = samplePost.MaBaiViet,
                            MaTaiKhoan = commentUser.MaTaiKhoan,
                            NoiDung = "Tuyệt vời quá ạ! Em rất háo hức chờ buổi học đầu tiên tuần này 🎉📚",
                            NgayBinhLuan = DateTime.Now.AddMinutes(-15)
                        });
                    }

                    samplePost.SoChiaSe = 2;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error seeding posts: " + ex.Message);
            }

            // Tự động chuẩn hóa và làm sạch dữ liệu Tiếng Việt có dấu chuẩn đẹp
            SanitizeAndFixVietnameseData(context);
        }

        public static void SanitizeAndFixVietnameseData(TrungTamDbContext context)
        {
            try
            {
                // 1. Sửa trạng thái DangKyHoc
                var allDk = context.DangKyHocs.ToList();
                foreach (var dk in allDk)
                {
                    if (dk.TrangThai.Contains("Ä") || dk.TrangThai == "Da xac nhan" || dk.TrangThai == "Đã xác nhận")
                    {
                        dk.TrangThai = "Đã xác nhận";
                    }
                    else if (dk.TrangThai.Contains("Cho") || dk.TrangThai == "Chờ xác nhận")
                    {
                        dk.TrangThai = "Chờ xác nhận";
                    }
                    else if (dk.TrangThai.Contains("huy") || dk.TrangThai.Contains("Huy") || dk.TrangThai == "Đã hủy")
                    {
                        dk.TrangThai = "Đã hủy";
                    }
                }

                // 2. Sửa trạng thái và thông tin LopHoc
                var allLop = context.LopHocs.ToList();
                foreach (var lop in allLop)
                {
                    if (lop.TrangThai.Contains("Ä") || lop.TrangThai == "Dang hoc" || lop.TrangThai == "Đang học")
                    {
                        lop.TrangThai = "Đang học";
                    }
                    else if (lop.TrangThai.Contains("tuyen sinh") || lop.TrangThai == "Đang tuyển sinh")
                    {
                        lop.TrangThai = "Đang tuyển sinh";
                    }

                    // Sửa lịch học mojibake hoặc không dấu
                    if (lop.LichHoc.Contains("Toi 2-4-6") || lop.LichHoc.Contains("Tá»‘i 2-4-6")) lop.LichHoc = "Tối 2-4-6 (18:00 - 19:30)";
                    else if (lop.LichHoc.Contains("Toi 3-5-7") || lop.LichHoc.Contains("Tá»‘i 3-5-7")) lop.LichHoc = "Tối 3-5-7 (18:00 - 19:30)";
                    else if (lop.LichHoc.Contains("Sang 2-4-6") || lop.LichHoc.Contains("SÃ¡ng 2-4-6")) lop.LichHoc = "Sáng 2-4-6 (08:00 - 09:30)";
                    else if (lop.LichHoc.Contains("Sang 3-5-7") || lop.LichHoc.Contains("SÃ¡ng 3-5-7")) lop.LichHoc = "Sáng 3-5-7 (08:00 - 09:30)";
                    else if (lop.LichHoc.Contains("Chieu 2-4-6") || lop.LichHoc.Contains("Chiá» u 2-4-6")) lop.LichHoc = "Chiều 2-4-6 (14:00 - 15:30)";
                    else if (lop.LichHoc.Contains("Chieu 3-5-7") || lop.LichHoc.Contains("Chiá» u 3-5-7")) lop.LichHoc = "Chiều 3-5-7 (14:00 - 15:30)";

                    // Sửa phòng học
                    if (lop.PhongHoc.Contains("Phong A.101") || lop.PhongHoc.Contains("PhÃ²ng A.101")) lop.PhongHoc = "Phòng A.101";
                    else if (lop.PhongHoc.Contains("Phong A.102") || lop.PhongHoc.Contains("PhÃ²ng A.102")) lop.PhongHoc = "Phòng A.102";
                    else if (lop.PhongHoc.Contains("Phong B.201") || lop.PhongHoc.Contains("PhÃ²ng B.201")) lop.PhongHoc = "Phòng B.201";
                    else if (lop.PhongHoc.Contains("Phong B.202") || lop.PhongHoc.Contains("PhÃ²ng B.202")) lop.PhongHoc = "Phòng B.202";
                    else if (lop.PhongHoc.Contains("Phong C.301") || lop.PhongHoc.Contains("PhÃ²ng C.301")) lop.PhongHoc = "Phòng C.301";
                    else if (lop.PhongHoc.Contains("Phong C.302") || lop.PhongHoc.Contains("PhÃ²ng C.302")) lop.PhongHoc = "Phòng C.302";
                }

                // 3. Sửa KhoaHoc
                var courseDict = new Dictionary<string, (string Ten, string NgoaiNgu, string CapDo, string MoTa)>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Tieng Anh Giao Tiep Co Ban", ("Tiếng Anh Giao Tiếp Cơ Bản", "Tiếng Anh", "Sơ cấp (A1/A2)", "Khóa học tiếng Anh giao tiếp chuẩn quốc tế cho người mới bắt đầu, rèn luyện phát âm và phản xạ tự nhiên.") },
                    { "Tieng Anh Giao Tiep Nang Cao", ("Tiếng Anh Giao Tiếp Nâng Cao", "Tiếng Anh", "Trung cấp (B1/B2)", "Nâng cao phản xạ giao tiếp tự tin, thuyết trình và đàm phán trong môi trường học tập & làm việc quốc tế.") },
                    { "Luyen Thi IELTS 6.0+", ("Luyện Thi IELTS 6.0+", "Tiếng Anh", "IELTS", "Luyện thi chuyên sâu 4 kỹ năng Nghe - Nói - Đọc - Viết, cam kết đầu ra IELTS 6.0+.") },
                    { "Luyen Thi IELTS 7.0+", ("Luyện Thi IELTS 7.0+", "Tiếng Anh", "IELTS", "Khóa luyện thi IELTS học thuật nâng cao, chiến thuật làm bài chinh phục band điểm 7.0 - 8.0+.") },
                    { "Tieng Anh TOEFL", ("Tiếng Anh Học Thuật TOEFL", "Tiếng Anh", "TOEFL", "Chuẩn bị toàn diện cho kỳ thi TOEFL iBT quốc tế phục vụ săn học bổng và du học.") },
                    { "Tieng Anh Business", ("Tiếng Anh Thương Mại Quốc Tế", "Tiếng Anh", "Trung cấp (B1/B2)", "Kỹ năng viết email chuyên nghiệp, thuyết trình kinh doanh và giao dịch thương mại bằng tiếng Anh.") },
                    { "Tieng Nhat So Cap N5", ("Tiếng Nhật Sơ Cấp JLPT N5", "Tiếng Nhật", "N5", "Nhập môn tiếng Nhật từ bảng chữ cái Hiragana/Katakana đến Kanji cơ bản, cam kết thi đỗ JLPT N5.") },
                    { "Tieng Nhat Trung Cap N4", ("Tiếng Nhật Trung Cấp JLPT N4", "Tiếng Nhật", "N4", "Mở rộng vốn từ vựng, ngữ pháp ứng dụng và luyện thi chắc chắn chứng chỉ JLPT N4.") },
                    { "Tieng Nhat Cao Cap N3", ("Tiếng Nhật Cao Cấp JLPT N3", "Tiếng Nhật", "N3", "Giao tiếp tiếng Nhật lưu loát trong công việc, luyện thi cấp tốc đạt JLPT N3.") },
                    { "Tieng Nhat Business N2", ("Tiếng Nhật Thương Mại JLPT N2", "Tiếng Nhật", "N2", "Khóa tiếng Nhật chuyên nghiệp cho doanh nghiệp Nhật Bản và luyện thi JLPT N2.") },
                    { "Tieng Han Co Ban TOPIK I", ("Tiếng Hàn Cơ Bản TOPIK I", "Tiếng Hàn", "TOPIK I", "Học tiếng Hàn từ bảng chữ cái Hangeul, giao tiếp đời sống và thi đỗ chứng chỉ TOPIK I.") },
                    { "Tieng Han Trung Cap TOPIK II", ("Tiếng Hàn Trung Cấp TOPIK II", "Tiếng Hàn", "TOPIK II", "Nâng cao ngữ pháp, đọc hiểu và kỹ năng viết luận thi TOPIK II cấp độ 3 - 4.") },
                    { "Tieng Han Du Hoc - Lam Viec", ("Tiếng Hàn Du Học & Định Cư", "Tiếng Hàn", "Cao cấp (C1/C2)", "Chương trình chuyên biệt cho học viên chuẩn bị du học đại học và làm việc tại Hàn Quốc.") },
                    { "Tieng Trung Co Ban HSK1-2", ("Tiếng Trung Cơ Bản HSK 1 - 2", "Tiếng Trung", "HSK 1-2", "Nhập môn phát âm Pinyin, quy tắc viết chữ Hán và giao tiếp cơ bản, đạt HSK 1-2.") },
                    { "Tieng Trung Giao Tiep HSK3", ("Tiếng Trung Giao Tiếp HSK 3", "Tiếng Trung", "HSK 3", "Rèn luyện khẩu ngữ giao tiếp thực tế và luyện đề thi chứng chỉ quốc tế HSK 3.") },
                    { "Tieng Trung Thuong Mai HSK4", ("Tiếng Trung Thương Mại HSK 4", "Tiếng Trung", "HSK 4", "Tiếng Trung thương mại, đàm phán hợp đồng, xuất nhập khẩu và luyện thi HSK 4.") },
                    { "Tieng Trung Nang Cao HSK5", ("Tiếng Trung Nâng Cao HSK 5", "Tiếng Trung", "HSK 5", "Khóa chuyên sâu từ vựng cao cấp, đọc báo chí và chinh phục chứng chỉ HSK 5.") },
                    { "Tieng Phap Co Ban A1-A2", ("Tiếng Pháp Nhập Môn DELF A1 - A2", "Tiếng Pháp", "A1-A2", "Khám phá ngôn ngữ Pháp lãng mạn, phát âm chuẩn Paris và đạt DELF A1 - A2.") },
                    { "Tieng Phap Giao Tiep B1", ("Tiếng Pháp Giao Tiếp DELF B1", "Tiếng Pháp", "B1", "Giao tiếp tiếng Pháp tự nhiên, phản xạ linh hoạt và luyện thi chứng chỉ DELF B1.") },
                    { "Tieng Phap Nang Cao DALF", ("Tiếng Pháp Nâng Cao DALF C1", "Tiếng Pháp", "B2-C1", "Khóa học học thuật chuyên sâu phục vụ du học Pháp và định cư các nước Pháp ngữ.") },
                    { "Tieng Anh Cho Tre Em 6-10 Tuoi", ("Tiếng Anh Thiếu Nhi (6 - 10 Tuổi)", "Tiếng Anh", "Thiếu nhi", "Phương pháp học tiếng Anh qua bài hát, trò chơi tương tác sinh động giúp trẻ yêu thích ngôn ngữ.") },
                    { "Tieng Anh Cho Tre Em 10-14 Tuoi", ("Tiếng Anh Thiếu Niên (10 - 14 Tuổi)", "Tiếng Anh", "Thiếu nhi", "Củng cố ngữ pháp nền tảng, mở rộng vốn từ vựng và tự tin giao tiếp tiếng Anh.") },
                    { "Tieng Nhat Hoi Thoai So Cap", ("Tiếng Nhật Hội Thoại & Văn Hóa", "Tiếng Nhật", "Sơ cấp (A1/A2)", "Khóa học đàm thoại thực tế khi du lịch, làm việc và tìm hiểu văn hóa đất nước mặt trời mọc.") },
                    { "Tieng Han Kpop va Van Hoa", ("Tiếng Hàn K-Pop & Giao Tiếp", "Tiếng Hàn", "Sơ cấp (A1/A2)", "Học tiếng Hàn thú vị qua âm nhạc K-Pop, phim ảnh và các tình huống đời sống giới trẻ.") },
                    { "Tieng Trung Hoi Thoai Cap Toc", ("Tiếng Trung Hội Thoại Cấp Tốc", "Tiếng Trung", "Sơ cấp (A1/A2)", "Khóa giao tiếp thực dụng cấp tốc trong 2 tháng dành cho người bận rộn.") },
                    { "Tieng Anh Online Live", ("Tiếng Anh Trực Tuyến Tương Tác 1-1", "Tiếng Anh", "Trung cấp (B1/B2)", "Học tiếng Anh online tương tác trực tiếp với giáo viên bản ngữ mọi lúc mọi nơi.") },
                    { "Tieng Nhat Luyen Thi JLPT N1", ("Tiếng Nhật Chuyên Sâu JLPT N1", "Tiếng Nhật", "N1", "Khóa luyện thi đỉnh cao JLPT N1 với đội ngũ giảng viên chuyên gia hàng đầu.") },
                    { "Tieng Han TOPIK II Cap Cao", ("Tiếng Hàn Nâng Cao TOPIK II Cấp 5-6", "Tiếng Hàn", "TOPIK II", "Luyện thi chuyên sâu TOPIK II cấp độ 5 và 6 phục vụ công việc biên - phiên dịch.") },
                    { "Tieng Anh TOEIC 600+", ("Luyện Thi TOEIC Cấp Tốc 600+", "Tiếng Anh", "TOEIC", "Chiến lược giải đề TOEIC Listening & Reading, cam kết đạt 600+ điểm nhanh nhất.") },
                    { "Tieng Anh TOEIC 750+", ("Luyện Thi TOEIC Nâng Cao 750+", "Tiếng Anh", "TOEIC", "Mục tiêu 750+ TOEIC, mở rộng cơ hội việc làm tại các tập đoàn đa quốc gia.") }
                };

                var allKhoaHoc = context.KhoaHocs.ToList();
                foreach (var kh in allKhoaHoc)
                {
                    kh.TrangThai = "Đang mở";
                    if (courseDict.TryGetValue(kh.TenKhoaHoc.Trim(), out var info))
                    {
                        kh.TenKhoaHoc = info.Ten;
                        kh.NgoaiNgu = info.NgoaiNgu;
                        kh.CapDo = info.CapDo;
                        kh.MoTa = info.MoTa;
                    }
                }

                // Cập nhật lại tên các lớp học theo tên khóa học có dấu
                foreach (var lop in allLop)
                {
                    var parentKh = allKhoaHoc.FirstOrDefault(k => k.MaKhoaHoc == lop.MaKhoaHoc);
                    if (parentKh != null)
                    {
                        var parts = lop.TenLop.Split(' ');
                        var lastPart = parts.LastOrDefault() ?? "";
                        if (lastPart.StartsWith("K") || lastPart.StartsWith("k"))
                        {
                            lop.TenLop = parentKh.TenKhoaHoc + " - " + lastPart.ToUpper();
                        }
                        else
                        {
                            lop.TenLop = parentKh.TenKhoaHoc + " - Lớp " + lop.MaLop;
                        }
                    }
                }

                // 4. Sửa Giáo viên (GiaoVien & TaiKhoan)
                var teacherData = new (string Ten, string ChuyenMon, string HocVi, string GioiThieu)[]
                {
                    ("Thầy Nguyễn Bảo Trân", "Tiếng Anh", "Thạc sĩ Giảng dạy Tiếng Anh (TESOL, ĐH Queensland)", "Hơn 10 năm kinh nghiệm luyện thi IELTS 8.5, phương pháp truyền cảm hứng và nhiệt huyết."),
                    ("Cô Phạm Thanh Tùng", "Tiếng Nhật", "Cử nhân Sư phạm Tiếng Nhật (ĐH Ngoại Ngữ)", "JLPT N1 xuất sắc, nhiều năm tu nghiệp tại Tokyo, giảng dạy tận tình và chu đáo."),
                    ("Cô Lê Thị Phương", "Tiếng Hàn", "Tiến sĩ Ngôn ngữ & Văn hóa Hàn Quốc (ĐH Quốc gia Seoul)", "TOPIK 6, chuyên gia đào tạo biên phiên dịch và luyện thi TOPIK II cao cấp."),
                    ("Thầy Trần Minh Đạt", "Tiếng Trung", "Thạc sĩ Hán ngữ Quốc tế (ĐH Bắc Kinh)", "HSK 6, HSKK Cao cấp, phương pháp học chữ Hán qua chiết tự độc đáo và dễ nhớ."),
                    ("Cô Hoàng Thu Hà", "Tiếng Pháp", "Thạc sĩ Ngôn ngữ Pháp (ĐH Sorbonne, Paris)", "DALF C2, chuyên gia luyện thi DELF/DALF và tư vấn hồ sơ du học Pháp."),
                    ("Thầy Vũ Đình Trọng", "Tiếng Anh", "Thạc sĩ Ngôn ngữ học Ứng dụng (ĐH Melbourne)", "TOEIC 990/990, giảng viên đào tạo kỹ năng giao tiếp doanh nghiệp hàng đầu."),
                    ("Cô Đặng Quỳnh Nga", "Tiếng Nhật", "Cử nhân Biên - Phiên dịch Tiếng Nhật", "JLPT N1, cựu thông dịch viên cấp cao, phong cách giảng dạy năng động."),
                    ("Thầy Bùi Quốc Huy", "Tiếng Hàn", "Thạc sĩ Giáo dục Tiếng Hàn (ĐH Yonsei)", "Nhiệt tình, am hiểu sâu sắc văn hóa và môi trường làm việc doanh nghiệp Hàn Quốc."),
                    ("Cô Nguyễn Mai Anh", "Tiếng Trung", "Cử nhân Sư phạm Tiếng Trung (ĐH Sư phạm)", "HSK 6, chuyên gia luyện thi HSK cấp tốc và giao tiếp thương mại thực tế."),
                    ("Thầy Đỗ Quang Khải", "Tiếng Anh", "Thạc sĩ TESOL (ĐH Huddersfield, Anh Quốc)", "IELTS 8.5, chuyên gia xây dựng lộ trình học cá nhân hóa cho học viên mất gốc."),
                    ("Cô Trịnh Phương Linh", "Tiếng Nhật", "Cử nhân Sư phạm Ngoại ngữ", "Nhiều năm kinh nghiệm giảng dạy tiếng Nhật trẻ em và hội thoại đời sống."),
                    ("Thầy Ngô Đức Thắng", "Tiếng Hàn", "Cử nhân Ngôn ngữ Hàn Quốc", "Phương pháp dạy tiếng Hàn qua phim ảnh và âm nhạc sinh động, thu hút học viên."),
                    ("Cô Phan Diệu My", "Tiếng Trung", "Thạc sĩ Ngôn ngữ học", "Tận tâm, luôn đồng hành sát sao cùng từng học viên trong suốt khóa học."),
                    ("Thầy Lương Thế Vinh", "Tiếng Pháp", "Cử nhân Sư phạm Tiếng Pháp", "DELF B2/C1, phương pháp đàm thoại phản xạ tự nhiên chuẩn giọng Pháp."),
                    ("Cô Dương Hải Yến", "Tiếng Anh", "Cử nhân Sư phạm Tiếng Anh", "Chuyên luyện phát âm chuẩn IPA và giao tiếp tự tin cho người mới bắt đầu.")
                };

                // Cập nhật 15 giáo viên gv01 -> gv15
                for (int i = 1; i <= 15; i++)
                {
                    string username = "gv" + i.ToString("D2");
                    var tk = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == username);
                    if (tk != null)
                    {
                        var data = teacherData[(i - 1) % teacherData.Length];
                        tk.HoTen = data.Ten;

                        var gv = context.GiaoViens.FirstOrDefault(g => g.MaTaiKhoan == tk.MaTaiKhoan);
                        if (gv != null)
                        {
                            gv.HoTen = data.Ten;
                            gv.ChuyenMon = data.ChuyenMon;
                            gv.HocVi = data.HocVi;
                            gv.GioiThieu = data.GioiThieu;
                        }
                    }
                }

                // Dọn sạch các giáo viên cũ bị Mojibake
                var allGv = context.GiaoViens.Include(g => g.TaiKhoan).ToList();
                foreach (var gv in allGv)
                {
                    if (gv.HoTen.Contains("Ä") || gv.HoTen.Contains("Ã") || gv.HoTen.Contains("á»") || gv.HoTen.Contains("áº"))
                    {
                        if (gv.HoTen.Contains("LÃª Minh")) gv.HoTen = "Thầy Lê Minh Đức";
                        else if (gv.HoTen.Contains("HoÃ ng VÄƒn Long")) gv.HoTen = "Thầy Hoàng Văn Long";
                        else if (gv.HoTen.Contains("Nguyá»…n")) gv.HoTen = "Thầy Nguyễn Anh Tuấn";
                        else if (gv.HoTen.Contains("Tráº§n")) gv.HoTen = "Cô Trần Thị Hương";
                        else gv.HoTen = "Thầy Lê Minh Đức";

                        if (gv.TaiKhoan != null) gv.TaiKhoan.HoTen = gv.HoTen;
                    }

                    if (gv.ChuyenMon.Contains("Tiáº¿ng HÃ n") || gv.ChuyenMon == "Tieng Han") gv.ChuyenMon = "Tiếng Hàn";
                    else if (gv.ChuyenMon.Contains("Tiáº¿ng PhÃ¡p") || gv.ChuyenMon == "Tieng Phap") gv.ChuyenMon = "Tiếng Pháp";
                    else if (gv.ChuyenMon.Contains("Tiáº¿ng Anh") || gv.ChuyenMon == "Tieng Anh") gv.ChuyenMon = "Tiếng Anh";
                    else if (gv.ChuyenMon.Contains("Tiáº¿ng Nháº­t") || gv.ChuyenMon == "Tieng Nhat") gv.ChuyenMon = "Tiếng Nhật";
                    else if (gv.ChuyenMon.Contains("Tiáº¿ng Trung") || gv.ChuyenMon == "Tieng Trung") gv.ChuyenMon = "Tiếng Trung";

                    if (gv.HocVi != null && (gv.HocVi.Contains("Ä") || gv.HocVi.Contains("Ã") || gv.HocVi.Contains("Cá»­")))
                    {
                        if (gv.HocVi.Contains("Tiáº¿n sÄ©")) gv.HocVi = "Tiến sĩ Ngôn ngữ học";
                        else if (gv.HocVi.Contains("Tháº¡c sÄ©")) gv.HocVi = "Thạc sĩ Ngôn ngữ học";
                        else gv.HocVi = "Cử nhân Sư phạm Ngoại ngữ";
                    }
                }

                // 5. Sửa Học viên (HocVien & TaiKhoan)
                var studentNames = new[] {
                    "Nguyễn Văn Thành", "Trần Thị Mai", "Lê Hoàng Phúc", "Phạm Minh Khoa", "Hoàng Thị Hoa",
                    "Đặng Văn Nam", "Bùi Thị Thu", "Vũ Quang Huy", "Đỗ Thị Lan", "Ngô Văn Đức",
                    "Trương Minh Trí", "Dương Thu Thảo", "Lý Gia Bảo", "Đinh Hải Yến", "Mai Xuân Trường",
                    "Lâm Ngọc Ánh", "Hồ Hữu Phước", "Phan Thùy Dung", "Võ Tuấn Kiệt", "Tạ Bích Phượng",
                    "Đoàn Thế Anh", "Lưu Cẩm Tú", "Chu Đình Trọng", "Cao Diệu Linh", "Bạch Quốc Bảo",
                    "Lương Thị Nga", "Thái Đăng Khoa", "Diệp Mỹ Duyên", "Kim Nhật Minh", "Khổng Tú Quỳnh",
                    "Quách Tuấn Du", "Ân Vĩnh Phát", "Hà Thị Kiều", "Nghiêm Xuân Tú", "Vương Quốc Cường",
                    "Đào Hồng Nhung", "Tăng Thanh Hà", "Tô Hoài Nam", "Giang Mỹ Linh", "Phùng Ngọc Huy",
                    "Lục Tiểu Vy", "Sơn Tùng", "Mã Quốc Cường", "Thạch Kim Tuấn", "Ôn Bích Hà",
                    "La Quốc Hùng", "Tiêu Châu Như Quỳnh", "Trịnh Thăng Bình", "Châu Khải Phong", "Hứa Vĩ Văn"
                };

                for (int i = 1; i <= 50; i++)
                {
                    string username = "hv" + i.ToString("D2");
                    var tk = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == username);
                    if (tk != null)
                    {
                        string name = studentNames[(i - 1) % studentNames.Length];
                        tk.HoTen = name;

                        var hv = context.HocViens.FirstOrDefault(h => h.MaTaiKhoan == tk.MaTaiKhoan);
                        if (hv != null)
                        {
                            hv.HoTen = name;
                            hv.GioiTinh = (i % 3 == 0) ? "Nữ" : "Nam";
                        }
                    }
                }

                // Sửa các học viên cũ bị Mojibake hoặc dính số
                var allHv = context.HocViens.Include(h => h.TaiKhoan).ToList();
                int fixIdx = 0;
                foreach (var hv in allHv)
                {
                    if (hv.HoTen.Contains("Ä") || hv.HoTen.Contains("Ã") || hv.HoTen.Contains("á»") || hv.HoTen.Contains("áº") || char.IsDigit(hv.HoTen.LastOrDefault()))
                    {
                        string cleanName = studentNames[fixIdx % studentNames.Length];
                        fixIdx++;
                        hv.HoTen = cleanName;
                        if (hv.TaiKhoan != null) hv.TaiKhoan.HoTen = cleanName;
                    }
                    if (hv.GioiTinh == "Nu") hv.GioiTinh = "Nữ";
                    else if (hv.GioiTinh == "Nam") hv.GioiTinh = "Nam";
                }

                context.SaveChanges();
                Console.WriteLine("✅ Đã chuẩn hóa toàn bộ dữ liệu Tiếng Việt thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sanitizing data: " + ex.Message);
            }
        }
    }
}

