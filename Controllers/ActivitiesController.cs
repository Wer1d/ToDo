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
    /// <summary>
    /// get activity by id according to token.userId
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles ="user")] // คนที่เรียก api ต้องมี role user เท่านั้น
    public IActionResult Get()
    {
        // ทำเป็น get based on token ได้
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var db = new ToDoDbContext();
        var activities = db.Activity.Where(a => a.UserId == UserId).ToList();
        if (activities == null) return NotFound();
        return Ok(activities);
    }
    
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles ="user")]
    public IActionResult Get(uint id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var db = new ToDoDbContext();
        var activities = db.Activity.Where(a => a.UserId == UserId).ToList();
        if (activities == null) return NotFound();
        return Ok(activities);
    }

    /// <summary>
    /// need only activityName and when
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles ="user")]
    public IActionResult Post([FromBody] DTOs.Activity data)
    {
        // access curr user id 
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var db = new ToDoDbContext();
        var user = db.User.FirstOrDefault(u => u.Id == UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var activity = new Models.Activity
        {
            Name = data.ActivityName,
            When = data.When,
            UserId = (uint)UserId
        };
        db.Activity.Add(activity);
        db.SaveChanges();
        return Ok(new { id = activity.Id });
    }
    
    
    [Route("{id}")]
    [HttpDelete]
    [Authorize(Roles ="user")]
    public IActionResult Delete(uint id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var db = new ToDoDbContext();
        var activities = db.Activity.Where(a => a.UserId == UserId).ToList();
        foreach (var activity in activities)
        {
            if (activity.Id == id)
            {
                db.Activity.Remove(activity);
                db.SaveChanges();
                return Ok(id + " was deleted successfully");
            }
        }
        return NotFound();
    }   
    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles ="user")]
    public IActionResult Update(uint id, [FromBody] DTOs.Activity data)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var db = new ToDoDbContext();
        var activities = db.Activity.Where(a => a.UserId == UserId).ToList();
        if (activities == null) return NotFound();
        foreach (var activity in activities)
        {
            if (activity.Id == id)
            {
                activity.Name = data.ActivityName;
                activity.When = data.When;
                db.SaveChanges();
                return Ok(id + " was updated successfully");
            }
        }
        return NotFound();
    }


    
}