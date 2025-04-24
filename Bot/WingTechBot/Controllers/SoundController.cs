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

	[HttpPost, Route("{id}/send-soundboard-sound")]
	public async Task<IActionResult> SendSound(ulong id, SoundboardSound sound)
	{
		var soundData = new SoundPostData(sound);
		var content = new StringContent(JsonSerializer.Serialize(soundData), Encoding.UTF8, "application/json");

		try
		{
			var response = await httpClient.PostAsync($"channels/{Program.Config.DefaultVoiceChannelId}/send-soundboard-sound", content);

			if (response.IsSuccessStatusCode)
				return Ok();

			var errorMessage = await response.Content.ReadAsStringAsync();
			return StatusCode((int)response.StatusCode, new { Error = errorMessage });
		}
		catch (Exception e)
		{
			return StatusCode(500, new { Error = e.Message });
		}
	}

	private readonly record struct SoundPostData(SoundboardSound sound)
	{
		public ulong? source_guild_id { get; } = sound.GuildId;

		public ulong sound_id { get; } = sound.SoundId;
	}
}