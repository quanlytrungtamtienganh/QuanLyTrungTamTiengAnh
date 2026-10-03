using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using quanlytrungtamtienganh.Models;

namespace quanlytrungtamtienganh.Pages.LopHoc
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LopHoc LopHoc { get; set; } = new LopHoc();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LopHocs.Add(LopHoc);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}