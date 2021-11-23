namespace WingTechBot.Alarm;
using System;
using System.Collections.Generic;

public class SingleTime : TimeBase
{
    public bool Override { get; private init; }

    public SingleTime(DateTime time, bool @override)
    {
        Time = time;
        Override = @override;
    }

    public bool EvaluateAndRemove(DateTime time, double timerInterval, List<SingleTime> singleTimes)
    {
        if (Evaluate(time, timerInterval))
        {
            singleTimes.Remove(this);
            return true;
        }
        else return false;
    }
}
