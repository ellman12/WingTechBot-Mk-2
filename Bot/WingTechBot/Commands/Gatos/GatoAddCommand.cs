namespace WingTechBot.Commands.Gatos;

public sealed class GatoAddCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("gato-add")
			.WithDescription("Upload an image or video of a cat.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("Name of the gato")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("media")
				.WithDescription("Media of the gato")
				.WithType(ApplicationCommandOptionType.Attachment)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await SaveMedia(command);
	}

	///Gets the provided image or video and sends it back so it's stored on Discord.
	private async Task SaveMedia(SocketSlashCommand command)
	{
		var options = command.Data.Options;

		if (options.Single(o => o.Name == "media").Value is not Attachment media)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		var mediaBytes = await Bot.HttpClient.GetByteArrayAsync(media.Url);
		await using MemoryStream mediaStream = new(mediaBytes);
		using FileAttachment file = new(mediaStream, media.Filename);

		await command.FollowupWithFileAsync(file, $"{command.User.Username} uploaded `{file.FileName}`:");

		var name = options.Single(o => o.Name == "name").Value.ToString() ?? throw new NullReferenceException("Missing gato name");
		await Gato.AddGato(mediaBytes, media.Filename, name.Trim(), command.User.Id);
	}
}