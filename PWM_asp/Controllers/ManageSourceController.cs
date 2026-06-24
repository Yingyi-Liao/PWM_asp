using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWM_asp.Models;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ManageSourceController : Controller
{
    private readonly ApplicationDbContext _context;

    public ManageSourceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sources = await _context.Sources
            .OrderBy(s => s.SourceName)
            .ToListAsync();

        return View(sources);
    }
    public async Task<IActionResult> Details(int id)
    {
        var source = await _context.Sources.FindAsync(id);
        if (source == null) return NotFound();

        return View(source);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var source = await _context.Sources.FindAsync(id);
        if (source == null) return NotFound();

        return View(source);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Source model)
    {
        if (id != model.SourceId) return BadRequest();

        var source = await _context.Sources.FindAsync(id);
        if (source == null) return NotFound();

        source.SourceName = model.SourceName;
        source.Description = model.Description;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Source updated.";
        return RedirectToAction("Index");
    }
}
