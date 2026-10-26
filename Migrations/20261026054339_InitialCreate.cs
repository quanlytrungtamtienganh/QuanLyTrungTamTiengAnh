using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrungTamNgoaiNgu.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhHeThong",
                columns: table => new
                {
                    Khoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhHeThong", x => x.Khoa);
                });

            migrationBuilder.CreateTable(
                name: "DeThi",
                columns: table => new
                {
                    MaDeThi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KyNang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianPhut = table.Column<int>(type: "int", nullable: false),
                    TongSoCau = table.Column<int>(type: "int", nullable: false),
                    SoLuotLam = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileAudio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.MaDeThi);
                });

            migrationBuilder.CreateTable(
                name: "KhoaHoc",
                columns: table => new
                {
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoaHoc = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NgoaiNgu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SoBuoi = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaHoc", x => x.MaKhoaHoc);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauDangKy",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VaiTroMongMuon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauDangKy", x => x.MaYeuCau);
                });

            migrationBuilder.CreateTable(
                name: "PhanThi",
                columns: table => new
                {
                    MaPhanThi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeThi = table.Column<int>(type: "int", nullable: false),
                    TenPhanThi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    TieuDePhan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DoanVanDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileAudio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TagDangBai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanThi", x => x.MaPhanThi);
                    table.ForeignKey(
                        name: "FK_PhanThi_DeThi_MaDeThi",
                        column: x => x.MaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaiViet",
                columns: table => new
                {
                    MaBaiViet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    HinhAnhUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhongNen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NguoiDuyetId = table.Column<int>(type: "int", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaBaiVietGoc = table.Column<int>(type: "int", nullable: true),
                    SoChiaSe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiViet", x => x.MaBaiViet);
                    table.ForeignKey(
                        name: "FK_BaiViet_BaiViet_MaBaiVietGoc",
                        column: x => x.MaBaiVietGoc,
                        principalTable: "BaiViet",
                        principalColumn: "MaBaiViet",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaiViet_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaiViet_TaiKhoan_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BinhLuanDeThi",
                columns: table => new
                {
                    MaBinhLuan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeThi = table.Column<int>(type: "int", nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NgayBinhLuan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaBinhLuanCha = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuanDeThi", x => x.MaBinhLuan);
                    table.ForeignKey(
                        name: "FK_BinhLuanDeThi_DeThi_MaDeThi",
                        column: x => x.MaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BinhLuanDeThi_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiaoVien",
                columns: table => new
                {
                    MaGiaoVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChuyenMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HocVi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GioiThieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVien", x => x.MaGiaoVien);
                    table.ForeignKey(
                        name: "FK_GiaoVien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.MaHocVien);
                    table.ForeignKey(
                        name: "FK_HocVien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaLuyenThi",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeThi = table.Column<int>(type: "int", nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    CheDoLam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CacPhanDaLam = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoCauDung = table.Column<int>(type: "int", nullable: false),
                    SoCauSai = table.Column<int>(type: "int", nullable: false),
                    SoCauBoQua = table.Column<int>(type: "int", nullable: false),
                    TongSoCau = table.Column<int>(type: "int", nullable: false),
                    DiemSo = table.Column<double>(type: "float", nullable: false),
                    DoChinhXac = table.Column<double>(type: "float", nullable: false),
                    ThoiGianLamGiay = table.Column<int>(type: "int", nullable: false),
                    NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChiTietBaiLamJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaLuyenThi", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaLuyenThi_DeThi_MaDeThi",
                        column: x => x.MaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaLuyenThi_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LichSuDoiMatKhau",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayDoi = table.Column<DateTime>(type: "date", nullable: false),
                    ThoiGianDoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDoiMatKhau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichSuDoiMatKhau_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhomChat",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AnhNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NguoiTaoId = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuocGoiDangDienRa = table.Column<bool>(type: "bit", nullable: false),
                    LoaiCuocGoi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NguoiGoiId = table.Column<int>(type: "int", nullable: true),
                    ThoiGianBatDauGoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomChat", x => x.MaNhom);
                    table.ForeignKey(
                        name: "FK_NhomChat_TaiKhoan_NguoiGoiId",
                        column: x => x.NguoiGoiId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan");
                    table.ForeignKey(
                        name: "FK_NhomChat_TaiKhoan_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OtpDoiMatKhau",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaOtp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaSuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpDoiMatKhau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpDoiMatKhau_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoiLuyenThi",
                columns: table => new
                {
                    MaCauHoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhanThi = table.Column<int>(type: "int", nullable: false),
                    SoThuTu = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiCauHoi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DapAnA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GiaiThichChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiDangBai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Diem = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiLuyenThi", x => x.MaCauHoi);
                    table.ForeignKey(
                        name: "FK_CauHoiLuyenThi_PhanThi_MaPhanThi",
                        column: x => x.MaPhanThi,
                        principalTable: "PhanThi",
                        principalColumn: "MaPhanThi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaoCaoBaiViet",
                columns: table => new
                {
                    MaBaoCao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBaiViet = table.Column<int>(type: "int", nullable: false),
                    NguoiBaoCaoId = table.Column<int>(type: "int", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChiTiet = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoBaiViet", x => x.MaBaoCao);
                    table.ForeignKey(
                        name: "FK_BaoCaoBaiViet_BaiViet_MaBaiViet",
                        column: x => x.MaBaiViet,
                        principalTable: "BaiViet",
                        principalColumn: "MaBaiViet",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaoCaoBaiViet_TaiKhoan_NguoiBaoCaoId",
                        column: x => x.NguoiBaoCaoId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BinhLuan",
                columns: table => new
                {
                    MaBinhLuan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NgayBinhLuan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: true),
                    MaBaiViet = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuan", x => x.MaBinhLuan);
                    table.ForeignKey(
                        name: "FK_BinhLuan_BaiViet_MaBaiViet",
                        column: x => x.MaBaiViet,
                        principalTable: "BaiViet",
                        principalColumn: "MaBaiViet",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BinhLuan_KhoaHoc_MaKhoaHoc",
                        column: x => x.MaKhoaHoc,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BinhLuan_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThichBaiViet",
                columns: table => new
                {
                    MaThich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBaiViet = table.Column<int>(type: "int", nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    NgayThich = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThichBaiViet", x => x.MaThich);
                    table.ForeignKey(
                        name: "FK_ThichBaiViet_BaiViet_MaBaiViet",
                        column: x => x.MaBaiViet,
                        principalTable: "BaiViet",
                        principalColumn: "MaBaiViet",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThichBaiViet_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false),
                    NgayKhaiGiang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LichHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhongHoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoLuongToiDa = table.Column<int>(type: "int", nullable: false),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopHoc_GiaoVien_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "GiaoVien",
                        principalColumn: "MaGiaoVien",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LopHoc_KhoaHoc_MaKhoaHoc",
                        column: x => x.MaKhoaHoc,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThanhVienNhom",
                columns: table => new
                {
                    MaThanhVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhom = table.Column<int>(type: "int", nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongNhom = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhVienNhom", x => x.MaThanhVien);
                    table.ForeignKey(
                        name: "FK_ThanhVienNhom_NhomChat_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NhomChat",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThanhVienNhom_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TinNhan",
                columns: table => new
                {
                    MaTinNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiGuiId = table.Column<int>(type: "int", nullable: false),
                    NguoiNhanId = table.Column<int>(type: "int", nullable: true),
                    MaNhom = table.Column<int>(type: "int", nullable: true),
                    LoaiTinNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HinhAnhUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaXem = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhan", x => x.MaTinNhan);
                    table.ForeignKey(
                        name: "FK_TinNhan_NhomChat_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NhomChat",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TinNhan_TaiKhoan_NguoiGuiId",
                        column: x => x.NguoiGuiId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TinNhan_TaiKhoan_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DangKyHoc",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyHoc", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyHoc_HocVien_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "HocVien",
                        principalColumn: "MaHocVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyHoc_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_MaBaiVietGoc",
                table: "BaiViet",
                column: "MaBaiVietGoc");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_MaTaiKhoan",
                table: "BaiViet",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_NguoiDuyetId",
                table: "BaiViet",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoBaiViet_MaBaiViet",
                table: "BaoCaoBaiViet",
                column: "MaBaiViet");

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoBaiViet_NguoiBaoCaoId",
                table: "BaoCaoBaiViet",
                column: "NguoiBaoCaoId");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaBaiViet",
                table: "BinhLuan",
                column: "MaBaiViet");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaKhoaHoc",
                table: "BinhLuan",
                column: "MaKhoaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaTaiKhoan",
                table: "BinhLuan",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanDeThi_MaDeThi",
                table: "BinhLuanDeThi",
                column: "MaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanDeThi_MaTaiKhoan",
                table: "BinhLuanDeThi",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiLuyenThi_MaPhanThi",
                table: "CauHoiLuyenThi",
                column: "MaPhanThi");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHoc_MaHocVien",
                table: "DangKyHoc",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHoc_MaLop",
                table: "DangKyHoc",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_MaTaiKhoan",
                table: "GiaoVien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HocVien_MaTaiKhoan",
                table: "HocVien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaLuyenThi_MaDeThi",
                table: "KetQuaLuyenThi",
                column: "MaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaLuyenThi_MaTaiKhoan",
                table: "KetQuaLuyenThi",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDoiMatKhau_MaTaiKhoan",
                table: "LichSuDoiMatKhau",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaKhoaHoc",
                table: "LopHoc",
                column: "MaKhoaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_NhomChat_NguoiGoiId",
                table: "NhomChat",
                column: "NguoiGoiId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomChat_NguoiTaoId",
                table: "NhomChat",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpDoiMatKhau_MaTaiKhoan",
                table: "OtpDoiMatKhau",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_PhanThi_MaDeThi",
                table: "PhanThi",
                column: "MaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienNhom_MaNhom",
                table: "ThanhVienNhom",
                column: "MaNhom");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienNhom_MaTaiKhoan",
                table: "ThanhVienNhom",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_ThichBaiViet_MaBaiViet_MaTaiKhoan",
                table: "ThichBaiViet",
                columns: new[] { "MaBaiViet", "MaTaiKhoan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThichBaiViet_MaTaiKhoan",
                table: "ThichBaiViet",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_MaNhom",
                table: "TinNhan",
                column: "MaNhom");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_NguoiGuiId",
                table: "TinNhan",
                column: "NguoiGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_NguoiNhanId",
                table: "TinNhan",
                column: "NguoiNhanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCaoBaiViet");

            migrationBuilder.DropTable(
                name: "BinhLuan");

            migrationBuilder.DropTable(
                name: "BinhLuanDeThi");

            migrationBuilder.DropTable(
                name: "CauHinhHeThong");

            migrationBuilder.DropTable(
                name: "CauHoiLuyenThi");

            migrationBuilder.DropTable(
                name: "DangKyHoc");

            migrationBuilder.DropTable(
                name: "KetQuaLuyenThi");

            migrationBuilder.DropTable(
                name: "LichSuDoiMatKhau");

            migrationBuilder.DropTable(
                name: "OtpDoiMatKhau");

            migrationBuilder.DropTable(
                name: "ThanhVienNhom");

            migrationBuilder.DropTable(
                name: "ThichBaiViet");

            migrationBuilder.DropTable(
                name: "TinNhan");

            migrationBuilder.DropTable(
                name: "YeuCauDangKy");

            migrationBuilder.DropTable(
                name: "PhanThi");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "BaiViet");

            migrationBuilder.DropTable(
                name: "NhomChat");

            migrationBuilder.DropTable(
                name: "DeThi");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "KhoaHoc");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
