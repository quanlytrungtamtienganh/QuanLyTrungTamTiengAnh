using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class LopHocDetailsViewModel
    {
        public LopHoc LopHoc { get; set; } = null!;
        public int SoLuongDaDangKy { get; set; }
        public int SoLuongConLai => LopHoc.SoLuongToiDa - SoLuongDaDangKy;
        public bool CanRegister { get; set; }
        public string? ReasonCannotRegister { get; set; }
        public bool AlreadyRegistered { get; set; }
        public bool IsStudent { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
