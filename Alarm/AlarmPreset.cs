namespace WingTechBot.Alarm;
using System.Collections.Generic;

public record AlarmPreset
{
	public string Name { get; set; }

	public List<RepeatingTime> RepeatingTimes { get; set; }

	public List<SingleTime> SingleTimes { get; set; }

	public AlarmPreset(string name, List<RepeatingTime> repeatingTimes, List<SingleTime> singleTimes)
	{
		Name = name;
		RepeatingTimes = repeatingTimes;
		SingleTimes = singleTimes;
	}
}
