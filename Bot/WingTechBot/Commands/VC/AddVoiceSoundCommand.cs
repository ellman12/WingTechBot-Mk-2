namespace WingTechBot.Commands.VC;

public sealed class AddVoiceSoundCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("add-voice-sound")
			.WithDescription("Add this audio file to WTB's VoiceSounds")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("audio")
				.WithDescription("The file to upload")
				.WithType(ApplicationCommandOptionType.Attachment)
				.WithRequired(true)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the sound")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var options = command.Data.Options;

		if (options.Single(o => o.Name == "audio").Value is not Attachment audio)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		if (!audio.ContentType.StartsWith("audio"))
		{
			await command.FollowupAsync("Only audio is allowed");
			return;
		}

		var name = options.Single(o => o.Name == "name").Value.ToString() ?? throw new NullReferenceException("Missing sound name");
		var audioBytes = await Bot.HttpClient.GetByteArrayAsync(audio.Url);

		await command.FollowupAsync($"{command.User.Username} uploaded `{name}`");

		await VoiceSound.AddVoiceSound(name, audioBytes);
	}
}