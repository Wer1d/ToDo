using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using ToDo.DTOs;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize(Roles ="user")]
    public IActionResult Get()
    {
        var db = new ToDoDbContext();
        var activities = from x in db.Activity select x;
        if (!activities.Any()) return NoContent();
        return Ok(activities);
    }

    [HttpPost]
    [Authorize(Roles ="user")]
    public IActionResult Post([FromBody] DTOs.Activity data)
    {
        var db = new ToDoDbContext();
        var activity = new Models.Activity();
        activity.Name = data.Name;
        activity.When = data.When;
        db.Activity.Add(activity);
        db.SaveChanges();
        return Ok();
    }
    [Route("{id}")]
    [HttpDelete]
    public IActionResult Delete(uint id)
    {
        var db = new ToDoDbContext();
        var activity = db.Activity.Find(id);
        db.Activity.Remove(activity);
        db.SaveChanges();
        return Ok();
    }   
    
    public IActionResult Update(uint id)
    {
        var db = new ToDoDbContext();
        var activity = db.Activity.Find(id);
        db.Activity.Remove(activity);
        db.SaveChanges();
        return Ok();
    }


    
}