namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public WingTechBot Bot { get; private set; }

	public HttpClient Client { get; } = new();

	public CancellationTokenSource SoundCancelToken { get; private set; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	///Where uses can send soundboard sounds to play. Faster than invoking one-off commands.
	public SocketThreadChannel SoundboardThread { get; private set; }

	///All users connected to <see cref="ConnectedChannel"/> (except the bot).
	public SocketGuildUser[] ConnectedUsers => ConnectedChannel.ConnectedUsers.Where(u => u.Id != Bot.Client.CurrentUser.Id).ToArray();

	public SoundboardSound[] AvailableSounds { get; private set; }

	public List<Task> PlayingSounds { get; } = [];

	public async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.UserVoiceStateUpdated += VoiceStateUpdated;

		SoundboardThread = Bot.Guild.ThreadChannels.FirstOrDefault(t => t.Name == "WTB Soundboard");
		if (SoundboardThread == null)
		{
			SoundboardThread = await Bot.BotChannel.CreateThreadAsync("WTB Soundboard", autoArchiveDuration: ThreadArchiveDuration.OneWeek);
			await SoundboardThread.SendMessageAsync("Send the names of soundboard sounds here to hear them in VC");
		}
		Bot.Client.MessageReceived += OnMessageReceived;

		Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Bot.Config.LoginToken}");

		await GetSounds();
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

	public void PlaySound(long amount, TimeSpan minDelay, TimeSpan maxDelay, SoundboardSound sound)
	{
		if (SoundCancelToken.IsCancellationRequested)
			SoundCancelToken = new CancellationTokenSource();

		PlayingSounds.Add(Task.Run(async () =>
		{
			var connection = Bot.VoiceChannelConnection;
			var available = connection.AvailableSounds;

			for (long i = 0; i < amount; i++)
			{
				if (SoundCancelToken.Token.IsCancellationRequested)
					return;

				var data = new SoundPostData(sound ?? available[Random.Shared.Next(0, available.Length)]);
				await connection.Client.PostAsync($"https://discord.com/api/v10/channels/{connection.ConnectedChannel.Id}/send-soundboard-sound", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
				var delay = GetRandomTimeSpan(minDelay, maxDelay);
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

	private sealed class SoundPostData(SoundboardSound sound)
	{
		public string source_guild_id { get; set; } = sound.GuildId;

		public string sound_id { get; set; } = sound.SoundId;
	}

	///Gets or refreshes the list of available sounds.
	public async Task GetSounds()
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

		AvailableSounds = sounds.ToArray();
	}

	private async Task VoiceStateUpdated(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
	{
		if (current.VoiceChannel != ConnectedChannel && ConnectedChannel != null && !ConnectedUsers.Any())
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

	private Task OnMessageReceived(SocketMessage message)
	{
		if (message.Channel.Id != SoundboardThread.Id)
			return Task.CompletedTask;

		var connection = Bot.VoiceChannelConnection;
		var sound = connection.AvailableSounds.FirstOrDefault(s => String.Equals(s.Name, message.Content, StringComparison.InvariantCultureIgnoreCase));
		if (sound == null && !message.Content.ToLower().StartsWith("rand"))
			return Task.CompletedTask;

		if (Bot.VoiceChannelConnection.ConnectedChannel == null)
			connection.Connect(Bot.DefaultVoiceChannel);

		var delay = TimeSpan.FromSeconds(1);
		connection.PlaySound(1, delay, delay, sound);
		return Task.CompletedTask;
	}

	private static TimeSpan GetRandomTimeSpan(TimeSpan min, TimeSpan max)
	{
		long minTicks = min.Ticks;
		long maxTicks = max.Ticks;
		return new TimeSpan(Random.Shared.NextInt64(minTicks, maxTicks));
	}
}