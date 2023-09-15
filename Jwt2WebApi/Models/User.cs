namespace Jwt2WebApi;

public class User
{
  public required string Username { get; set; }
  public required string PasswordHash { get; set; }
  public required bool IsAdmin { get; set; }
}

public class UserViewModel
{
  public required string Username { get; set; }
  public required string Password { get; set; }
}
