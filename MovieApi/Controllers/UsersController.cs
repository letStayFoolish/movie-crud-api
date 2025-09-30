using Microsoft.AspNetCore.Mvc;
using MovieApi.DTOs.Users;
using MovieApi.Models;
using MovieApi.Services.Users;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;

    public UsersController(IUsersService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        await _service.RegisterAsync(model);
        return Created();
    }


    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
    {
        var result = await _service.AddRoleAsync(model);
        return Ok(result);
    }
}
