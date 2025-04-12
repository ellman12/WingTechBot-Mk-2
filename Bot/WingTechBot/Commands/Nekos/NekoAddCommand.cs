namespace WingTechBot.Commands.Nekos;

public sealed class NekoAddCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("neko-add")
			.WithDescription("Upload an image of a neko.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("media")
				.WithDescription("Image of the neko")
				.WithType(ApplicationCommandOptionType.Attachment)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
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

		if (!media.ContentType.StartsWith("image"))
		{
			await command.FollowupAsync("Only images are allowed");
			return;
		}

		var mediaBytes = await Bot.HttpClient.GetByteArrayAsync(media.Url);
		await using MemoryStream mediaStream = new(mediaBytes);
		using FileAttachment file = new(mediaStream, media.Filename);

		await command.FollowupWithFileAsync(file, $"{command.User.Username} uploaded `{file.FileName}`:");

		await Neko.AddNeko(mediaBytes, media.Filename, command.User.Id);
	}
}