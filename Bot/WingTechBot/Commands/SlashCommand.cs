namespace WingTechBot.Commands;

///Represents any kind of slash command.
public abstract class SlashCommand
{
	public string Name { get; private set; }

	protected WingTechBot Bot { get; private set; }

	///If the command should be deferred when handled. Only disable if using RespondAsync() instead of FollowupAsync().
	public virtual bool Defer => true;

	///Override to true if only admins can run this command.
	public virtual bool Admin => false;

	///<summary>If no slash commands have changed, pass --no-recreate-commands to skip that step in initialization.</summary>
	///<remarks>This process takes a very long time to run when the bot starts, so don't recreate them unless needed.</remarks>
	public static bool NoRecreateCommands { get; set; }

	public async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		var built = CreateCommand().Build();
		Name = built.Name.Value;

		if (!NoRecreateCommands)
			await Bot.Client.CreateGlobalApplicationCommandAsync(built);

		Logger.LogLine($"Finish setting up command /{Name}", LogSeverity.Verbose);
	}

	protected abstract SlashCommandBuilder CreateCommand();

	public abstract Task HandleCommand(SocketSlashCommand command);
}