namespace WingTechBot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;
using WingTechBot.Alarm;

public class AlarmHandler
{
    public List<UserAlarm> Alarms { get; } = new();

    private Timer _minuteTimer;

    public AlarmHandler(DiscordSocketClient client)
    {
        UserAlarm.TimerInterval = 1;

        UserAlarm kraberQueenAlarm = Secrets.GetKraberQueenAlarm();
        kraberQueenAlarm.Log();
        Alarms.Add(kraberQueenAlarm);

        /*UserAlarm testAlarm = Secrets.GetTestAlarm();
        testAlarm.Log();
        Alarms.Add(testAlarm);*/

        foreach (var x in Alarms) client.MessageReceived += x.AlarmHandler;

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
}
