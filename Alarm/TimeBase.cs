namespace WingTechBot.Alarm;
using System;
using Newtonsoft.Json;

public abstract class TimeBase
{
    [JsonProperty] public DateTime Time { get; protected set; }

    protected static bool IsBetween(double s, double x, double e) => s <= x && x < e;

    public bool Evaluate(DateTime time, double timerInterval) => IsBetween(0, (time - Time).TotalMinutes, timerInterval);
}
