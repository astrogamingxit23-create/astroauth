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
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetAdminId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            var adminId = GetAdminId();
            var apps = await _context.Applications
                .Where(a => a.AdminId == adminId)
                .Select(a => new { a.Id, a.Name, a.AppSecret, a.CreatedAt })
                .ToListAsync();
            return Ok(apps);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] string name)
        {
            var adminId = GetAdminId();
            var app = new Application
            {
                Id = Guid.NewGuid(),
                Name = name,
                AdminId = adminId
            };

            _context.Applications.Add(app);
            await _context.SaveChangesAsync();

            return Ok(app);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
        {
            var adminId = GetAdminId();
            var app = await _context.Applications.FirstOrDefaultAsync(a => a.Id == id && a.AdminId == adminId);

            if (app == null) return NotFound();

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
