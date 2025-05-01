namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public WingTechBot Bot { get; private set; }

	public HttpClient Client { get; } = new();

	public CancellationTokenSource SoundCancelToken { get; private set; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	public IAudioClient AudioClient { get; private set; }

	///Where uses can send soundboard sounds to play. Faster than invoking one-off commands.
	public SocketThreadChannel SoundboardThread { get; private set; }

	///All users connected to <see cref="ConnectedChannel"/> (except the bot).
	public SocketGuildUser[] ConnectedUsers => ConnectedChannel.ConnectedUsers.Where(u => u.Id != Bot.Client.CurrentUser.Id).ToArray();

	public SoundboardSound[] AvailableSounds { get; private set; }

	public List<Task> PlayingSounds { get; } = [];

	private AutoSounds AutoSounds { get; set; }

	public async Task SetUp(WingTechBot bot)
	{
		Logger.LogLine("Setting up VoiceChannelConnection");

		Bot = bot;
		Bot.Client.UserVoiceStateUpdated += VoiceStateUpdated;

		Client.BaseAddress = new Uri("http://localhost:5000/api/");

		SoundboardThread = Bot.Guild.ThreadChannels.FirstOrDefault(t => t.Name == "WTB Soundboard");
		if (SoundboardThread == null)
		{
			SoundboardThread = await Bot.BotChannel.CreateThreadAsync("WTB Soundboard", autoArchiveDuration: ThreadArchiveDuration.OneWeek);
			await SoundboardThread.SendMessageAsync("Send the names of soundboard sounds here to hear them in VC");
		}
		Bot.Client.MessageReceived += OnMessageReceived;

		await GetSounds();

		AutoSounds = new AutoSounds(Bot, this);

		Logger.LogLine("Finish setting up VoiceChannelConnection");
	}

	public void Connect(SocketVoiceChannel channel)
	{
		if (SoundCancelToken.IsCancellationRequested)
			SoundCancelToken = new CancellationTokenSource();

		ConnectedChannel = channel;

		Task.Run(async () =>
		{
			AudioClient = await channel.ConnectAsync(disconnect: true);
		});
	}

	public async Task Disconnect()
	{
		await ConnectedChannel.DisconnectAsync();
		ConnectedChannel = null;
	}

	public void PlaySound(SoundboardSound sound)
	{
		PlaySound(sound, 1, TimeSpan.Zero, TimeSpan.Zero);
	}

	public void PlaySound(SoundboardSound sound, long amount, TimeSpan minDelay, TimeSpan maxDelay)
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

				var data = sound ?? available[Random.Shared.Next(0, available.Length)];
				await connection.Client.PostAsync("soundboard/send-soundboard-sound", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
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

	///Gets or refreshes the list of available sounds.
	public async Task GetSounds()
	{
		Logger.LogLine("Refreshing available soundboard sounds", LogSeverity.Verbose);

		var response = await Client.GetAsync("soundboard/available-sounds");
		var sounds = JsonSerializer.Deserialize<SoundboardSound[]>(await response.Content.ReadAsStringAsync());
		AvailableSounds = sounds.ToArray();

		Logger.LogLine("Finish refreshing available soundboard sounds", LogSeverity.Verbose);
	}

	private async Task<Process> CreateStreamFromBytes(byte[] audio)
	{
		Logger.LogLine("Begin CreateStreamFromBytes, starting FFmpeg process", LogSeverity.Debug);

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "ffmpeg",
				Arguments = "-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			}
		};

		process.Start();

		Logger.LogLine("FFmpeg process started", LogSeverity.Debug);

		//This is almost certainly terrible, but it hangs forever without being wrapped in Task.Run
		_ = Task.Run(async () =>
		{
			Logger.LogLine("Opening FFmpeg stream", LogSeverity.Debug);
			await using var stdin = process.StandardInput.BaseStream;

			Logger.LogLine("Opening FFmpeg stream", LogSeverity.Debug);
			await stdin.WriteAsync(audio, 0, audio.Length, SoundCancelToken.Token);

			Logger.LogLine("Closing FFmpeg stream", LogSeverity.Debug);
			await stdin.FlushAsync(SoundCancelToken.Token);
			stdin.Close();

			Logger.LogLine("FFmpeg stream closed", LogSeverity.Debug);
		}, SoundCancelToken.Token);

		Logger.LogLine("Finish CreateStreamFromBytes", LogSeverity.Debug);
		return process;
	}

	//https://docs.discordnet.dev/guides/voice/sending-voice.html
	public async Task SendAudio(byte[] audio)
	{
		Logger.LogLine("Begin SendAudio", LogSeverity.Debug);

		using var ffmpeg = await CreateStreamFromBytes(audio);
		await using var output = ffmpeg.StandardOutput.BaseStream;
		await using var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed);

		Logger.LogLine("PCM stream created", LogSeverity.Debug);

		try
		{
			Logger.LogLine("Begin sending audio to Discord", LogSeverity.Debug);
			await output.CopyToAsync(discord, SoundCancelToken.Token);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, Bot.BotChannel);
		}
		finally
		{
			Logger.LogLine("Flushing audio stream", LogSeverity.Debug);
			await discord.FlushAsync(SoundCancelToken.Token);
		}
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

		var delay = TimeSpan.FromSeconds(1);
		connection.PlaySound(sound, 1, delay, delay);
		return Task.CompletedTask;
	}

	private static TimeSpan GetRandomTimeSpan(TimeSpan min, TimeSpan max)
	{
		long minTicks = min.Ticks;
		long maxTicks = max.Ticks;
		return new TimeSpan(Random.Shared.NextInt64(minTicks, maxTicks));
	}
}