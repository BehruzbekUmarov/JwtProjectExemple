using Jwt.Application.Interfaces;
using Jwt.Application.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IUserManager _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    public AuthController(IUserManager? userManager,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserDto dto)
    {
        var user = await _userManager.RegisterUser(dto);

        return Ok(user);
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserDto userDto)
    {
        var currentContext = _contextAccessor.HttpContext
            ?? throw ArgumentNullException("Content not found");

        var token = await _userManager
            .LoginUser(userDto, currentContext);

        return Ok(token);
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var currentContext = _contextAccessor.HttpContext
            ?? throw new InvalidDataException(nameof(HttpContext));

        var token = await _userManager
            .GenerateRefreshToken(currentContext);

        return Ok(token);
    }

    [HttpPost("setUserRole")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUserRole(RoleDto dto)
    {
        var userRole = _userManager.SetUserRoleAsync(dto);

        return Ok(userRole);
    }

    private Exception ArgumentNullException(string v)
    {
        throw new NotImplementedException();
    }
}
