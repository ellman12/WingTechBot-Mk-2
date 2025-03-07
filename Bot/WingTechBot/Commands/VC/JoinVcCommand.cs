namespace WingTechBot.Commands.VC;

public sealed class JoinVcCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		Bot.VoiceChannelConnection.Client.DefaultRequestHeaders.Add("Authorization", $"Bot {Bot.Config.LoginToken}");

		return new SlashCommandBuilder()
			.WithName("join-vc")
			.WithDescription("Make WTB join a specific VC channel")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the channel")
				.WithType(ApplicationCommandOptionType.Channel)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var channel = command.Data.Options.Single(o => o.Name == "name").Value;
		if (channel is not SocketVoiceChannel voiceChannel)
		{
			await command.FollowupAsync("Please provide a voice channel");
			return;
		}

		await command.FollowupAsync($"Joining {voiceChannel.Mention}");
		Bot.VoiceChannelConnection.Connect(voiceChannel);
	}
}