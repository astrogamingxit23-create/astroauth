using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(ClientRegisterRequest request)
        {
            var app = await _context.Applications.FirstOrDefaultAsync(a => a.Id == request.AppId && a.AppSecret == request.AppSecret);
            if (app == null) return BadRequest("Aplicativo inválido ou segredo incorreto.");

            var license = await _context.Licenses.FirstOrDefaultAsync(l => l.Key == request.LicenseKey && l.ApplicationId == request.AppId);
            if (license == null) return BadRequest("Licença inválida.");
            if (license.IsUsed) return BadRequest("Licença já utilizada.");

            if (await _context.SoftwareUsers.AnyAsync(u => u.Username == request.Username && u.ApplicationId == request.AppId))
                return BadRequest("Usuário já existe para este aplicativo.");

            var user = new SoftwareUser
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                HWID = request.HWID,
                ApplicationId = request.AppId,
                ExpiryDate = DateTime.UtcNow.AddDays(license.DurationDays)
            };

            license.IsUsed = true;
            license.UsedAt = DateTime.UtcNow;
            license.ExpiresAt = user.ExpiryDate;

            _context.SoftwareUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registrado com sucesso!", expiry = user.ExpiryDate });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(ClientLoginRequest request)
        {
            var app = await _context.Applications.FirstOrDefaultAsync(a => a.Id == request.AppId && a.AppSecret == request.AppSecret);
            if (app == null) return BadRequest("Aplicativo inválido.");

            var user = await _context.SoftwareUsers.FirstOrDefaultAsync(u => u.Username == request.Username && u.ApplicationId == request.AppId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos.");

            if (user.ExpiryDate < DateTime.UtcNow)
                return BadRequest("Sua assinatura expirou.");

            if (!string.IsNullOrEmpty(user.HWID) && user.HWID != request.HWID)
                return BadRequest("HWID não corresponde.");

            user.LastLogin = DateTime.UtcNow;
            user.IP = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Login bem-sucedido!", 
                expiry = user.ExpiryDate,
                username = user.Username
            });
        }

        public class ClientRegisterRequest
        {
            public Guid AppId { get; set; }
            public string AppSecret { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string LicenseKey { get; set; } = string.Empty;
            public string? HWID { get; set; }
        }

        public class ClientLoginRequest
        {
            public Guid AppId { get; set; }
            public string AppSecret { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? HWID { get; set; }
        }
    }
}
