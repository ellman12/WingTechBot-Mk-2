namespace WingTechBot.Commands.VC;

public sealed class AddVoiceSoundCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("add-voice-sound")
			.WithDescription("Add this audio file to WTB's VoiceSounds")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("audio")
				.WithDescription("The file to upload")
				.WithType(ApplicationCommandOptionType.Attachment)
				.WithRequired(true)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("name")
				.WithDescription("The name of the sound")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var options = command.Data.Options;
		var connection = Bot.VoiceChannelConnection;

		if (options.Single(o => o.Name == "audio").Value is not Attachment audio)
		{
			await command.FollowupAsync("Missing attachment");
			return;
		}

		if (!audio.ContentType.StartsWith("audio"))
		{
			await command.FollowupAsync("Only audio is allowed");
			return;
		}

		var name = options.Single(o => o.Name == "name").Value.ToString() ?? throw new NullReferenceException("Missing sound name");
		var audioBytes = await Bot.HttpClient.GetByteArrayAsync(audio.Url);

		var existing = connection.AvailableSounds.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
		if (existing != null)
		{
			if (existing.Type == "voice")
			{
				await command.FollowupAsync($"A sound with the name `{name}` already exists, and will be overwritten");
				await SoundboardSound.OverwriteSoundAudio(name, audioBytes);
			}
			else
			{
				await command.FollowupAsync($"A Discord soundboard sound with the name `{name}` already exists, and will *not* be overwritten");
				return;
			}
		}
		else
		{
			await command.FollowupAsync($"{command.User.Username} uploaded `{name}`");
			await SoundboardSound.AddSoundboardSound(name, audioBytes);
		}

		await connection.GetSounds();
	}
}