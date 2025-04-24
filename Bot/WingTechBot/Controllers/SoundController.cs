namespace WingTechBot.Controllers;

[ApiController, Route("api/soundboard")]
public sealed class SoundController : ControllerBase
{
	private readonly HttpClient httpClient;

	public SoundController()
	{
		httpClient = new HttpClient();
		httpClient.BaseAddress = new Uri("https://discord.com/api/v10/");
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", Program.Config.LoginToken);
	}

	[HttpGet, Route("available-sounds")]
	public async Task<IActionResult> AvailableSounds()
	{
		var sounds = new List<SoundboardSound>();

		foreach (var id in Program.Config.SoundboardServerIds)
		{
			var response = await httpClient.GetAsync($"guilds/{id}/soundboard-sounds");
			string json = await response.Content.ReadAsStringAsync();

			var items = JsonDocument.Parse(json).RootElement.GetProperty("items");
			sounds.AddRange(JsonSerializer.Deserialize<SoundboardSound[]>(items.GetRawText()));
		}

		return Ok(sounds);
	}
}