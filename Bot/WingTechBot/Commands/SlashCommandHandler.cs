using Discord;
using Discord.WebSocket;

namespace WingTechBot.Commands;

public sealed class SlashCommandHandler
{
	//It is essential that the keys are all lowercase.
	//Also, when adding new ones, you might need to reload Discord.
	private static readonly Dictionary<string, string> Commands = new()
	{
		{"help", "Lists all commands"},
		{"gato", "Sends a random cat picture"},
	};

	private WingTechBot Bot { get; init; }

	private SlashCommandHandler() {}

	public static async Task<SlashCommandHandler> Create(WingTechBot bot)
	{
		SlashCommandHandler handler = new() {Bot = bot};
		await handler.SetUpCommands();
		return handler;
	}

	public async Task SetUpCommands()
	{
		await ClearCommands();

		foreach (var command in Commands)
		{
			await SetUpCommand(command.Key, command.Value);
		}
	}

	///Removes all slash commands from the bot.
	private async Task ClearCommands()
	{
		var guild = Bot.Client.GetGuild(Bot.Config.ServerId);
		await guild.DeleteApplicationCommandsAsync();
	}

	private async Task<SlashCommandProperties> SetUpCommand(string name, string description)
	{
		var globalCommand = new SlashCommandBuilder().WithName(name).WithDescription(description);
		var built = globalCommand.Build();
		await Bot.Client.CreateGlobalApplicationCommandAsync(built);
		return built;
	}

	public async Task SlashCommandExecuted(SocketSlashCommand command)
	{
		await command.RespondAsync($"You executed {command.Data.Name}");

		/*
		* TODO: switch here
		*/
	}
}