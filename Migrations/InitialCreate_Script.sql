IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [CauHinhHeThong] (
    [Khoa] nvarchar(100) NOT NULL,
    [GiaTri] nvarchar(500) NOT NULL,
    [MoTa] nvarchar(255) NULL,
    [NgayCapNhat] datetime2 NOT NULL,
    CONSTRAINT [PK_CauHinhHeThong] PRIMARY KEY ([Khoa])
);

CREATE TABLE [DeThi] (
    [MaDeThi] int NOT NULL IDENTITY,
    [TieuDe] nvarchar(255) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [TheLoai] nvarchar(50) NOT NULL,
    [KyNang] nvarchar(50) NOT NULL,
    [ThoiGianPhut] int NOT NULL,
    [TongSoCau] int NOT NULL,
    [SoLuotLam] int NOT NULL,
    [HinhAnh] nvarchar(255) NULL,
    [FileAudio] nvarchar(500) NULL,
    [NgayTao] datetime2 NOT NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_DeThi] PRIMARY KEY ([MaDeThi])
);

CREATE TABLE [KhoaHoc] (
    [MaKhoaHoc] int NOT NULL IDENTITY,
    [TenKhoaHoc] nvarchar(150) NOT NULL,
    [NgoaiNgu] nvarchar(50) NOT NULL,
    [CapDo] nvarchar(50) NOT NULL,
    [HocPhi] decimal(18,2) NOT NULL,
    [SoBuoi] int NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_KhoaHoc] PRIMARY KEY ([MaKhoaHoc])
);

CREATE TABLE [TaiKhoan] (
    [MaTaiKhoan] int NOT NULL IDENTITY,
    [TenDangNhap] nvarchar(50) NOT NULL,
    [MatKhau] nvarchar(255) NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [VaiTro] nvarchar(20) NOT NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_TaiKhoan] PRIMARY KEY ([MaTaiKhoan])
);

CREATE TABLE [YeuCauDangKy] (
    [MaYeuCau] int NOT NULL IDENTITY,
    [HoTen] nvarchar(150) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [SoDienThoai] nvarchar(20) NOT NULL,
    [VaiTroMongMuon] nvarchar(30) NOT NULL,
    [GhiChu] nvarchar(500) NULL,
    [NgayGui] datetime2 NOT NULL,
    [TrangThai] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_YeuCauDangKy] PRIMARY KEY ([MaYeuCau])
);

CREATE TABLE [PhanThi] (
    [MaPhanThi] int NOT NULL IDENTITY,
    [MaDeThi] int NOT NULL,
    [TenPhanThi] nvarchar(100) NOT NULL,
    [ThuTu] int NOT NULL,
    [TieuDePhan] nvarchar(255) NULL,
    [DoanVanDoc] nvarchar(max) NULL,
    [FileAudio] nvarchar(500) NULL,
    [TagDangBai] nvarchar(100) NULL,
    CONSTRAINT [PK_PhanThi] PRIMARY KEY ([MaPhanThi]),
    CONSTRAINT [FK_PhanThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE
);

CREATE TABLE [BaiViet] (
    [MaBaiViet] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [NoiDung] nvarchar(4000) NOT NULL,
    [HinhAnhUrl] nvarchar(500) NULL,
    [VideoUrl] nvarchar(500) NULL,
    [PhongNen] nvarchar(50) NULL,
    [NgayDang] datetime2 NOT NULL,
    [TrangThai] nvarchar(30) NOT NULL,
    [NguoiDuyetId] int NULL,
    [NgayDuyet] datetime2 NULL,
    [MaBaiVietGoc] int NULL,
    [SoChiaSe] int NOT NULL,
    CONSTRAINT [PK_BaiViet] PRIMARY KEY ([MaBaiViet]),
    CONSTRAINT [FK_BaiViet_BaiViet_MaBaiVietGoc] FOREIGN KEY ([MaBaiVietGoc]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BaiViet_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BaiViet_TaiKhoan_NguoiDuyetId] FOREIGN KEY ([NguoiDuyetId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [BinhLuanDeThi] (
    [MaBinhLuan] int NOT NULL IDENTITY,
    [MaDeThi] int NOT NULL,
    [MaTaiKhoan] int NOT NULL,
    [NoiDung] nvarchar(1000) NOT NULL,
    [NgayBinhLuan] datetime2 NOT NULL,
    [MaBinhLuanCha] int NULL,
    CONSTRAINT [PK_BinhLuanDeThi] PRIMARY KEY ([MaBinhLuan]),
    CONSTRAINT [FK_BinhLuanDeThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE,
    CONSTRAINT [FK_BinhLuanDeThi_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);

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

CREATE TABLE [HocVien] (
    [MaHocVien] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [NgaySinh] datetime2 NOT NULL,
    [GioiTinh] nvarchar(10) NOT NULL,
    [SoDienThoai] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [DiaChi] nvarchar(255) NULL,
    CONSTRAINT [PK_HocVien] PRIMARY KEY ([MaHocVien]),
    CONSTRAINT [FK_HocVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);

CREATE TABLE [KetQuaLuyenThi] (
    [MaKetQua] int NOT NULL IDENTITY,
    [MaDeThi] int NOT NULL,
    [MaTaiKhoan] int NULL,
    [CheDoLam] nvarchar(50) NOT NULL,
    [CacPhanDaLam] nvarchar(255) NULL,
    [SoCauDung] int NOT NULL,
    [SoCauSai] int NOT NULL,
    [SoCauBoQua] int NOT NULL,
    [TongSoCau] int NOT NULL,
    [DiemSo] float NOT NULL,
    [DoChinhXac] float NOT NULL,
    [ThoiGianLamGiay] int NOT NULL,
    [NgayNop] datetime2 NOT NULL,
    [ChiTietBaiLamJson] nvarchar(max) NULL,
    CONSTRAINT [PK_KetQuaLuyenThi] PRIMARY KEY ([MaKetQua]),
    CONSTRAINT [FK_KetQuaLuyenThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE,
    CONSTRAINT [FK_KetQuaLuyenThi_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
);

CREATE TABLE [LichSuDoiMatKhau] (
    [Id] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [NgayDoi] date NOT NULL,
    [ThoiGianDoi] datetime2 NOT NULL,
    CONSTRAINT [PK_LichSuDoiMatKhau] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LichSuDoiMatKhau_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);

CREATE TABLE [NhomChat] (
    [MaNhom] int NOT NULL IDENTITY,
    [TenNhom] nvarchar(150) NOT NULL,
    [AnhNhom] nvarchar(255) NULL,
    [NguoiTaoId] int NOT NULL,
    [NgayTao] datetime2 NOT NULL,
    [CuocGoiDangDienRa] bit NOT NULL,
    [LoaiCuocGoi] nvarchar(20) NULL,
    [NguoiGoiId] int NULL,
    [ThoiGianBatDauGoi] datetime2 NULL,
    CONSTRAINT [PK_NhomChat] PRIMARY KEY ([MaNhom]),
    CONSTRAINT [FK_NhomChat_TaiKhoan_NguoiGoiId] FOREIGN KEY ([NguoiGoiId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]),
    CONSTRAINT [FK_NhomChat_TaiKhoan_NguoiTaoId] FOREIGN KEY ([NguoiTaoId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [OtpDoiMatKhau] (
    [Id] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [MaOtp] nvarchar(10) NOT NULL,
    [ThoiGianTao] datetime2 NOT NULL,
    [ThoiGianHetHan] datetime2 NOT NULL,
    [DaSuDung] bit NOT NULL,
    CONSTRAINT [PK_OtpDoiMatKhau] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OtpDoiMatKhau_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);

CREATE TABLE [CauHoiLuyenThi] (
    [MaCauHoi] int NOT NULL IDENTITY,
    [MaPhanThi] int NOT NULL,
    [SoThuTu] int NOT NULL,
    [NoiDung] nvarchar(max) NOT NULL,
    [LoaiCauHoi] nvarchar(30) NOT NULL,
    [DapAnA] nvarchar(max) NULL,
    [DapAnB] nvarchar(max) NULL,
    [DapAnC] nvarchar(max) NULL,
    [DapAnD] nvarchar(max) NULL,
    [DapAnDung] nvarchar(255) NOT NULL,
    [GiaiThichChiTiet] nvarchar(max) NULL,
    [PhanLoaiDangBai] nvarchar(100) NULL,
    [Diem] float NOT NULL,
    CONSTRAINT [PK_CauHoiLuyenThi] PRIMARY KEY ([MaCauHoi]),
    CONSTRAINT [FK_CauHoiLuyenThi_PhanThi_MaPhanThi] FOREIGN KEY ([MaPhanThi]) REFERENCES [PhanThi] ([MaPhanThi]) ON DELETE CASCADE
);

CREATE TABLE [BaoCaoBaiViet] (
    [MaBaoCao] int NOT NULL IDENTITY,
    [MaBaiViet] int NOT NULL,
    [NguoiBaoCaoId] int NOT NULL,
    [LyDo] nvarchar(200) NOT NULL,
    [ChiTiet] nvarchar(1000) NULL,
    [NgayBaoCao] datetime2 NOT NULL,
    [TrangThai] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_BaoCaoBaiViet] PRIMARY KEY ([MaBaoCao]),
    CONSTRAINT [FK_BaoCaoBaiViet_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE,
    CONSTRAINT [FK_BaoCaoBaiViet_TaiKhoan_NguoiBaoCaoId] FOREIGN KEY ([NguoiBaoCaoId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [BinhLuan] (
    [MaBinhLuan] int NOT NULL IDENTITY,
    [NoiDung] nvarchar(1000) NOT NULL,
    [NgayBinhLuan] datetime2 NOT NULL,
    [MaTaiKhoan] int NOT NULL,
    [MaKhoaHoc] int NULL,
    [MaBaiViet] int NULL,
    CONSTRAINT [PK_BinhLuan] PRIMARY KEY ([MaBinhLuan]),
    CONSTRAINT [FK_BinhLuan_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE,
    CONSTRAINT [FK_BinhLuan_KhoaHoc_MaKhoaHoc] FOREIGN KEY ([MaKhoaHoc]) REFERENCES [KhoaHoc] ([MaKhoaHoc]) ON DELETE SET NULL,
    CONSTRAINT [FK_BinhLuan_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);

CREATE TABLE [ThichBaiViet] (
    [MaThich] int NOT NULL IDENTITY,
    [MaBaiViet] int NOT NULL,
    [MaTaiKhoan] int NOT NULL,
    [NgayThich] datetime2 NOT NULL,
    CONSTRAINT [PK_ThichBaiViet] PRIMARY KEY ([MaThich]),
    CONSTRAINT [FK_ThichBaiViet_BaiViet_MaBaiViet] FOREIGN KEY ([MaBaiViet]) REFERENCES [BaiViet] ([MaBaiViet]) ON DELETE CASCADE,
    CONSTRAINT [FK_ThichBaiViet_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [LopHoc] (
    [MaLop] int NOT NULL IDENTITY,
    [TenLop] nvarchar(100) NOT NULL,
    [MaKhoaHoc] int NOT NULL,
    [NgayKhaiGiang] datetime2 NOT NULL,
    [LichHoc] nvarchar(100) NOT NULL,
    [PhongHoc] nvarchar(50) NOT NULL,
    [SoLuongToiDa] int NOT NULL,
    [HocPhi] decimal(18,2) NOT NULL,
    [TrangThai] nvarchar(50) NOT NULL,
    [MaGiaoVien] int NULL,
    CONSTRAINT [PK_LopHoc] PRIMARY KEY ([MaLop]),
    CONSTRAINT [FK_LopHoc_GiaoVien_MaGiaoVien] FOREIGN KEY ([MaGiaoVien]) REFERENCES [GiaoVien] ([MaGiaoVien]) ON DELETE SET NULL,
    CONSTRAINT [FK_LopHoc_KhoaHoc_MaKhoaHoc] FOREIGN KEY ([MaKhoaHoc]) REFERENCES [KhoaHoc] ([MaKhoaHoc]) ON DELETE NO ACTION
);

CREATE TABLE [ThanhVienNhom] (
    [MaThanhVien] int NOT NULL IDENTITY,
    [MaNhom] int NOT NULL,
    [MaTaiKhoan] int NOT NULL,
    [VaiTroTrongNhom] nvarchar(30) NOT NULL,
    [NgayThamGia] datetime2 NOT NULL,
    CONSTRAINT [PK_ThanhVienNhom] PRIMARY KEY ([MaThanhVien]),
    CONSTRAINT [FK_ThanhVienNhom_NhomChat_MaNhom] FOREIGN KEY ([MaNhom]) REFERENCES [NhomChat] ([MaNhom]) ON DELETE CASCADE,
    CONSTRAINT [FK_ThanhVienNhom_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [TinNhan] (
    [MaTinNhan] int NOT NULL IDENTITY,
    [NguoiGuiId] int NOT NULL,
    [NguoiNhanId] int NULL,
    [MaNhom] int NULL,
    [LoaiTinNhan] nvarchar(20) NOT NULL,
    [NoiDung] nvarchar(2000) NULL,
    [HinhAnhUrl] nvarchar(500) NULL,
    [ThoiGian] datetime2 NOT NULL,
    [DaXem] bit NOT NULL,
    CONSTRAINT [PK_TinNhan] PRIMARY KEY ([MaTinNhan]),
    CONSTRAINT [FK_TinNhan_NhomChat_MaNhom] FOREIGN KEY ([MaNhom]) REFERENCES [NhomChat] ([MaNhom]) ON DELETE CASCADE,
    CONSTRAINT [FK_TinNhan_TaiKhoan_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TinNhan_TaiKhoan_NguoiNhanId] FOREIGN KEY ([NguoiNhanId]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [DangKyHoc] (
    [MaDangKy] int NOT NULL IDENTITY,
    [MaHocVien] int NOT NULL,
    [MaLop] int NOT NULL,
    [NgayDangKy] datetime2 NOT NULL,
    [HocPhi] decimal(18,2) NOT NULL,
    [TrangThai] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_DangKyHoc] PRIMARY KEY ([MaDangKy]),
    CONSTRAINT [FK_DangKyHoc_HocVien_MaHocVien] FOREIGN KEY ([MaHocVien]) REFERENCES [HocVien] ([MaHocVien]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DangKyHoc_LopHoc_MaLop] FOREIGN KEY ([MaLop]) REFERENCES [LopHoc] ([MaLop]) ON DELETE NO ACTION
);

CREATE INDEX [IX_BaiViet_MaBaiVietGoc] ON [BaiViet] ([MaBaiVietGoc]);

CREATE INDEX [IX_BaiViet_MaTaiKhoan] ON [BaiViet] ([MaTaiKhoan]);

CREATE INDEX [IX_BaiViet_NguoiDuyetId] ON [BaiViet] ([NguoiDuyetId]);

CREATE INDEX [IX_BaoCaoBaiViet_MaBaiViet] ON [BaoCaoBaiViet] ([MaBaiViet]);

CREATE INDEX [IX_BaoCaoBaiViet_NguoiBaoCaoId] ON [BaoCaoBaiViet] ([NguoiBaoCaoId]);

CREATE INDEX [IX_BinhLuan_MaBaiViet] ON [BinhLuan] ([MaBaiViet]);

CREATE INDEX [IX_BinhLuan_MaKhoaHoc] ON [BinhLuan] ([MaKhoaHoc]);

CREATE INDEX [IX_BinhLuan_MaTaiKhoan] ON [BinhLuan] ([MaTaiKhoan]);

CREATE INDEX [IX_BinhLuanDeThi_MaDeThi] ON [BinhLuanDeThi] ([MaDeThi]);

CREATE INDEX [IX_BinhLuanDeThi_MaTaiKhoan] ON [BinhLuanDeThi] ([MaTaiKhoan]);

CREATE INDEX [IX_CauHoiLuyenThi_MaPhanThi] ON [CauHoiLuyenThi] ([MaPhanThi]);

CREATE INDEX [IX_DangKyHoc_MaHocVien] ON [DangKyHoc] ([MaHocVien]);

CREATE INDEX [IX_DangKyHoc_MaLop] ON [DangKyHoc] ([MaLop]);

CREATE UNIQUE INDEX [IX_GiaoVien_MaTaiKhoan] ON [GiaoVien] ([MaTaiKhoan]);

CREATE UNIQUE INDEX [IX_HocVien_MaTaiKhoan] ON [HocVien] ([MaTaiKhoan]);

CREATE INDEX [IX_KetQuaLuyenThi_MaDeThi] ON [KetQuaLuyenThi] ([MaDeThi]);

CREATE INDEX [IX_KetQuaLuyenThi_MaTaiKhoan] ON [KetQuaLuyenThi] ([MaTaiKhoan]);

CREATE INDEX [IX_LichSuDoiMatKhau_MaTaiKhoan] ON [LichSuDoiMatKhau] ([MaTaiKhoan]);

CREATE INDEX [IX_LopHoc_MaGiaoVien] ON [LopHoc] ([MaGiaoVien]);

CREATE INDEX [IX_LopHoc_MaKhoaHoc] ON [LopHoc] ([MaKhoaHoc]);

CREATE INDEX [IX_NhomChat_NguoiGoiId] ON [NhomChat] ([NguoiGoiId]);

CREATE INDEX [IX_NhomChat_NguoiTaoId] ON [NhomChat] ([NguoiTaoId]);

CREATE INDEX [IX_OtpDoiMatKhau_MaTaiKhoan] ON [OtpDoiMatKhau] ([MaTaiKhoan]);

CREATE INDEX [IX_PhanThi_MaDeThi] ON [PhanThi] ([MaDeThi]);

CREATE INDEX [IX_ThanhVienNhom_MaNhom] ON [ThanhVienNhom] ([MaNhom]);

CREATE INDEX [IX_ThanhVienNhom_MaTaiKhoan] ON [ThanhVienNhom] ([MaTaiKhoan]);

CREATE UNIQUE INDEX [IX_ThichBaiViet_MaBaiViet_MaTaiKhoan] ON [ThichBaiViet] ([MaBaiViet], [MaTaiKhoan]);

CREATE INDEX [IX_ThichBaiViet_MaTaiKhoan] ON [ThichBaiViet] ([MaTaiKhoan]);

CREATE INDEX [IX_TinNhan_MaNhom] ON [TinNhan] ([MaNhom]);

CREATE INDEX [IX_TinNhan_NguoiGuiId] ON [TinNhan] ([NguoiGuiId]);

CREATE INDEX [IX_TinNhan_NguoiNhanId] ON [TinNhan] ([NguoiNhanId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261026054339_InitialCreate', N'10.0.12');

COMMIT;
GO

