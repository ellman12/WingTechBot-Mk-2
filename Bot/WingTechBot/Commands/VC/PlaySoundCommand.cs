namespace WingTechBot.Commands.VC;

public sealed class PlaySoundCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("play-sound")
			.WithDescription("Make WTB play a sound from the soundboard.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the sound")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("amount")
				.WithDescription("How many times to play the sound.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.Integer)
				.WithMinValue(1)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("delay")
				.WithDescription("Delay between each sound. Only used if amount specified.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.String)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var options = command.Data.Options;
		var name = options.Single(o => o.Name == "name").Value as string;
		if (String.IsNullOrWhiteSpace(name))
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		var connection = Bot.VoiceChannelConnection;
		var sound = connection.AvailableSounds.FirstOrDefault(s => String.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase));
		var amount = options.FirstOrDefault(o => o.Name == "amount")?.Value as long? ?? 1;
		var delay = options.FirstOrDefault(o => o.Name == "delay")?.Value as string ?? "1 s";
		if (sound == null)
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		var parsedDelay = ParseTimeSpan(delay);

		string shared = $"\"{sound.Name}\" {(amount > 1 ? $"{amount} times, with a delay of {delay}" : "")}";
		if (Bot.VoiceChannelConnection.ConnectedChannel == null)
		{
			await command.FollowupAsync($"Joining {Bot.DefaultVoiceChannel.Mention} and playing {shared}");
			connection.Connect(Bot.DefaultVoiceChannel);
		}
		else
		{
			await command.FollowupAsync($"Playing {shared}");
		}

		connection.PlaySound(sound.GuildId, sound.SoundId, amount, parsedDelay);
	}

	private static TimeSpan ParseTimeSpan(string input)
	{
		if (String.IsNullOrWhiteSpace(input))
			return TimeSpan.FromSeconds(1);

		var match = Regex.Match(input, @"((?:\d\.)?\d+)\s*(ms?|s|secs?|seconds?|m|mins?|minutes?|h|hr|hours?)", RegexOptions.IgnoreCase);
		if (!match.Success)
			throw new ArgumentException("Invalid time format");

		var value = double.Parse(match.Groups[1].Value);
		string unit = match.Groups[2].Value.ToLower();

		return unit switch
		{
			"ms" => TimeSpan.FromMilliseconds(value),
			"s" or "sec" or "secs" or "second" or "seconds" => TimeSpan.FromSeconds(value),
			"m" or "min" or "mins" or "minute" or "minutes" => TimeSpan.FromMinutes(value),
			"h" or "hr" or "hrs" or "hour" or "hours" => TimeSpan.FromHours(value),
			_ => throw new ArgumentException("Unsupported time unit")
		};
	}
}