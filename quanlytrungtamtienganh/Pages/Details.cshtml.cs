using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using quanlytrungtamtienganh.Models;

namespace quanlytrungtamtienganh.Pages.LopHoc
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public LopHoc LopHoc { get; set; } = new LopHoc();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LopHoc = await _context.LopHocs
                .FirstOrDefaultAsync(x => x.MaLop == id);

            if (LopHoc == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}