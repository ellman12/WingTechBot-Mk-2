namespace WingTechBot.Commands.VC;

public sealed class SoundCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("sound")
			.WithDescription("Make WTB play a sound from the soundboard.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the sound, or none for random.")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(false)
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
				.WithDescription("Constant delay between each sound. Only used if amount specified.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.String)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("min-delay")
				.WithDescription("Min delay between each sound. Only used if amount specified.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.String)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("max-delay")
				.WithDescription("Max delay between each sound. Only used if amount specified.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.String)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var connection = Bot.VoiceChannelConnection;
		TimeSpan parsedMin, parsedMax;

		var options = command.Data.Options;
		var amount = options.SingleOrDefault(o => o.Name == "amount")?.Value as long? ?? 1;
		var delay = options.SingleOrDefault(o => o.Name == "delay")?.Value as string;
		var minDelay = options.SingleOrDefault(o => o.Name == "min-delay")?.Value as string;
		var maxDelay = options.SingleOrDefault(o => o.Name == "max-delay")?.Value as string;

		if (!String.IsNullOrWhiteSpace(delay))
		{
			parsedMin = parsedMax = ParseTimeSpan(delay);
		}
		else if (!String.IsNullOrWhiteSpace(minDelay) && !String.IsNullOrWhiteSpace(maxDelay))
		{
			parsedMin = ParseTimeSpan(minDelay);
			parsedMax = ParseTimeSpan(maxDelay);
			if (parsedMin > parsedMax)
			{
				await command.FollowupAsync("Min delay cannot be greater than max delay");
				return;
			}
		}
		else
		{
			delay = "1 s";
			parsedMin = parsedMax = TimeSpan.FromSeconds(1);
		}

		var name = options.SingleOrDefault(o => o.Name == "name")?.Value as string;
		var sound = connection.AvailableSounds.FirstOrDefault(s => String.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase));
		if (!String.IsNullOrWhiteSpace(name) && sound == null)
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		string delayMsg = $"with a {(String.IsNullOrWhiteSpace(delay) ? $"random delay between {minDelay} and {maxDelay}" : $"delay of {delay}")}";
		string shared = $"{(sound == null ? "a random sound" : $"\"{sound.Name}\"")} {(amount > 1 ? $"{amount} times, {delayMsg}" : "")}";
		if (connection.ConnectedChannel == null)
		{
			await command.FollowupAsync($"Joining {Bot.DefaultVoiceChannel.Mention} and playing {shared}");
		}
		else
		{
			await command.FollowupAsync($"Playing {shared}");
		}

		connection.PlaySound(sound, amount, parsedMin, parsedMax);
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