using Microsoft.AspNetCore.Mvc.Rendering;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class DangKyFilterViewModel
    {
        public string? SearchHocVien { get; set; }
        public int? MaLop { get; set; }
        public string? TrangThai { get; set; }

        public List<DangKyHoc> Items { get; set; } = new List<DangKyHoc>();
        public SelectList? LopHocList { get; set; }
        public SelectList? TrangThaiList { get; set; }
    }
}
