using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using ToDo.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace ToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("{id}")]
    public IActionResult Get(uint id)
    {
        var db = new ToDoDbContext();
        var user = db.User.Find(id);
        if (user == null) return NotFound("Not found");
        return Ok(user);
    }

    /// <summary>Get all users </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get()
    {
        var db = new ToDoDbContext();
        var users = db.User.ToList();
        if (!users.Any()) return NoContent();
        return Ok(users);
    }
    
    /// <summary>Add new use</summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post([FromBody] DTOs.UserDTO data)
    {   
        if (data == null) return BadRequest();  
    
        var db = new ToDoDbContext();
        var salt = CreateSalt();
        var hash = GenerateHash( salt,data.Password);
        var newUser = new Models.User{
            Username = data.Username,
            Password = hash,
            Salt = salt
        };
        db.User.Add(newUser);
        db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, "User registered successfully.");
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

   [HttpPut("{id}")]
    public IActionResult Update(uint id,[FromBody] DTOs.UserDTO data)
    {
        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound("user not found");
        var hash = GenerateHash(user.Salt,data.Password);
        user.Username = data.Username;
        user.Password = hash;
        db.SaveChanges();
        return Ok("user updated successfully");
    }
    [HttpDelete("{id}")]

    public IActionResult Delete(uint id)
    {
        var db = new ToDoDbContext();
        var user = db.User.Find(id);
        if (user == null) return NotFound();
        db.User.Remove(user);
        db.SaveChanges();
        return Ok("user deleted successfully");
     
    }

    
}