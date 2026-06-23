using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWM_asp.Models;

namespace PWM_asp.Controllers
{
    [Authorize] // Only logged-in users can view audit history
    public class ArchivedPWDController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryption;
        private readonly UserManager<AppUser> _userManager;

        public ArchivedPWDController(ApplicationDbContext context, IEncryptionService encryption, UserManager<AppUser> userManager)
        {
            _context = context;
            _encryption = encryption;
            _userManager = userManager;
        }

        // GET: ArchivedPWD
        public async Task<IActionResult> Index(string search)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.ArchivedPWDs
                .Where(s => s.UserId == userId)
                .Include(s => s.Source)
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(s =>
                    s.Account.ToLower().Contains(search) ||
                    s.Description.ToLower().Contains(search) ||
                    s.Source.SourceName.ToLower().Contains(search)
                );
            }

            var list = await query.ToListAsync();

            ViewBag.Search = search;
            return View(list);
        }


        // GET: ArchivedPWD/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var item = await _context.ArchivedPWDs
                .Where(s => s.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Source)
                .FirstOrDefaultAsync(a => a.ArchivedPWDId == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        public async Task<IActionResult> Reveal(int id)
        {
            var userId = _userManager.GetUserId(User);

            var saved = await _context.ArchivedPWDs
                        .Where(s => s.UserId == userId)
                        .FirstOrDefaultAsync(s => s.ArchivedPWDId == id);

            if (saved == null)
                return NotFound();

            var decrypted = _encryption.Decrypt(saved.EncryptedPWD, saved.EncryptedDataKey);

            TempData["RevealedPassword"] = decrypted;
            return RedirectToAction("Details", new { id });
        }
    }
}
