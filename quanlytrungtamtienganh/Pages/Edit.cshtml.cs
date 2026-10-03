using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace quanlytrungtamtienganh.Pages
{
    public class EditModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using quanlytrungtamtienganh.Models;

namespace quanlytrungtamtienganh.Pages.LopHoc
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LopHoc).State =
                EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LopHocExists(LopHoc.MaLop))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool LopHocExists(int id)
        {
            return _context.LopHocs
                .Any(e => e.MaLop == id);
        }
    }
}