using Microsoft.AspNetCore.Mvc;

using static BCrypt.Net.BCrypt;

using LimitlessFit.Data;
using LimitlessFit.Models;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public RegisterController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (_context.Users.Any(user => user.Email == model.Email))
        {
            return BadRequest("Username already exists.");
        } 

        var hashedPassword = HashPassword(model.Password);

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = hashedPassword,
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("Registration successful!");
    }
}