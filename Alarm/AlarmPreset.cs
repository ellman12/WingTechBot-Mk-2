namespace WingTechBot.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
