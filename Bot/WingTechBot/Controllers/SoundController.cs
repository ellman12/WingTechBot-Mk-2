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
		await using BotDbContext context = new();

		var response = await httpClient.GetAsync("soundboard-default-sounds");
		sounds.AddRange(JsonSerializer.Deserialize<SoundboardSound[]>(await response.Content.ReadAsStringAsync()));

		foreach (var id in Program.Config.SoundboardServerIds)
		{
			response = await httpClient.GetAsync($"guilds/{id}/soundboard-sounds");
			string json = await response.Content.ReadAsStringAsync();

			var items = JsonDocument.Parse(json).RootElement.GetProperty("items");
			sounds.AddRange(JsonSerializer.Deserialize<SoundboardSound[]>(items.GetRawText()));
		}

		sounds.AddRange(context.SoundboardSounds);

		//Explicit JSON serialize to ensure the audio byte[] arrays are serialized properly.
		return Ok(JsonSerializer.Serialize(sounds.OrderBy(s => s.Name).ToArray()));
	}

	[HttpPost, Route("send-soundboard-sound")]
	public async Task<IActionResult> SendSound(SoundboardSound sound)
	{
		try
		{
			if (Program.Bot.VoiceChannelConnection.ConnectedChannel == null)
			{
				Program.Bot.VoiceChannelConnection.Connect(Program.Bot.DefaultVoiceChannel);
				await Task.Delay(1250); //Ensures AudioClient has enough time to get a value.
			}

			if (sound.Type == "soundboard")
			{
				var soundData = new SoundPostData(sound);
				var content = new StringContent(JsonSerializer.Serialize(soundData), Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync($"channels/{Program.Bot.VoiceChannelConnection.ConnectedChannel!.Id}/send-soundboard-sound", content);

				if (response.IsSuccessStatusCode)
					return Ok();

				var errorMessage = await response.Content.ReadAsStringAsync();
				return StatusCode((int) response.StatusCode, new {Error = errorMessage});
			}

			await Program.Bot.VoiceChannelConnection.SendAudio(sound.Audio);
			return Ok();
		}
		catch (Exception e)
		{
			return StatusCode(500, new {Error = e.Message});
		}
	}

	private readonly record struct SoundPostData(SoundboardSound sound)
	{
		public ulong? source_guild_id { get; } = sound.GuildId;

		public ulong sound_id { get; } = sound.Id;
	}
}