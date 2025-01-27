namespace WingTechBot.Commands;

///Displays various info about the bot.
public sealed class InfoCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		var infoCommand = new SlashCommandBuilder().WithName("info").WithDescription("Displays various info about the bot.");

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(infoCommand.Build());
		}
		catch (Exception e)
		{
			Logger.LogLine("Error adding info command");
			Logger.LogException(e);
		}
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != "info")
			return;

		string message = $@"
WingTech Bot is used for various actions, like tracking reactions, calculating karma, and other silly shenanigans and antics.
### Reaction Tracking and Karma
WingTech Bot will track every reaction added to every message sent after the cutoff date (which is `{Bot.Config.StartDate:s}`). Any reactions added to or removed from messages sent before then are ignored. WTB will only track built-in emojis like 🤓 and emotes part of this server (like upvote/downvote, and the awards).

Karma is calculated based on the amount of reactions you receive with the upvote and downvote emotes. Giving yourself up/down votes will be ignored and you will be scolded for doing so.
		";

		await command.FollowupAsync(message, ephemeral: true);
	}
}