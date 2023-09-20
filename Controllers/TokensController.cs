using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using ToDo.Models;
using ToDo.DTOs;

namespace ToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class TokensController : ControllerBase
{
    private readonly ILogger<TokensController> _logger;
    public TokensController(ILogger<TokensController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("")]
    public IActionResult Post([FromBody] DTOs.Login login)
    {   

        var db = new ToDoDbContext();
        var user = db.User.Find(login.Username);
        if (user == null) return Unauthorized(new { msg = "Invalid username or password" });

        string hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: login.Password,
                salt: Convert.FromBase64String(user.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            )
        );
        if (user.Password != hash) return Unauthorized(new { msg = "Invalid username or password" });
        var desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(
            new Claim[] {
            new Claim(ClaimTypes.Name, user.Id),
            new Claim(ClaimTypes.Role, "user")
        });
        desc.Expires = DateTime.UtcNow.AddMinutes(30);
        desc.NotBefore = DateTime.UtcNow;
        desc.IssuedAt = DateTime.UtcNow;
        desc.Issuer = "ToDoApp";
        desc.Audience = "public";
        desc.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecurityKey)),
            SecurityAlgorithms.HmacSha256Signature
        );
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(desc);

        return Ok(new { token = handler.WriteToken(token) });
    }
}