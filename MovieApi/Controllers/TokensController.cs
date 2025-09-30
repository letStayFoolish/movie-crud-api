// This file is part of the project. Copyright (c) Company.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.DTOs.Auth;
using MovieApi.Models;
using MovieApi.Services.AuthCookie;
using MovieApi.Services.Users;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokensController : ControllerBase
{
    private readonly IUsersService _service;
    private readonly IAuthCookieService _authCookieService;

    public TokensController(IUsersService service, IAuthCookieService authCookieService)
    {
        _service = service;
        _authCookieService = authCookieService;
    }

    [HttpPost]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
    {
        var result = await _service.GetTokenAsync(model);
        _authCookieService.SetRefreshTokenInCookie(result.RefreshToken, 10);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _service.RefreshTokenAsync(refreshToken);
        if (!string.IsNullOrEmpty(response.RefreshToken))
        {
            _authCookieService.SetRefreshTokenInCookie(response.RefreshToken, 10);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    [Route("{id}")]
    public IActionResult GetRefreshTokens(string id)
    {
        var user = _service.GetById(id);
        if (user is null)
        {
            return NotFound($"User with id {id} not found.");
        }

        return Ok(user.RefreshTokens);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
    {
        // accept token from request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Token is required." });
        }

        var response = await _service.RevokeTokenAsync(token);

        if (!response)
        {
            return NotFound(new { message = "Token not found." });
        }

        return Ok(new { message = "Token revoked" });
    }
}
