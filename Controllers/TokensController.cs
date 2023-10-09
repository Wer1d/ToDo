using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using ToDo.Models;
using ToDo.DTOs;
using System.Security.Cryptography;
using Swashbuckle.AspNetCore.Filters;

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

    /// <summary>
    ///  Generate Token by giving username and password
    /// </summary>
    /// <param name="request" >Hi</param>
    /// <example>Cloudy with a chance of rain</example>

    /// <returns></returns>
    [HttpPost]
    [Route("")]
        public IActionResult Post([FromBody] DTOs.Login login)
    {   

        var db = new ToDoDbContext();

        // primary key is Id of user that why we cant use Find()
        var users = db.User.ToList();
        var user = null as Models.User;
        foreach (var tempUser in users)
        {
            if (tempUser.Username == login.UserName)
            {
                user = tempUser;
                break;
            }
        }
        if (user == null) return Unauthorized(new { msg = "Invalid username or password" });

        string hash = GenerateHash(user.Salt, login.Password);
        if (user.Password != hash) return Unauthorized(new { msg = "Invalid username or password" });
        var desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(
            new Claim[] {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        return Ok(new { token = handler.WriteToken(token) , role = "user"});
    }
    
    private string CreateSalt()
    {
        byte[] randomBytes = new byte[128 / 8];
        var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
        
    }

    private string GenerateHash(string salt, string password)
    {
        string hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            )
        );
        return hash;
    }
}
