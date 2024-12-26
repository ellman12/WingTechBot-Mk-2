using Discord;
using Discord.WebSocket;

namespace WingTechBot.Commands;

public static class SlashCommandHandler
{
	//It is essential that the keys are all lowercase.
	//Also, when adding new ones, you might need to reload Discord.
	private static readonly Dictionary<string, string> Commands = new()
	{
		{"help", "Lists all commands"},
		{"gato", "Sends a random cat picture"},
	};

	///Sets up all commands for the bot.
	public static async Task SetUpCommands()
	{
		await ClearCommands();

		foreach (var command in Commands)
		{
			await SetUpCommand(command.Key, command.Value);
		}
	}

	private static async Task ClearCommands()
	{
		var guild = Program.Client.GetGuild(Program.Config.ServerId);
		await guild.DeleteApplicationCommandsAsync();
	}

	private static async Task<SlashCommandProperties> SetUpCommand(string name, string description)
	{
		SlashCommandBuilder globalCommand = new();
		globalCommand.WithName(name);
		globalCommand.WithDescription(description);

		var built = globalCommand.Build();
		await Program.Client.CreateGlobalApplicationCommandAsync(built);
		return built;
	}

	public static async Task SlashCommandExecuted(SocketSlashCommand command)
	{
		await command.RespondAsync($"You executed {command.Data.Name}");

		/*
		* TODO: switch here
		*/
	}
}