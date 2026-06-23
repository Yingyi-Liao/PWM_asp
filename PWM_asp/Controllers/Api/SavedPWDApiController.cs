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
    public class SavedPWDController : ControllerBase
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

        // GET: api/SavedPWD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);

            var list = await _context.SavedPWDs
                .Where(s => s.UserId == userId)
                .Include(s => s.Source)
                .Select(s => new {
                    s.SavedPWDId,
                    s.Account,
                    s.Description,
                    Source = s.Source.SourceName,
                    CreatedAt = s.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    UpdatedAt = s.UpdatedAt.HasValue ? s.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") : null
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/SavedPWD/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.SavedPWDs
                .Where(s => s.UserId == userId)
                .Include(s => s.Source)
                .FirstOrDefaultAsync(s => s.SavedPWDId == id);

            if (item == null)
                return NotFound();

            return Ok(new
            {
                item.SavedPWDId,
                item.Account,
                item.Description,
                Source = item.Source.SourceName,
                CreatedAt = item.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = item.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
            });
        }

        // POST: api/SavedPWD
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavedPWDViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);

            var source = await _context.Sources
                .FirstOrDefaultAsync(s => s.SourceName == model.SourceName)
                ?? new Source { SourceName = model.SourceName };

            if (source.SourceId == 0)
            {
                _context.Sources.Add(source);
                await _context.SaveChangesAsync();
            }

            var (encryptedPwd, encryptedKey) = _encryption.Encrypt(model.Password);

            var saved = new SavedPWD
            {
                Account = model.Account,
                Description = model.Description,
                SourceId = source.SourceId,
                EncryptedPWD = encryptedPwd,
                EncryptedDataKey = encryptedKey,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.SavedPWDs.Add(saved);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Created", saved.SavedPWDId });
        }

        // PUT: api/SavedPWD/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SavedPWDViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            var saved = await _context.SavedPWDs
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync(s => s.SavedPWDId == id);

            if (saved == null)
                return NotFound();

            saved.Account = model.Account;
            saved.Description = model.Description;
            saved.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var (encryptedPwd, encryptedKey) = _encryption.Encrypt(model.Password);
                saved.EncryptedPWD = encryptedPwd;
                saved.EncryptedDataKey = encryptedKey;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Updated" });
        }

        // DELETE: api/SavedPWD/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var saved = await _context.SavedPWDs
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync(s => s.SavedPWDId == id);

            if (saved == null)
                return NotFound();

            _context.SavedPWDs.Remove(saved);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted (archived)" });
        }

        // POST: api/SavedPWD/5/reveal
        [HttpPost("{id}/reveal")]
        public async Task<IActionResult> Reveal(int id)
        {
            var userId = _userManager.GetUserId(User);

            var saved = await _context.SavedPWDs
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync(s => s.SavedPWDId == id);

            if (saved == null)
                return NotFound();

            var decrypted = _encryption.Decrypt(saved.EncryptedPWD, saved.EncryptedDataKey);

            return Ok(new { password = decrypted });
        }
    }
}
