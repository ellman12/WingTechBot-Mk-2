using Discord;
using Discord.WebSocket;
using WingTechBot.Commands;
using WingTechBot.Database;
using WingTechBot.Database.Models;

namespace WingTechBot;

public static class Program
{
	public static DiscordSocketClient Client { get; private set; }

	public static Config Config { get; private set; }

	public static SocketTextChannel BotChannel { get; private set; }

	public static void Main()
	{
		using BotDbContext context = new();
		context.Database.EnsureCreated();

		context.Karma.Add(new Karma {Giver = "giver", Receiver = "receiver", Amount = 69});
		context.SaveChanges();
		
		Console.WriteLine(context.Karma.Select(a => a.Giver).Count());
		Console.WriteLine(context.Karma.Select(a => a.Giver).First());

		try
		{
			MainAsync().GetAwaiter().GetResult();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			Console.ReadLine();
			throw;
		}
	}

	private static async Task MainAsync()
	{
		Config = Config.FromJson();

		DiscordSocketConfig config = new() {MessageCacheSize = 100, AlwaysDownloadUsers = true};
		Client = new DiscordSocketClient(config);

		Client.Log += Log;

		Client.Ready += OnClientReady;

		await Client.LoginAsync(TokenType.Bot, Config.LoginToken);
		await Client.SetCustomStatusAsync(Config.StatusMessage);
		await Client.StartAsync();

		Client.SlashCommandExecuted += SlashCommandHandler.SlashCommandExecuted;

		await Task.Delay(Timeout.Infinite);
	}

	private static async Task OnClientReady()
	{
		BotChannel = Client.GetChannel(Config.BotChannelID) as SocketTextChannel;

		if (BotChannel == null)
		{
			throw new NullReferenceException("Could not find bot channel");
		}

		await SlashCommandHandler.SetUpCommands();

		await BotChannel.SendMessageAsync("Bot started and ready");
	}

	private static Task Log(LogMessage message)
	{
		Logger.LogLine(message.Message, message.Severity);
		return Task.CompletedTask;
	}

}