using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LicenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetAdminId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("{appId}")]
        public async Task<IActionResult> GetLicenses(Guid appId)
        {
            var adminId = GetAdminId();
            var appExists = await _context.Applications.AnyAsync(a => a.Id == appId && a.AdminId == adminId);
            if (!appExists) return Unauthorized();

            var licenses = await _context.Licenses
                .Where(l => l.ApplicationId == appId)
                .ToListAsync();
            return Ok(licenses);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateLicenses([FromBody] GenerateLicenseRequest request)
        {
            var adminId = GetAdminId();
            var app = await _context.Applications.FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.AdminId == adminId);
            if (app == null) return Unauthorized();

            var newLicenses = new List<License>();
            for (int i = 0; i < request.Amount; i++)
            {
                newLicenses.Add(new License
                {
                    Id = Guid.NewGuid(),
                    Key = GenerateRandomKey(),
                    DurationDays = request.DurationDays,
                    ApplicationId = request.ApplicationId,
                    Note = request.Note
                });
            }

            _context.Licenses.AddRange(newLicenses);
            await _context.SaveChangesAsync();

            return Ok(newLicenses);
        }

        private string GenerateRandomKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public class GenerateLicenseRequest
        {
            public Guid ApplicationId { get; set; }
            public int Amount { get; set; }
            public int DurationDays { get; set; }
            public string? Note { get; set; }
        }
    }
}
