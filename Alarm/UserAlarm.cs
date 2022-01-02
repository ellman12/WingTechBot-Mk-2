namespace WingTechBot.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

public class UserAlarm
{
    public static double TimerInterval { get; set; } = 1;

    public virtual ulong UserID { get; set; }
    private IUser User { get; set; }
    private IDMChannel Channel { get; set; }

    public virtual double SnoozeInterval { get; set; } = 60;

    public virtual int SnoozeTolerance { get; set; } = 25;

    private bool Ringing { get; set; } = false;

    private int _count = 0;
    private int _wordCount = 0;

    private Timer _snoozeTimer;

    private ulong ChannelID { get; set; }

    [JsonProperty] public List<RepeatingTime> RepeatingTimes { get; set; }

    [JsonProperty] public List<SingleTime> SingleTimes { get; set; }

    [JsonProperty] public List<AlarmPreset> Presets { get; set; }

    private readonly string _alarmMessage = "Your alarm is ringing. DM me any message to stop.", _snoozeMessage = "Your alarm is ringing. DM me any message to stop.";

    public virtual Func<string> GetAlarmMessage { protected get; init; }
    public virtual Func<string> GetSnoozeMessage { protected get; init; }
    public virtual bool DMOwner { get; set; } = false;
    public virtual bool SOTD { get; set; } = false;
    public virtual int WordCountRequirement { get; set; } = 0;
    public virtual string Name { get; set; } = string.Empty;

    public UserAlarm() { }

    public UserAlarm(ulong userID, List<RepeatingTime> times, List<SingleTime> singleTimes = null)
    {
        UserID = userID;
        RepeatingTimes = times;
        SingleTimes = singleTimes ?? new();
        GetAlarmMessage = () => _alarmMessage;
        GetSnoozeMessage = () => _snoozeMessage;
    }

    public void Init()
    {
        User ??= Program.GetUser(UserID);
        Channel ??= User.GetOrCreateDMChannelAsync().Result;
        ChannelID = Channel.Id;
    }

    public void Message(string s, bool isSnooze = false)
    {
        Init();
        Channel.SendMessageAsync(s);

        if (DMOwner || !isSnooze)
        {
            Program.GetUser(Program.Config.OwnerID).GetOrCreateDMChannelAsync().Result.SendMessageAsync($"Alarm sent to {User.Username}#{User.Discriminator}: {s}");
        }

        Console.WriteLine($"Alarm DM'd {User.Username}#{User.Discriminator}: {s}");
    }

    public Task AlarmHandler(SocketMessage message)
    {
        Init();
        if (message.Author.Id != UserID) return Task.CompletedTask;
        if (message.Channel.Id != ChannelID) return Task.CompletedTask;
        if (!Ringing) return Task.CompletedTask;

        _wordCount += message.Content.Split().Length;
        if (_wordCount < WordCountRequirement)
        {
            Message($"Not enough written! {_wordCount}/{WordCountRequirement} words.");
        }
        else
        {
            Message("Good morning!");

            if (SOTD && _count <= SnoozeTolerance)
            {
                Message(SongOfTheDay.GetSong());
            }

            _wordCount = 0;
            Ringing = false;
        }

        return Task.CompletedTask;
    }

    public void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        if (RepeatingTimes.Any(x => x.EvaluateAndIncrement(e.SignalTime, TimerInterval, SingleTimes)) ||
            SingleTimes.Any(x => x.EvaluateAndRemove(e.SignalTime, TimerInterval, SingleTimes)))
        {
            _count = 1;
            Ringing = true;
            Message($"{GetAlarmMessage()}");

            _snoozeTimer = new(SnoozeInterval * 1000);
            _snoozeTimer.Elapsed += OnSnooze;
            _snoozeTimer.AutoReset = true;
            _snoozeTimer.Enabled = true;
        }
    }

    public void OnSnooze(object source, ElapsedEventArgs e)
    {
        if (Ringing)
        {
            Message($"{GetSnoozeMessage()} ({_count++})", true);
        }
        else
        {
            _snoozeTimer.Enabled = false;
            _snoozeTimer = null;
        }
    }

    public void Log()
    {
        Console.WriteLine($"Alarm Times for {Name}:");
        foreach (var x in RepeatingTimes) Console.WriteLine(x.Time);
        foreach (var x in SingleTimes) Console.WriteLine(x.Time);
    }
}
