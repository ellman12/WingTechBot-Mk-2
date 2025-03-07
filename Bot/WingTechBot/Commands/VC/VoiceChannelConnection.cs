namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public WingTechBot Bot { get; private set; }

	public Task Connection { get; private set; }

	public HttpClient Client { get; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	public SoundboardSound[] Sounds { get; private set; }

	public async Task SetUp(WingTechBot bot)
	{
		Bot = bot;

		Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Bot.Config.LoginToken}");

		Sounds = await GetSounds();
	}

	public void Connect(SocketVoiceChannel channel)
	{
		ConnectedChannel = channel;
		Connection = channel.ConnectAsync(disconnect: true);
	}

	public async Task Disconnect(SocketVoiceChannel channel)
	{
		ConnectedChannel = null;
		await channel.DisconnectAsync();
	}

	private async Task<SoundboardSound[]> GetSounds()
	{
		var response = await Client.GetAsync("https://discord.com/api/v10/soundboard-default-sounds");
		var defaultSounds = JsonSerializer.Deserialize<SoundboardSound[]>(await response.Content.ReadAsStringAsync());

		response = await Client.GetAsync($"https://discord.com/api/v10/guilds/{Bot.Guild.Id}/soundboard-sounds");
		string json = await response.Content.ReadAsStringAsync();

		var items = JsonDocument.Parse(json).RootElement.GetProperty("items");
		var serverSounds = JsonSerializer.Deserialize<SoundboardSound[]>(items.GetRawText());

		return defaultSounds.Concat(serverSounds).ToArray();
	}
}