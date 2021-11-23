namespace WingTechBot;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using WingTechBot.Handlers;

public static class Program
{
    private static DiscordSocketClient _client;

    public static SocketTextChannel BotChannel { get; private set; }

    public const string DELETE_PATH = @"wingtech_bot_deleted_messages.txt";
    public const string AUDIT_PATH = @"wingtech_bot_audit_log.txt";

    public const string NOTIFY_SOUND_PATH = @"C:\Windows\Media\Windows Notify Messaging.wav";

    public static bool BotOnly { get; set; }

    public static Random Random { get; } = new();

    public static CommandHandler CommandHandler { get; private set; } = new();
    public static GameHandler GameHandler { get; private set; } = new();
    public static KarmaHandler KarmaHandler { get; private set; } = new();
    public static VoiceLogger VoiceLogger { get; private set; } = new();
    public static AlarmHandler AlarmHandler { get; private set; }

    public static void Main() => MainAsync().GetAwaiter().GetResult();

    private static void OnProcessExit(object sender, EventArgs e) => KarmaHandler.Save();

    private static async Task MainAsync()
    {
        KarmaHandler.Load();

        var config = new DiscordSocketConfig() { MessageCacheSize = 100, AlwaysDownloadUsers = true };
        _client = new DiscordSocketClient(config);

        _client.MessageReceived += CommandHandler.CommandTask;
        _client.MessageReceived += GameHandler.GameTask;
        _client.Log += Log;

        _client.ReactionAdded += KarmaHandler.ReactionAdded;
        _client.ReactionRemoved += KarmaHandler.ReactionRemoved;

        _client.UserVoiceStateUpdated += VoiceLogger.LogVoice;

        _client.Ready += Start;

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        await _client.LoginAsync(TokenType.Bot, Secrets.LOGIN_TOKEN); // Secrets is only accessible on my local copy... Sorry, no stealing my log-in!
        await _client.SetGameAsync("cringe | ~help");
        await _client.StartAsync();

        AlarmHandler = new(_client);

        await AutoSave();
        await KarmaHandler.CheckRunningKarma();

        // Block this task until the Program is closed.
        await Task.Delay(-1);
    }

    private static Task Start()
    {
        BotChannel = _client.GetChannel(Secrets.BOT_CHANNEL_ID) as SocketTextChannel;
        foreach (SocketVoiceChannel vc in _client.GetGroupChannelsAsync().Result)
        {
            if (vc.Users.FirstOrDefault(x => x.Id == Secrets.OWNER_USER_ID) is not null)
            {
                VoiceLogger.OwnerInVoice = true;
                break;
            }
        }

        return Task.CompletedTask;
    }

    private static async Task AutoSave()
    {
        while (true)
        {
            await Task.Delay(3_600_000);
            Console.WriteLine("Autosaving");
            await KarmaHandler.Save();
        }
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public static SocketUser GetUserFromMention(SocketMessage message, string[] arguments, int index = 1)
    {
        SocketUser requested = message.Author;
        bool parsed = true;

        if (arguments.Length > 1)
        {
            parsed = MentionUtils.TryParseUser(arguments[index], out ulong id);
            requested = _client.GetUser(id);
        }

        if (requested is null || !parsed)
        {
            message.Channel.SendMessageAsync("User not found.");
            Console.WriteLine("User not found.");
            Console.WriteLine(arguments[index]);
            if (parsed) Console.WriteLine(MentionUtils.ParseUser(arguments[index]));
        }

        return requested;
    }

    public static bool TryGetUser(string mention, out IUser user)
    {
        user = null;
        bool found = MentionUtils.TryParseUser(mention, out ulong id);

        if (found)
        {
            try
            {
                user = _client.GetUser(id);
            }
            catch { }
        }

        return found;
    }

    public static void AddToAuditLog(IUser mod, string action)
    {
        using StreamWriter file = File.AppendText(AUDIT_PATH);

        file.WriteLine($"{mod.Username} {action} at {DateTime.Now}");
    }

    public static IUser GetUser(ulong id) => _client.GetUser(id);

    public static IChannel GetChannel(ulong id) => _client.GetChannel(id);
}
