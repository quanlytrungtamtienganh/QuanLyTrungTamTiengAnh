using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using quanlytrungtamtienganh.Models;

namespace quanlytrungtamtienganh.Pages.LopHoc
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (LopHoc.MaLop == 0)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs
                .FindAsync(LopHoc.MaLop);

            if (lopHoc == null)
            {
                return NotFound();
            }

            _context.LopHocs.Remove(lopHoc);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}