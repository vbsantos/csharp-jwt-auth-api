namespace Jwt2WebApi;

public class User
{
  public required string Username { get; set; }
  public required string PasswordHash { get; set; }
}

public class UserViewModel
{
  public required string Username { get; set; }
  public required string Password { get; set; }
}
