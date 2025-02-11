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
		Attachment media = options.First(o => o.Name == "media").Value as Attachment;

		if (media == null)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		byte[] mediaBytes = await Gato.HttpClient.GetByteArrayAsync(media.Url);
		await using MemoryStream mediaStream = new(mediaBytes);
		FileAttachment file = new(mediaStream, media.Filename);

		var message = await command.FollowupWithFileAsync(file, $"{command.User.Username} uploaded `{file.FileName}`:");

		media = message.Attachments.FirstOrDefault();
		if (media == null)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		var name = options.First(o => o.Name == "name").Value.ToString() ?? throw new NullReferenceException("Missing gato name");
		await Gato.AddGato(media.Url, name.Trim(), command.User.Id);
	}
}