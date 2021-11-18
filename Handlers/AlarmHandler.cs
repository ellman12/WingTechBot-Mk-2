using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using WingTechBot.Alarm;

namespace WingTechBot
{
    public class AlarmHandler
    {
        public List<UserAlarm> Alarms { get; } = new();

        private Timer minuteTimer;

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
                if (minuteTimer == null)
                {
                    minuteTimer = new Timer(UserAlarm.TimerInterval * 60_000);
                    foreach (var x in Alarms) minuteTimer.Elapsed += x.OnTimedEvent;
                    minuteTimer.AutoReset = true;
                    minuteTimer.Enabled = true;
                }

                return Task.CompletedTask;
            };
        }
    }
}