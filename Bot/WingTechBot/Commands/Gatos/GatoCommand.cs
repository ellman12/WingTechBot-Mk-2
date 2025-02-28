using System.Globalization;

namespace WingTechBot.Commands.Gatos;

public sealed class GatoCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("gato")
			.WithDescription("Sends a random picture of any cat, or one with the name specified.")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("Name of the gato")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await SendRandomMedia(command);
	}

	private async Task SendRandomMedia(SocketSlashCommand command)
	{
		await using BotDbContext context = new();
		var name = (string)command.Data.Options.FirstOrDefault()?.Value;
		name = name?.ToLower();

		Gato gato = await context.Gatos.OrderBy(_ => EF.Functions.Random()).FirstOrDefaultAsync(g => String.IsNullOrWhiteSpace(name) ? g.Id > 0 : g.Name == name);
		if (gato == null)
		{
			await command.FollowupAsync("No gatos found");
			return;
		}

		await using MemoryStream mediaStream = new(gato.Media);
		using FileAttachment file = new(mediaStream, $"{gato.Filename}");

		await command.FollowupWithFileAsync(file, String.IsNullOrWhiteSpace(gato.Name) ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gato.Name));
	}
}