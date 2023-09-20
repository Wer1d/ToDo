using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using ToDo.DTOs;

namespace ToDo.Controllers;

[ApiController]
[Route("[users]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get(uint id)
    {
        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound("Not found");
        return Ok(user);
    }

    public IActionResult GetAll()
    {
        var db = new ToDoDbContext();
        var users = db.User.ToList();
        if (!users.Any()) return NoContent();
        return Ok(users);
    }

    [HttpPost]
    [Route("register")]
    public IActionResult Post([FromBody] DTOs.Login data)
    {   
        if (data == null) return BadRequest();  
    
        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Username == data.Username);
        if (user != null) return Conflict(new { msg = "Username already exists" });

        var newUser = new Models.User{
            Username = data.Username,
            Password = data.Password
        };
        db.User.Add(newUser);
        db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, "User registered successfully.");

    }
    

    [HttpPut]
    public IActionResult Update(uint id,[FromBody] DTOs.Login data)
    {
        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound("user not found");
        user.Username = data.Username;
        user.Password = data.Password;
        db.SaveChanges();
        return Ok("user updated successfully");
    }
    [HttpDelete]
    public IActionResult Delete(uint id)
    {
        var db = new ToDoDbContext();
        var user = db.User.Find(id);
        if (user == null) return NotFound("user not found");

        db.User.Remove(user);
        db.SaveChanges();
        return Ok("user deleted successfully");
     
    }

    
}