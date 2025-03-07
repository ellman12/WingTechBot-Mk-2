namespace WingTechBot.Commands.VC;

public sealed class VoiceChannelConnection
{
	public Task Connection { get; private set; }

	public HttpClient Client { get; } = new();

	public SocketVoiceChannel ConnectedChannel { get; private set; }

	public void Connect(SocketVoiceChannel channel)
	{
		ConnectedChannel = channel;
		Connection = channel.ConnectAsync(disconnect: true);
	}

	public async Task Disconnect(SocketVoiceChannel channel)
	{
		ConnectedChannel = null;
		await channel.DisconnectAsync();
	}

}