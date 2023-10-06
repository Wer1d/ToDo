using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using ToDo.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;
namespace ToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitesController : ControllerBase
{
    private readonly ILogger<ActivitesController> _logger;
    public ActivitesController(ILogger<ActivitesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles ="user")] // คนที่เรียก api ต้องมี role user เท่านั้น
    public IActionResult Get()
    {
        // ทำเป็น get based on token ได้
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var Id = null as int?;
        if (identity != null)
        {
            Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // use userId here
        }

        var db = new ToDoDbContext();
        var activities = db.Activity.Where(a => a.Id == Id).ToList();
        if (!activities.Any()) return NoContent();
        return Ok(activities);
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles ="user")]
    public IActionResult Get(uint id)
    {
        var db = new ToDoDbContext();
        var activity = db.Activity.Find(id);
        if (activity == null) return NotFound();
        return Ok(activity);
    }

    [HttpPost]
    [Authorize(Roles ="user")]
    public IActionResult Post([FromBody] DTOs.Activity data)
    {
        // access curr user id 
        
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = null as int?;
        if (identity != null)
        {
            UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // use userId here
        }

        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Id == UserId);
        if (user != null)
        {
            return NotFound("User not found");
        }
        var activity = new Models.Activity();
        activity.Name = data.ActivityName;
        activity.When = data.When;
        activity.UserId = (uint)UserId;
        db.Activity.Add(activity);
        db.SaveChanges();
        return Ok(new { id = activity.UserId });
    }

    [Route("{id}")]
    [HttpDelete]
    [Authorize(Roles ="user")]
    public IActionResult Delete(uint id)
    {
        var db = new ToDoDbContext();
        var activity = db.Activity.Find(id);
        if (activity == null) return NotFound();
        db.Activity.Remove(activity);
        db.SaveChanges();
        return Ok();
    }   
    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles ="user")]
    public IActionResult Update(uint id, [FromBody] DTOs.Activity data)
    {
        var db = new ToDoDbContext();
        var activity = db.Activity.Find(id);
        if (activity == null) return NotFound();
        activity.Name = data.ActivityName;
        activity.When = data.When;
        db.SaveChanges();
        return Ok();
    }


    
}