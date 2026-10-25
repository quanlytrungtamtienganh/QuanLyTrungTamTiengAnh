using Microsoft.AspNetCore.Mvc.Rendering;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class LopHocFilterViewModel
    {
        // 1. Search & Filters
        public string? SearchTerm { get; set; }
        public int? MaKhoaHoc { get; set; }
        public string? NgoaiNgu { get; set; }
        public string? TrangThai { get; set; }
        public decimal? HocPhiTu { get; set; }
        public decimal? HocPhiDen { get; set; }

        // 2. Sorting
        public string? SortOrder { get; set; } // "ten_asc", "ten_desc", "ngay_asc", "ngay_desc", "hocphi_asc", "hocphi_desc"

        // 3. Pagination
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // 4. Data Items
        public List<LopHoc> Items { get; set; } = new List<LopHoc>();

        // 5. SelectLists for Dropdowns
        public SelectList? KhoaHocList { get; set; }
        public SelectList? NgoaiNguList { get; set; }
        public SelectList? TrangThaiList { get; set; }
        public SelectList? SortOrderList { get; set; }
    }
}
