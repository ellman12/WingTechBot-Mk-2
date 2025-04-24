namespace WingTechBot.Controllers;

[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
	[HttpPost("login")]
	public IActionResult Login(LoginRequest request)
	{
		if (request.Password == Program.Config.SoundboardPassword)
		{
			return Ok(new {message = "Success"});
		}

		return Unauthorized(new {message = "Invalid password"});
	}
}

public sealed record LoginRequest
{
	[JsonPropertyName("password")]
	public string Password { get; init; }
}