namespace WingTechBot.Commands.VC;

///Manages automatically playing soundboard sounds when specific events occur.
public sealed class AutoSounds
{
	public WingTechBot Bot { get; }

	public VoiceChannelConnection Connection { get; }

	public AutoSounds(WingTechBot bot, VoiceChannelConnection connection)
	{
		Bot = bot;
		Connection = connection;

		Bot.Client.UserVoiceStateUpdated += VoiceStateUpdated;
	}

	private async Task VoiceStateUpdated(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
	{
		string vcEvent = GetEventName(user, previous, current);

		if (String.IsNullOrWhiteSpace(vcEvent))
			return;

		if (!Bot.Config.AutoSounds.TryGetValue(vcEvent, out var eventGroup))
			return;

		if (!eventGroup.TryGetValue(user.Id, out var soundIds))
			return;

		//A delay is necessary for the user who just joined to hear the sound.
		await Task.Delay(300);

		var soundId = soundIds.GetRandom();
		var sound = Bot.VoiceChannelConnection.AvailableSounds.Single(s => s.Id == soundId);
		Connection.PlaySound(sound);
	}

	private string GetEventName(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
	{
		if (Connection.ConnectedChannel == null)
			return "";

		if (current.VoiceChannel?.Id == Connection.ConnectedChannel.Id)
			return "UserJoin";

		if (previous.VoiceChannel?.Id == Connection.ConnectedChannel.Id && user.Id != Bot.Config.UserId)
			return "UserLeave";

		return "";
	}
}