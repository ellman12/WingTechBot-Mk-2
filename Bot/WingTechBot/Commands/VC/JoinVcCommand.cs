namespace WingTechBot.Commands.VC;

public sealed class JoinVcCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("join-vc")
			.WithDescription("Make WTB join a specific VC channel")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the channel")
				.WithType(ApplicationCommandOptionType.Channel)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		SocketVoiceChannel voiceChannel;
		if (command.Data.Options.SingleOrDefault(o => o.Name == "name")?.Value is not SocketChannel channel)
		{
			voiceChannel = Bot.DefaultVoiceChannel;
		}
		else if (channel is not SocketVoiceChannel socketVoiceChannel)
		{
			await command.FollowupAsync("Please provide a voice channel");
			return;
		}
		else
		{
			voiceChannel = socketVoiceChannel;
		}


		await command.FollowupAsync($"Joining {voiceChannel.Mention}");
		Bot.VoiceChannelConnection.Connect(voiceChannel);
	}
}