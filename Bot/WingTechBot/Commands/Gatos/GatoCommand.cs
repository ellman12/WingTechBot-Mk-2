namespace WingTechBot.Commands.Gatos;

public sealed class GatoCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		var gatoCommand = new SlashCommandBuilder()
			.WithName("gato")
			.WithDescription("Sends a random picture of any cat, or one with the name specified.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("Name of the gato")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(false)
			);

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(gatoCommand.Build());
		}
		catch (Exception e)
		{
			Logger.LogLine("Error adding gato command");
			Logger.LogException(e);
		}
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != "gato")
			return;

		await SendRandomImage(command);
	}

	private async Task SendRandomImage(SocketSlashCommand command)
	{
		await using BotDbContext context = new();
		string name = (string)command.Data.Options.FirstOrDefault()?.Value;

		Gato gato = await context.Gatos.OrderBy(_ => EF.Functions.Random()).FirstOrDefaultAsync(g => String.IsNullOrWhiteSpace(name) ? g.Id > 0 : g.Name == name);

		if (gato == null)
		{
			await command.FollowupAsync("No gatos found");
			return;
		}

		if (String.IsNullOrWhiteSpace(gato.Name))
		{
			await command.FollowupAsync(gato.Url);
		}
		else
		{
			//Less efficient but necessary. Trying to send the name and url in the same message causes an ugly filename to appear between the name and image.
			//However, sending it as an actual file doesn't... 🤷‍♂️
			byte[] imageBytes = await Gato.HttpClient.GetByteArrayAsync(gato.Url);
			string filename = Path.GetFileName(new Uri(gato.Url).LocalPath);

			await using MemoryStream imageStream = new(imageBytes);
			FileAttachment file = new(imageStream, filename);

			await command.FollowupWithFileAsync(file, gato.Name);
		}
	}
}