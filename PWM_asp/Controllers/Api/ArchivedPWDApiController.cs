using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWM_asp.Models;
using PWM_asp.Services;

namespace PWM_asp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArchivedPWDController : ControllerBase
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

        // GET: api/ArchivedPWD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);

            var list = await _context.ArchivedPWDs
                .Where(s => s.UserId == userId)
                .Include(s => s.Source)
                .Select(s => new {
                    s.ArchivedPWDId,
                    s.Account,
                    s.Description,
                    Source = s.Source.SourceName,
                    ArchivedAt = s.ArchivedAt.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/ArchivedPWD/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.ArchivedPWDs
                .Where(s => s.UserId == userId)
                .Include(s => s.Source)
                .FirstOrDefaultAsync(s => s.ArchivedPWDId == id);

            if (item == null)
                return NotFound();

            return Ok(new
            {
                item.ArchivedPWDId,
                item.Account,
                item.Description,
                Source = item.Source.SourceName,
                ArchivedAt = item.ArchivedAt.ToString("dd/MM/yyyy HH:mm")
            });
        }

        // POST: api/ArchivedPWD/5/reveal
        [HttpPost("{id}/reveal")]
        public async Task<IActionResult> Reveal(int id)
        {
            var userId = _userManager.GetUserId(User);

            var saved = await _context.ArchivedPWDs
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync(s => s.ArchivedPWDId == id);

            if (saved == null)
                return NotFound();

            var decrypted = _encryption.Decrypt(saved.EncryptedPWD, saved.EncryptedDataKey);

            return Ok(new { password = decrypted });
        }
    }
}
