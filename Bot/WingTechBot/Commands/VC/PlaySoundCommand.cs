namespace WingTechBot.Commands.VC;

public sealed class PlaySoundCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("play-sound")
			.WithDescription("Make WTB join a specific VC channel")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the sound")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var name = command.Data.Options.Single(o => o.Name == "name").Value as string;
		if (String.IsNullOrWhiteSpace(name))
		{
			await command.FollowupAsync("Invalid sound name");
			return;
		}

		var sound = Bot.VoiceChannelConnection.Sounds.FirstOrDefault(s => s.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
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
			await command.FollowupAsync($"Playing sound \"{sound.Name}\"");

			var data = new {sound_id = sound.SoundId};
			await Bot.VoiceChannelConnection.Client.PostAsync($"https://discord.com/api/v10/channels/{channel.Id}/send-soundboard-sound", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
		}
	}
}