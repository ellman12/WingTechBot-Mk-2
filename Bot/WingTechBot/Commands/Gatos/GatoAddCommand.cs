namespace WingTechBot.Commands.Gatos;

public sealed class GatoAddCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var gatoAddCommand = new SlashCommandBuilder()
			.WithName("gato-add")
			.WithDescription("Upload an image of a cat, optionally including a name.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("image")
				.WithDescription("Image of the gato")
				.WithType(ApplicationCommandOptionType.Attachment)
				.WithRequired(true)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("Name of the gato")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(false)
			);

		await AddCommand(bot, gatoAddCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await SaveImage(command);
	}

	///Gets the provided image and sends it back so it's stored on Discord.
	private async Task SaveImage(SocketSlashCommand command)
	{
		var options = command.Data.Options;
		Attachment image = options.First(o => o.Name == "image").Value as Attachment;

		if (image == null)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		byte[] imageBytes = await Gato.HttpClient.GetByteArrayAsync(image.Url);

		await using MemoryStream imageStream = new(imageBytes);
		FileAttachment file = new(imageStream, image.Filename);

		var message = await command.FollowupWithFileAsync(file, $"{command.User.Username} uploaded `{file.FileName}`:");

		image = message.Attachments.FirstOrDefault();
		if (image == null)
		{
			await Logger.LogExceptionAsMessage(new NullReferenceException("Missing attachment"), command.Channel);
			return;
		}

		string name = (string)options.FirstOrDefault(o => o.Name == "name")?.Value;
		await Gato.AddGato(image.Url, name?.Trim(), command.User.Id);
	}
}