namespace WingTechBot.Alarm;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SingleTime : TimeBase
{
	[JsonProperty] public bool Override { get; private init; }

	[JsonConstructor] private SingleTime() { }

	public SingleTime(DateTime time, bool @override)
	{
		Time = time;
		Override = @override;
	}

	public override string ToString() => $"SingleTime at {Time} with Override = {Override}";

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
