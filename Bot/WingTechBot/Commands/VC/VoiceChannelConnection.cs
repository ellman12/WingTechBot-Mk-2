namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public WingTechBot Bot { get; private set; }

	public HttpClient Client { get; } = new();

	public CancellationTokenSource SoundCancelToken { get; private set; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	public SoundboardSound[] AvailableSounds { get; private set; }

	public List<Task> PlayingSounds { get; } = [];

	public async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.UserVoiceStateUpdated += VoiceStateUpdated;

		Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Bot.Config.LoginToken}");

		AvailableSounds = await GetSounds();
	}

	public void Connect(SocketVoiceChannel channel)
	{
		ConnectedChannel = channel;
		_ = channel.ConnectAsync(disconnect: true);
	}

	public async Task Disconnect(SocketVoiceChannel channel)
	{
		ConnectedChannel = null;
		await channel.DisconnectAsync();
	}

	///Cancels all the currently-playing sounds and resets the cancellation token.
	public async Task CancelSounds()
	{
		await SoundCancelToken.CancelAsync();
		await Task.Delay(2000);
		PlayingSounds.Clear();
		SoundCancelToken = new CancellationTokenSource();
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

	private async Task VoiceStateUpdated(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
	{
		if (user.Id != Bot.Client.CurrentUser.Id)
			return;

		ConnectedChannel = current.VoiceChannel;
		if (ConnectedChannel == null)
			await CancelSounds();
	}
}
