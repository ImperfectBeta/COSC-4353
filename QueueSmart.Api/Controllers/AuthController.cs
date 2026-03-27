using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // base route is /api/auth
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserStore _userStore;

    public AuthController(IAuthService authService, IUserStore userStore)
    {
        _authService = authService;
        _userStore = userStore;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request)
    {
            if (request == null)
                return BadRequest(new { message = "Request body is required." });

        var (response, error) = _authService.Register(request);

        if (error != null)
            return BadRequest(new { message = error });

        return Ok(response);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        var (response, error) = _authService.Login(request);

        if (error != null)
            return Unauthorized(new { message = error });

        return Ok(response);
    }

    // GET /api/auth/users/{id}
    [HttpGet("users/{id:int}")]
    public ActionResult<UserResponse> GetUser(int id)
    {
        var user = _userStore.GetById(id);
        if (user == null)
            return NotFound();

        return Ok(UserResponse.FromUser(user));
    }

    // GET /api/auth/users
    [HttpGet("users")]
    public ActionResult<IEnumerable<UserResponse>> GetAllUsers()
    {
        var users = _userStore.GetAll().Select(UserResponse.FromUser);
        return Ok(users);
    }
}