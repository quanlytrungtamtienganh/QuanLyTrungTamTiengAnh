using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Data
{
    public class TrungTamDbContext : DbContext
    {
        public TrungTamDbContext(DbContextOptions<TrungTamDbContext> options) : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public DbSet<KhoaHoc> KhoaHocs { get; set; } = null!;
        public DbSet<LopHoc> LopHocs { get; set; } = null!;
        public DbSet<HocVien> HocViens { get; set; } = null!;
        public DbSet<DangKyHoc> DangKyHocs { get; set; } = null!;
        public DbSet<BinhLuan> BinhLuans { get; set; } = null!;
        public DbSet<GiaoVien> GiaoViens { get; set; } = null!;
        public DbSet<TinNhan> TinNhans { get; set; } = null!;
        public DbSet<NhomChat> NhomChats { get; set; } = null!;
        public DbSet<ThanhVienNhom> ThanhVienNhoms { get; set; } = null!;
        public DbSet<BaiViet> BaiViets { get; set; } = null!;
        public DbSet<BaoCaoBaiViet> BaoCaoBaiViets { get; set; } = null!;
        public DbSet<CauHinhHeThong> CauHinhHeThongs { get; set; } = null!;
        public DbSet<ThichBaiViet> ThichBaiViets { get; set; } = null!;
        public DbSet<YeuCauDangKy> YeuCauDangKys { get; set; } = null!;
        public DbSet<OtpDoiMatKhau> OtpDoiMatKhaus { get; set; } = null!;
        public DbSet<LichSuDoiMatKhau> LichSuDoiMatKhaus { get; set; } = null!;

        // Hệ thống Luyện thi trực tuyến (Study4 Style)
        public DbSet<DeThi> DeThis { get; set; } = null!;
        public DbSet<PhanThi> PhanThis { get; set; } = null!;
        public DbSet<CauHoiLuyenThi> CauHoiLuyenThis { get; set; } = null!;
        public DbSet<BinhLuanDeThi> BinhLuanDeThis { get; set; } = null!;
        public DbSet<KetQuaLuyenThi> KetQuaLuyenThis { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Quan hệ 1 - 1 giữa TaiKhoan và HocVien
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.HocVien)
                .WithOne(h => h.TaiKhoan)
                .HasForeignKey<HocVien>(h => h.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // 1.1 Quan hệ 1 - 1 giữa TaiKhoan và GiaoVien
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.GiaoVien)
                .WithOne(g => g.TaiKhoan)
                .HasForeignKey<GiaoVien>(g => g.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Quan hệ 1 - N giữa KhoaHoc và LopHoc
            modelBuilder.Entity<KhoaHoc>()
                .HasMany(k => k.LopHocs)
                .WithOne(l => l.KhoaHoc)
                .HasForeignKey(l => l.MaKhoaHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 2.1 Quan hệ 1 - N giữa GiaoVien và LopHoc
            modelBuilder.Entity<GiaoVien>()
                .HasMany(g => g.LopHocs)
                .WithOne(l => l.GiaoVien)
                .HasForeignKey(l => l.MaGiaoVien)
                .OnDelete(DeleteBehavior.SetNull);

            // 3. Quan hệ 1 - N giữa LopHoc và DangKyHoc
            modelBuilder.Entity<LopHoc>()
                .HasMany(l => l.DangKyHocs)
                .WithOne(d => d.LopHoc)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Quan hệ 1 - N giữa HocVien và DangKyHoc
            modelBuilder.Entity<HocVien>()
                .HasMany(h => h.DangKyHocs)
                .WithOne(d => d.HocVien)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Quan hệ 1 - N giữa TaiKhoan và BinhLuan
            modelBuilder.Entity<TaiKhoan>()
                .HasMany(t => t.BinhLuans)
                .WithOne(b => b.TaiKhoan)
                .HasForeignKey(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. Quan hệ 1 - N giữa KhoaHoc và BinhLuan
            modelBuilder.Entity<KhoaHoc>()
                .HasMany(k => k.BinhLuans)
                .WithOne(b => b.KhoaHoc)
                .HasForeignKey(b => b.MaKhoaHoc)
                .OnDelete(DeleteBehavior.SetNull);

            // 7. Quan hệ TinNhan (Tin nhắn chat 1-1 và chat nhóm)
            modelBuilder.Entity<TinNhan>()
                .HasOne(t => t.NguoiGui)
                .WithMany()
                .HasForeignKey(t => t.NguoiGuiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TinNhan>()
                .HasOne(t => t.NguoiNhan)
                .WithMany()
                .HasForeignKey(t => t.NguoiNhanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TinNhan>()
                .HasOne(t => t.NhomChat)
                .WithMany(n => n.TinNhans)
                .HasForeignKey(t => t.MaNhom)
                .OnDelete(DeleteBehavior.Cascade);

            // 8. Quan hệ NhomChat và ThanhVienNhom
            modelBuilder.Entity<NhomChat>()
                .HasOne(n => n.NguoiTao)
                .WithMany()
                .HasForeignKey(n => n.NguoiTaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThanhVienNhom>()
                .HasOne(t => t.NhomChat)
                .WithMany(n => n.ThanhViens)
                .HasForeignKey(t => t.MaNhom)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThanhVienNhom>()
                .HasOne(t => t.TaiKhoan)
                .WithMany()
                .HasForeignKey(t => t.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // 9. Quan hệ BaiViet và BaoCaoBaiViet
            modelBuilder.Entity<BaiViet>()
                .HasOne(b => b.NguoiDang)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaiViet>()
                .HasOne(b => b.NguoiDuyet)
                .WithMany()
                .HasForeignKey(b => b.NguoiDuyetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaoCaoBaiViet>()
                .HasOne(r => r.BaiViet)
                .WithMany(b => b.BaoCaos)
                .HasForeignKey(r => r.MaBaiViet)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaoCaoBaiViet>()
                .HasOne(r => r.NguoiBaoCao)
                .WithMany()
                .HasForeignKey(r => r.NguoiBaoCaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BinhLuan>()
                .HasOne(b => b.BaiViet)
                .WithMany(bv => bv.BinhLuans)
                .HasForeignKey(b => b.MaBaiViet)
                .OnDelete(DeleteBehavior.Cascade);

            // 9.1 Quan hệ ThichBaiViet
            modelBuilder.Entity<ThichBaiViet>()
                .HasOne(t => t.BaiViet)
                .WithMany(b => b.Thichs)
                .HasForeignKey(t => t.MaBaiViet)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThichBaiViet>()
                .HasOne(t => t.TaiKhoan)
                .WithMany()
                .HasForeignKey(t => t.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThichBaiViet>()
                .HasIndex(t => new { t.MaBaiViet, t.MaTaiKhoan })
                .IsUnique();

            // 9.2 Quan hệ chia sẻ bài viết (BaiVietGoc)
            modelBuilder.Entity<BaiViet>()
                .HasOne(b => b.BaiVietGoc)
                .WithMany()
                .HasForeignKey(b => b.MaBaiVietGoc)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình độ chính xác số thập phân cho Học phí
            modelBuilder.Entity<KhoaHoc>()
                .Property(k => k.HocPhi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<LopHoc>()
                .Property(l => l.HocPhi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DangKyHoc>()
                .Property(d => d.HocPhi)
                .HasPrecision(18, 2);

            // 10. Quan hệ Đề thi - Phần thi - Câu hỏi
            modelBuilder.Entity<PhanThi>()
                .HasOne(p => p.DeThi)
                .WithMany(d => d.PhanThis)
                .HasForeignKey(p => p.MaDeThi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CauHoiLuyenThi>()
                .HasOne(c => c.PhanThi)
                .WithMany(p => p.CauHois)
                .HasForeignKey(c => c.MaPhanThi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BinhLuanDeThi>()
                .HasOne(b => b.DeThi)
                .WithMany(d => d.BinhLuans)
                .HasForeignKey(b => b.MaDeThi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BinhLuanDeThi>()
                .HasOne(b => b.TaiKhoan)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KetQuaLuyenThi>()
                .HasOne(k => k.DeThi)
                .WithMany(d => d.KetQuas)
                .HasForeignKey(k => k.MaDeThi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KetQuaLuyenThi>()
                .HasOne(k => k.TaiKhoan)
                .WithMany()
                .HasForeignKey(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
