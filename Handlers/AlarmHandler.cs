namespace WingTechBot;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;
using Newtonsoft.Json;
using WingTechBot.Alarm;

public class AlarmHandler
{
	public const string ALARM_PATH = @"save\alarms.json";

	public List<UserAlarm> Alarms { get; } = new();

	private Timer _minuteTimer;

	public void SaveAlarms() => File.WriteAllText(ALARM_PATH, JsonConvert.SerializeObject(this));

	public void HookAlarms(DiscordSocketClient client)
	{
		foreach (var x in Alarms) client.MessageReceived += x.OnReceiveMessage;

		client.Connected += delegate
		{
			if (_minuteTimer is null)
			{
				_minuteTimer = new Timer(UserAlarm.TimerInterval * 60_000);
				foreach (var x in Alarms) _minuteTimer.Elapsed += x.OnTimedEvent;
				_minuteTimer.AutoReset = true;
				_minuteTimer.Enabled = true;
			}

			return Task.CompletedTask;
		};
	}

	public UserAlarm GetAlarm(ulong id) => Alarms.Find(x => x.UserID == id);

	public void AddAlarmToTimer(UserAlarm x) => _minuteTimer.Elapsed += x.OnTimedEvent;
}
