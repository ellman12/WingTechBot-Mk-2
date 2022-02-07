namespace WingTechBot.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

	public bool Ringing { get; private set; } = false;

	private int _count = 0;
	private int _wordCount = 0;

	private Timer _snoozeTimer;

	protected ulong ChannelID { get; private set; }

	[JsonProperty] public List<RepeatingTime> RepeatingTimes { get; set; }

	[JsonProperty] public List<SingleTime> SingleTimes { get; set; }

	[JsonProperty] public List<AlarmPreset> Presets { get; set; }

	private readonly string _alarmMessage = "Your alarm is ringing. DM me any message to stop.", _snoozeMessage = "Your alarm is ringing. DM me any message to stop.";

	protected virtual Func<string> GetAlarmMessage { get; set; }
	protected virtual Func<string> GetSnoozeMessage { get; set; }
	public virtual bool DMOwner { get; set; } = false;
	public virtual bool SOTD { get; set; } = false;
	public virtual int WordCountRequirement { get; set; } = 0;
	public virtual string Name { get; set; } = string.Empty;
	public virtual bool Paused { get; set; } = false;

	public UserAlarm()
	{
		GetAlarmMessage = () => _alarmMessage;
		GetSnoozeMessage = () => _snoozeMessage;
	}

	public UserAlarm(ulong userID, List<RepeatingTime> times, List<SingleTime> singleTimes = null) : base()
	{
		UserID = userID;
		RepeatingTimes = times;
		SingleTimes = singleTimes ?? new();
	}

	public virtual void Init()
	{
		User ??= Program.GetUser(UserID);
		Channel ??= User.GetOrCreateDMChannelAsync().Result;
		ChannelID = Channel.Id;
	}

	public virtual void Message(string s, bool isSnooze = false)
	{
		Init();
		Channel.SendMessageAsync(s);

		if (DMOwner || !isSnooze)
		{
			Program.GetUser(Program.Config.OwnerID).GetOrCreateDMChannelAsync().Result.SendMessageAsync($"Alarm sent to {User.Username}#{User.Discriminator}: {s}");
		}

		Console.WriteLine($"Alarm DM'd {User.Username}#{User.Discriminator}: {s}");
	}

	public virtual Task OnReceiveMessage(SocketMessage message)
	{
		Init(); // $$$ investigate
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

	public virtual void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		if (RepeatingTimes.Any(x => x.EvaluateAndIncrement(e.SignalTime, TimerInterval, SingleTimes)) ||
			SingleTimes.Any(x => x.EvaluateAndRemove(e.SignalTime, TimerInterval, SingleTimes)))
		{
			if (Paused) return;

			_count = 1;
			Ringing = true;
			Message($"{GetAlarmMessage()}");

			_snoozeTimer = new(SnoozeInterval * 1000);
			_snoozeTimer.Elapsed += OnSnooze;
			_snoozeTimer.AutoReset = true;
			_snoozeTimer.Enabled = true;
		}
	}

	public virtual void OnSnooze(object source, ElapsedEventArgs e)
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

	[OnDeserialized]
	private void OnDeserialized(StreamingContext _) => Reset();

	public virtual void Reset()
	{
		foreach (var x in RepeatingTimes) x.Reset();
	}

	public RepeatingTime NextTime() => RepeatingTimes.MinBy(x => x.Time);

	public void StopRinging()
	{
		Ringing = false;
		_wordCount = 0;
	}
}
