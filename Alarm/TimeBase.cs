namespace WingTechBot.Alarm;
using System;

public abstract class TimeBase
{
    public DateTime Time { get; protected set; }

    protected static bool IsBetween(double s, double x, double e) => s <= x && x < e;

    public bool Evaluate(DateTime time, double timerInterval) => IsBetween(0, (time - Time).TotalMinutes, timerInterval);
}
