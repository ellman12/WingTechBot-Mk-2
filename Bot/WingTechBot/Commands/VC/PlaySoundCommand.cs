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
				.WithDescription("Delay in ms between each sound. Only used if amount specified.")
				.WithRequired(false)
				.WithType(ApplicationCommandOptionType.Integer)
				.WithMinValue(100)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var options = command.Data.Options;
		var name = options.Single(o => o.Name == "name").Value as string;
		if (String.IsNullOrWhiteSpace(name))
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		var sound = Bot.VoiceChannelConnection.Sounds.FirstOrDefault(s => s.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
		var amount = options.FirstOrDefault(o => o.Name == "amount")?.Value as long? ?? 1;
		var delay = options.FirstOrDefault(o => o.Name == "delay")?.Value as long? ?? 1000;
		if (sound == null)
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		var channel = Bot.VoiceChannelConnection.ConnectedChannel;
		if (channel == null)
		{
			await command.FollowupAsync("Bot not in VC");
		}
		else
		{
			await command.FollowupAsync($"Playing sound \"{sound.Name}\" {(amount > 1 ? $"{amount} times, with delay of {delay} ms" : "")}");
			var data = new {sound_id = sound.SoundId};

			_ = Task.Run(async () =>
			{
				for (int i = 0; i < amount; i++)
				{
					await Bot.VoiceChannelConnection.Client.PostAsync($"https://discord.com/api/v10/channels/{channel.Id}/send-soundboard-sound", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
					await Task.Delay(TimeSpan.FromMilliseconds(delay));
				}
			});
		}
	}
}