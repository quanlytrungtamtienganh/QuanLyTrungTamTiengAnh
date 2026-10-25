using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class LuyenThiIndexViewModel
    {
        public List<DeThi> DeThis { get; set; } = new List<DeThi>();
        public string TheLoaiHienTai { get; set; } = "TatCa";
        public string? TuKhoa { get; set; }
        public string Tab { get; set; } = "TatCa"; // TatCa hoặc DeRutGon
        public int TongSoDe { get; set; }
        public List<KetQuaLuyenThi> LichSuLamBaiGanDay { get; set; } = new List<KetQuaLuyenThi>();
    }

    public class ChiTietDeThiViewModel
    {
        public DeThi DeThi { get; set; } = null!;
        public List<PhanThi> PhanThis { get; set; } = new List<PhanThi>();
        public List<BinhLuanDeThi> BinhLuans { get; set; } = new List<BinhLuanDeThi>();
        public int TongSoCauHoi { get; set; }
        public int TongSoBinhLuan { get; set; }
    }

    public class ExamTicket
    {
        public string TicketId { get; set; } = string.Empty;
        public int MaDeThi { get; set; }
        public string CheDo { get; set; } = "LuyenTap";
        public List<int> PhanThiIds { get; set; } = new List<int>();
        public int ThoiGianPhut { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime? ExpireTimeUtc { get; set; }
        public int RemainingSeconds { get; set; }
    }

    public class KhoiTaoThiInputModel
    {
        public int MaDeThi { get; set; }
        public string CheDo { get; set; } = "LuyenTap";
        public string? PhanThiIds { get; set; }
        public int? ThoiGianPhut { get; set; }
    }

    public class LamBaiViewModel
    {
        public string TicketId { get; set; } = string.Empty;
        public DeThi DeThi { get; set; } = null!;
        public List<PhanThi> PhanThis { get; set; } = new List<PhanThi>();
        public string CheDo { get; set; } = "LuyenTap"; // LuyenTap hoặc FullTest
        public List<int> PhanThiIds { get; set; } = new List<int>();
        public int ThoiGianPhut { get; set; } = 40; // 0 = Không giới hạn
        public int RemainingSeconds { get; set; } = 0;
        public int TongSoCau { get; set; }
    }

    public class NopBaiInputModel
    {
        public string? TicketId { get; set; }
        public int MaDeThi { get; set; }
        public string CheDo { get; set; } = "LuyenTap";
        public string? CacPhanDaLam { get; set; }
        public int ThoiGianLamGiay { get; set; }
        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>();
    }

    public class KetQuaChiTietViewModel
    {
        public KetQuaLuyenThi KetQua { get; set; } = null!;
        public DeThi DeThi { get; set; } = null!;
        public List<ThongKeDangBaiItem> ThongKeDangBai { get; set; } = new List<ThongKeDangBaiItem>();
        public List<CauHoiKetQuaItem> DanhSachCauHoi { get; set; } = new List<CauHoiKetQuaItem>();
    }

    public class ThongKeDangBaiItem
    {
        public string TenDangBai { get; set; } = string.Empty;
        public int SoCauDung { get; set; }
        public int SoCauSai { get; set; }
        public int SoCauBoQua { get; set; }
        public int TongSoCau => SoCauDung + SoCauSai + SoCauBoQua;
        public double DoChinhXac => TongSoCau > 0 ? Math.Round((double)SoCauDung / TongSoCau * 100, 1) : 0;
        public List<CauHoiBadgeItem> DanhSachSoCau { get; set; } = new List<CauHoiBadgeItem>();
    }

    public class CauHoiBadgeItem
    {
        public int SoThuTu { get; set; }
        public string TrangThai { get; set; } = "BoQua"; // Dung, Sai, BoQua
        public int MaCauHoi { get; set; }
    }

    public class CauHoiKetQuaItem
    {
        public int MaCauHoi { get; set; }
        public int SoThuTu { get; set; }
        public string TenPhanThi { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiCauHoi { get; set; } = "TracNghiem";
        public string? DapAnA { get; set; }
        public string? DapAnB { get; set; }
        public string? DapAnC { get; set; }
        public string? DapAnD { get; set; }
        public string DapAnDung { get; set; } = string.Empty;
        public string? DapAnCuaBan { get; set; }
        public bool IsDung { get; set; }
        public bool IsBoQua { get; set; }
        public string? GiaiThichChiTiet { get; set; }
        public string? PhanLoaiDangBai { get; set; }
    }
}
