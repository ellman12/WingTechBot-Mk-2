namespace WingTechBot.Commands.Nekos;

public sealed class NekoCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("neko").WithDescription("neko");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		await using BotDbContext context = new();

		if (!await context.Nekos.AnyAsync())
		{
			await command.FollowupAsync("No nekos");
			return;
		}

		var neko = await context.Nekos.OrderBy(_ => EF.Functions.Random()).FirstOrDefaultAsync();

		await using MemoryStream mediaStream = new(neko.Media);
		using FileAttachment file = new(mediaStream, $"{neko.Filename}");
		await command.FollowupWithFileAsync(file);
	}
}