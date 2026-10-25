using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class LandingViewModel
    {
        public List<KhoaHoc> KhoaHocs { get; set; } = new();
        public List<GiaoVien> GiaoViens { get; set; } = new();
        public List<DeThi> DeThisHot { get; set; } = new();
    }
}

