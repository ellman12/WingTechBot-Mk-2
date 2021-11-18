using System;
using System.Collections.Generic;
using System.Linq;

namespace WingTechBot.Alarm
{
    public class RepeatingTime : TimeBase
    {
        double Interval { get; set; }

        IntervalType IntervalType { get; set; }

        public RepeatingTime(DateTime start, double interval, IntervalType intervalType = IntervalType.Day)
        {
            Time = start;
            Interval = interval;
            IntervalType = intervalType;

            while (Interval != 0 && Time < DateTime.Now) Increment();
        }

        public RepeatingTime(int dayOfWeek, int hour, int minute, double interval, IntervalType intervalType = IntervalType.Day)
        {
            var now = DateTime.Now;

            Time = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            Time = Time.AddDays(dayOfWeek - (int)now.DayOfWeek);

            Interval = interval;
            IntervalType = intervalType;

            while (Interval != 0 && Time < DateTime.Now) Increment();
        }

        public void Increment()
        {
            Console.WriteLine($"Incrementing alarm at {Time} by {Interval} {IntervalType}(s)");

            Time = IntervalType switch
            {
                IntervalType.Millisecond => Time.AddMilliseconds(Interval),
                IntervalType.Second => Time.AddSeconds(Interval),
                IntervalType.Minute => Time.AddMinutes(Interval),
                IntervalType.Hour => Time.AddHours(Interval),
                IntervalType.Day => Time.AddDays(Interval),
                IntervalType.Month => Time.AddMonths((int)Interval),
                IntervalType.Year => Time.AddYears((int)Interval),
                _ => throw new ArgumentException("Invalid IntervalType")
            };
        }

        public bool EvaluateAndIncrement(DateTime time, double timerInterval, List<SingleTime> singleTimes)
        {
            if (Evaluate(time, timerInterval) && !singleTimes.Any(x => x.Time.Date == Time.Date && x.Override))
            {
                Increment();
                return true;
            }
            else return false;
        }
    }

    public enum IntervalType
    {
        Millisecond, Second, Minute, Hour, Day, Month, Year
    }
}
