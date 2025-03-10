namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public WingTechBot Bot { get; private set; }

	public HttpClient Client { get; } = new();

	public CancellationTokenSource SoundCancelToken { get; private set; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	///All users connected to <see cref="ConnectedChannel"/> (except the bot).
	public SocketGuildUser[] ConnectedUsers => ConnectedChannel.ConnectedUsers.Where(u => u.Id != Bot.Client.CurrentUser.Id).ToArray();

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
		if (SoundCancelToken.IsCancellationRequested)
			SoundCancelToken = new CancellationTokenSource();

		ConnectedChannel = channel;
		_ = channel.ConnectAsync(disconnect: true);
	}

	public async Task Disconnect()
	{
		await ConnectedChannel.DisconnectAsync();
		ConnectedChannel = null;
	}

	public void PlaySound(string guildId, string soundId, long amount, TimeSpan delay)
	{
		var data = new {source_guild_id = guildId, sound_id = soundId};

		if (SoundCancelToken.IsCancellationRequested)
			SoundCancelToken = new CancellationTokenSource();

		PlayingSounds.Add(Task.Run(async () =>
		{
			for (long i = 0; i < amount; i++)
			{
				if (SoundCancelToken.Token.IsCancellationRequested)
					return;

				await Bot.VoiceChannelConnection.Client.PostAsync($"https://discord.com/api/v10/channels/{Bot.VoiceChannelConnection.ConnectedChannel.Id}/send-soundboard-sound", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
				await Task.Delay(delay, SoundCancelToken.Token);
			}
		}, SoundCancelToken.Token));
	}

	///Cancels all the currently-playing sounds and resets the cancellation token.
	public async Task CancelSounds()
	{
		await SoundCancelToken.CancelAsync();
		PlayingSounds.Clear();
	}

	private async Task<SoundboardSound[]> GetSounds()
	{
		var response = await Client.GetAsync("https://discord.com/api/v10/soundboard-default-sounds");
		var sounds = JsonSerializer.Deserialize<SoundboardSound[]>(await response.Content.ReadAsStringAsync()).ToList();

		foreach (var guild in Bot.Client.Guilds)
		{
			response = await Client.GetAsync($"https://discord.com/api/v10/guilds/{guild.Id}/soundboard-sounds");
			string json = await response.Content.ReadAsStringAsync();

			var items = JsonDocument.Parse(json).RootElement.GetProperty("items");
			sounds.AddRange(JsonSerializer.Deserialize<SoundboardSound[]>(items.GetRawText()));
		}

		return sounds.ToArray();
	}

	private async Task VoiceStateUpdated(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
	{
		if (ConnectedChannel != null && !ConnectedUsers.Any())
		{
			await Disconnect();
			return;
		}

		if (user.Id != Bot.Client.CurrentUser.Id)
			return;

		ConnectedChannel = current.VoiceChannel;
		if (ConnectedChannel == null)
			await CancelSounds();
	}
}