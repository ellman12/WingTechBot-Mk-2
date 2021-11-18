using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace WingTechBot.Alarm
{
    public class UserAlarm
    {
        public static double TimerInterval { get; set; }
        public ulong UserID { get; private set; }
        private IUser User { get; set; }
        private IDMChannel Channel { get; set; }

        public double SnoozeInterval { private get; init; } = 60;

        private bool Ringing { get; set; } = false;

        private int count = 0;
        private int wordCount = 0;

        private Timer snoozeTimer;

        private ulong ChannelID { get; set; }

        private List<RepeatingTime> RepeatingTimes { get; init; }

        private List<SingleTime> SingleTimes { get; init; }

        private readonly string alarmMessage = "Your alarm is ringing. DM me any message to stop.", snoozeMessage = "Your alarm is ringing. DM me any message to stop.";

        public Func<string> GetAlarmMessage { private get; init; }
        public Func<string> GetSnoozeMessage { private get; init; }
        public bool DMOwner { private get; init; } = false;
        public bool SOTD { private get; init; } = false;
        public int WordCountRequirement { private get; init; } = 0;
        public string Name { get; init; } = string.Empty;

        public UserAlarm(ulong userID, List<RepeatingTime> times, List<SingleTime> singleTimes = null)
        {
            UserID = userID;
            RepeatingTimes = times;
            SingleTimes = singleTimes ?? new();
            GetAlarmMessage = () => alarmMessage;
            GetSnoozeMessage = () => snoozeMessage;
        }

        public void Init()
        {
            User ??= Program.GetUser(UserID);
            Channel ??= User.GetOrCreateDMChannelAsync().Result;
            ChannelID = Channel.Id;
        }

        public void Message(string s)
        {
            Init();
            Channel.SendMessageAsync(s);

            if (DMOwner)
            {
                Program.GetUser(Secrets.OWNER_USER_ID).GetOrCreateDMChannelAsync().Result.SendMessageAsync($"Alarm sent to {User.Username}#{User.Discriminator}: {s}");
            }

            Console.WriteLine($"Alarm DM'd {User.Username}#{User.Discriminator}: {s}");
        }

        public Task AlarmHandler(SocketMessage message)
        {
            Init();
            if (message.Author.Id != UserID) return Task.CompletedTask;
            if (message.Channel.Id != ChannelID) return Task.CompletedTask;
            if (!Ringing) return Task.CompletedTask;

            wordCount += message.Content.Split().Length;
            if (wordCount < WordCountRequirement)
            {
                Message($"Not enough written! {wordCount}/{WordCountRequirement} words.");
            }
            else
            {
                Message("Good morning!");

                if (SOTD && count <= 6)
                {
                    Message(Secrets.SOTD[Program.Random.Next(Secrets.SOTD.Length)]);
                }

                wordCount = 0;
                Ringing = false;
            }

            return Task.CompletedTask;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (RepeatingTimes.Any(x => x.EvaluateAndIncrement(e.SignalTime, TimerInterval, SingleTimes)) ||
                SingleTimes.Any(x => x.EvaluateAndRemove(e.SignalTime, TimerInterval, SingleTimes)))
            {
                count = 1;
                Ringing = true;
                Message($"{GetAlarmMessage()}");

                snoozeTimer = new(SnoozeInterval * 1000);
                snoozeTimer.Elapsed += OnSnooze;
                snoozeTimer.AutoReset = true;
                snoozeTimer.Enabled = true;
            }
        }

        public void OnSnooze(object source, ElapsedEventArgs e)
        {
            if (Ringing)
            {
                Message($"{GetSnoozeMessage()} ({count++})");
            }
            else
            {
                snoozeTimer.Enabled = false;
                snoozeTimer = null;
            }
        }

        public void Log()
        {
            Console.WriteLine($"Alarm Times for {Name}:");
            foreach (var x in RepeatingTimes) Console.WriteLine(x.Time);
            foreach (var x in SingleTimes) Console.WriteLine(x.Time);
        }
    }
}
