namespace Jwt2WebApi.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BCrypt.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
  private readonly IConfiguration _configuration;
  private static readonly List<User> RegistredUsers = new();

  public AuthController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpPost("Register")]
  public IActionResult Register(UserViewModel request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 4)
      return BadRequest("Invalid Username.");

    if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
      return BadRequest("Invalid Password.");

    if (RegistredUsers.Any(user => user.Username == request.Username))
      return BadRequest("Username is already taken.");

    var newUser = new User
    {
      Username = request.Username,
      PasswordHash = BCrypt.HashPassword(request.Password),
      IsAdmin = false
    };
    RegistredUsers.Add(newUser);

    return Ok();
  }

  [HttpPost("RegisterAdmin")]
  public IActionResult RegisterAdmin(UserViewModel request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 4)
      return BadRequest("Invalid Username.");

    if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
      return BadRequest("Invalid Password.");

    if (RegistredUsers.Any(user => user.Username == request.Username))
      return BadRequest("Username is already taken.");

    var newUser = new User
    {
      Username = request.Username,
      PasswordHash = BCrypt.HashPassword(request.Password),
      IsAdmin = true
    };
    RegistredUsers.Add(newUser);

    return Ok();
  }

  [HttpPost("Login")]
  public IActionResult Login(UserViewModel request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 4)
      return BadRequest("Invalid Username.");

    if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
      return BadRequest("Invalid Password.");

    var user = RegistredUsers.Find(user => user.Username == request.Username);
    if (user is null)
      return NotFound("Username doesn't exist.");

    if (!BCrypt.Verify(request.Password, user.PasswordHash))
      return Unauthorized();

    var token = GenerateJwtToken(user.Username, user.IsAdmin);

    Response.Cookies.Append("access_token", token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict
    });

    return Ok();
  }

  [HttpPost("LoginApi")]
  public IActionResult LoginApi(UserViewModel request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 4)
      return BadRequest("Invalid Username.");

    if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
      return BadRequest("Invalid Password.");

    var user = RegistredUsers.Find(user => user.Username == request.Username);
    if (user is null)
      return NotFound("Username doesn't exist.");

    if (!BCrypt.Verify(request.Password, user.PasswordHash))
      return Unauthorized();

    var token = GenerateJwtToken(user.Username, user.IsAdmin);

    return Ok(new { Token = token });
  }

  [HttpGet("Users"), Authorize(Roles = "Admin")]
  public IActionResult GetDefaultUsers()
  {
    return Ok(RegistredUsers.Where(user => !user.IsAdmin));
  }
  
  private string GenerateJwtToken(string username, bool isAdmin)
  {
    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, username),
      new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "Default")
    };

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)
    );

    var creds = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256
    );

    var expires = DateTime.Now.AddDays(
      Convert.ToDouble(_configuration["JwtSettings:ExpireDays"])
    );

    var token = new JwtSecurityToken(
      _configuration["JwtSettings:Issuer"],
      _configuration["JwtSettings:Audience"],
      claims,
      expires: expires,
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
