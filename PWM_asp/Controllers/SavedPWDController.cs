using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWM_asp.Extensions;
using PWM_asp.Models;
using PWM_asp.Services;


[Authorize]
public class SavedPWDController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEncryptionService _encryption;
    private readonly UserManager<AppUser> _userManager;

    public SavedPWDController(ApplicationDbContext context, IEncryptionService encryption, UserManager<AppUser> userManager)
    {
        _context = context;
        _encryption = encryption;
        _userManager = userManager;
    }

    // INDEX (no decryption)
    public async Task<IActionResult> Index(string search)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.SavedPWDs
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


    // DETAILS (decrypt on demand)
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);

        var entity = await _context.SavedPWDs
            .Where(s => s.UserId == userId)
            .Include(s => s.Source)
            .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (entity == null)
            return NotFound();

        // Only decrypt if Reveal button was used
        if (TempData["RevealedPassword"] != null)
        {
            ViewBag.DecryptedPassword = TempData["RevealedPassword"];
        }

        return View(entity);
    }


    // GET: SavedPWD/Create
    public IActionResult Create()
    {
        // Optional: preload source names for dropdown
        ViewBag.SourceNames = new SelectList(_context.Sources.Select(s => s.SourceName));
        return View(new SavedPWDViewModel());
    }

    // POST: SavedPWD/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavedPWDViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SourceNames = new SelectList(_context.Sources.Select(s => s.SourceName));
            return View(model);
        }

        // 1. Resolve or create Source
        var source = await _context.Sources
            .FirstOrDefaultAsync(s => s.SourceName == model.SourceName);

        if (source == null)
        {
            source = new Source { SourceName = model.SourceName };
            _context.Sources.Add(source);
            await _context.SaveChangesAsync();
        }

        // 2. Encrypt password
        var (encryptedPwd, encryptedKey) = _encryption.Encrypt(model.Password);

        // 3. Create entity
        var saved = new SavedPWD
        {
            Account = model.Account,
            Description = model.Description,
            SourceId = source.SourceId,
            EncryptedPWD = encryptedPwd,
            EncryptedDataKey = encryptedKey,
            UserId = _userManager.GetUserId(User),
            CreatedAt = DateTime.Now
        };

        _context.SavedPWDs.Add(saved);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Password saved successfully.";
        return RedirectToAction(nameof(Index));
    }


    // GET: SavedPWD/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);

        var saved = await _context.SavedPWDs
            .Where(s => s.UserId == userId)
            .Include(s => s.Source)
            .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (saved == null)
            return NotFound();

        var model = new SavedPWDViewModel
        {
            SavedPWDId = saved.SavedPWDId,
            Account = saved.Account,
            Description = saved.Description,
            SourceName = saved.Source?.SourceName ?? "",
            Password = "" // user must re-enter if they want to change
        };

        ViewBag.SourceNames = new SelectList(_context.Sources.Select(s => s.SourceName));
        return View(model);
    }

    // POST: SavedPWD/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SavedPWDViewModel model)
    {
        if (id != model.SavedPWDId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.SourceNames = new SelectList(_context.Sources.Select(s => s.SourceName));
            return View(model);
        }

        var userId = _userManager.GetUserId(User);

        var saved = await _context.SavedPWDs
            .Where(s => s.UserId == userId)
            .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (saved == null)
            return NotFound();

        saved.Account = model.Account;
        saved.Description = model.Description;
        saved.UpdatedAt = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var (encryptedPwd, encryptedKey) = _encryption.Encrypt(model.Password);
            saved.EncryptedPWD = encryptedPwd;
            saved.EncryptedDataKey = encryptedKey;
        }

        await _context.SaveChangesAsync();


        TempData["Success"] = "Password updated successfully.";
        return RedirectToAction(nameof(Index));
    }


    // GET: SavedPWD/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);

        var saved = await _context.SavedPWDs
            .Where(s => s.UserId == userId)
            .Include(s => s.Source)
            .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (saved == null)
            return NotFound();

        return View(saved);
    }


    // POST: SavedPWD/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {

        var userId = _userManager.GetUserId(User);

        var saved = await _context.SavedPWDs
            .Where(s => s.UserId == userId)
            .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (saved == null)
            return NotFound();

        _context.SavedPWDs.Remove(saved);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Password deleted (archived) successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Reveal(int id)
    {
        var userId = _userManager.GetUserId(User);

        var saved = await _context.SavedPWDs
                    .Where(s => s.UserId == userId)
                    .FirstOrDefaultAsync(s => s.SavedPWDId == id);

        if (saved == null)
            return NotFound();

        var decrypted = _encryption.Decrypt(saved.EncryptedPWD, saved.EncryptedDataKey);

        TempData["RevealedPassword"] = decrypted;
        return RedirectToAction("Details", new { id });
    }

}
