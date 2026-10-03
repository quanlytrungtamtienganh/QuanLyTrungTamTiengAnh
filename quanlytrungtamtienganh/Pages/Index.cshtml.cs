using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using quanlytrungtamtienganh.Models;

namespace quanlytrungtamtienganh.Pages.LopHoc
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TrangThaiFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<LopHoc> query = _context.LopHocs;

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(x =>
                    x.TenLop.Contains(SearchString));
            }

            // Lọc trạng thái
            if (!string.IsNullOrWhiteSpace(TrangThaiFilter))
            {
                query = query.Where(x =>
                    x.TrangThai == TrangThaiFilter);
            }

            // Sắp xếp
            switch (SortOrder)
            {
                case "ten_desc":
                    query = query.OrderByDescending(x => x.TenLop);
                    break;

                case "ma_asc":
                    query = query.OrderBy(x => x.MaLop);
                    break;

                case "ma_desc":
                    query = query.OrderByDescending(x => x.MaLop);
                    break;

                case "ngay_asc":
                    query = query.OrderBy(x => x.NgayBatDau);
                    break;

                case "ngay_desc":
                    query = query.OrderByDescending(x => x.NgayBatDau);
                    break;

                default:
                    query = query.OrderBy(x => x.TenLop);
                    break;
            }

            // Tổng số trang
            int totalItems = await query.CountAsync();

            TotalPages = (int)Math.Ceiling(
                totalItems / (double)PageSize);

            if (TotalPages == 0)
            {
                TotalPages = 1;
            }

            // Không cho PageNumber vượt quá số trang
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            // Phân trang
            LopHocs = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}