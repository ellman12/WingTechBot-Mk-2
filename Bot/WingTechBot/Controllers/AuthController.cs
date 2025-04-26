namespace WingTechBot.Controllers;

[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
	[HttpPost("login")]
	public IActionResult Login(LoginRequest request)
	{
		using BotDbContext context = new();

		if (context.SoundboardUsers.Any(u => u.Id == request.UserId))
		{
			return Ok(new {message = "Success"});
		}

		return Unauthorized(new {message = "Unauthorized"});
	}

	public readonly record struct LoginRequest
	{
		[JsonPropertyName("user_id")]
		public ulong UserId { get; init; }
	}
}