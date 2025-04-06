namespace WingTechBot.Commands;

///Displays various info about the bot.
public sealed class InfoCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("info").WithDescription("Displays various info about the bot.");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		string message = $@"
WingTech Bot is used for various actions, like tracking reactions, calculating karma, and other silly shenanigans and antics.
## Reaction Tracking and Karma
WingTech Bot will track every reaction added to every message sent after the cutoff date (which is `{Bot.Config.StartDate:s}`). Any reactions added to or removed from messages sent before then are ignored. WTB will only track built-in emojis like 🤓 and emotes part of this server (like upvote/downvote, and the awards).

Karma is calculated based on the amount of reactions you receive with the upvote and downvote emotes. Giving yourself up/down votes will be ignored and you will be scolded for doing so.
## Soundboard
WingTech Bot Mk 2 has an enormous soundboard. To see all the available sounds, use `/available-sounds`. To play a sound, there are three methods.
### Slash Command
`/sound` gives you the most amount of options and features. You can play a specific sound, a random sound, repeat sounds, play them with a constant or random delay, and more. However, for one-off sounds, it is the slowest.
### Soundboard Thread
The [thread channel](https://discord.com/channels/111588824525627392/1349812179750359123) lets you send a sound name which will be played by the bot. This is meant for one-off sounds and does not let you do delays or looped sounds.
### Soundboard Website
The soundboard website was designed to look and feel like Discord's native soundboard. As long as the bot is in a VC channel, it will play the sound that's clicked.

If the bot is not in a VC channel when any of these are invoked, it will automatically join the [default VC](https://discord.com/channels/111588824525627392/111588824546598912) and play the sound. You can use `/leave-vc` to kick it out. It will also leave once everyone else leaves the VC. To have it join a specific VC, use `/join-vc`.
		";

		await command.RespondAsync(message, ephemeral: true);
	}
}
