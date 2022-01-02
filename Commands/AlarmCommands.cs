namespace WingTechBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Newtonsoft.Json;
using WingTechBot.Alarm;

internal class AlarmCommand : Command
{
	private static readonly Dictionary<string, Func<UserAlarm, IMessage, string[], string>> _subCommands = new()
	{
		["log"] = AlarmSubCommands.Log,
	};

	private static readonly string[] _allowNull = new string[] { "log" };

	private string _logString;
	private UserAlarm _alarm;

	public override void Execute()
	{
		_alarm = Program.AlarmHandler.GetAlarm(message.Author.Id);
		string command = arguments[1].ToLower();

		if (_subCommands.ContainsKey(command))
		{
			if (_alarm is not null || _allowNull.Contains(command))
			{
				_logString = _subCommands[command].Invoke(_alarm, message, arguments[2..]);
			}
			else throw new Exception($"You do not have any alarms saved.");
		}
		else throw new Exception($"Alarm subcommand {arguments[1]} does not exist.");
	}

	public override string LogString => _logString;
}

internal static class AlarmSubCommands
{
	public static string Log(UserAlarm alarm, IMessage message, string[] _)
	{
		if (alarm is null) throw new Exception("You do not have any alarms saved.");
		else message.Channel.SendMessageAsync($"```json\n{JsonConvert.SerializeObject(alarm, Formatting.Indented)}\n```");
		

		return $"logged alarms for {message.Author.Username}";
	}

	public static string Skip(UserAlarm alarm, IMessage message, string[] _)
	{
		if (alarm?.RepeatingTimes is null) throw new Exception("You do not have any repeating alarms to skip.");
		else
		{
			RepeatingTime found = alarm.NextTime();

			message.Channel.SendMessageAsync($"Incrementing alarm {found}.");

			found.Increment();

			return $"incremented {message.Author.Username}'s next alarm to {found}";
		}
	}

	public static string Preset(UserAlarm alarm, IMessage message, string[] arguments)
	{
		if (arguments.Length < 2) throw new Exception("Invalid number of arguments! Preset expects 2 arguments.");

		string name = arguments[1].ToLower();
		AlarmPreset found = alarm.Presets.FirstOrDefault(x => x.Name == name);

		switch (arguments[0].ToLower())
		{
			case "load":
			{
				if (found is null) throw new Exception($"Preset {arguments[1]} does not exist.");
				else alarm.RepeatingTimes = found.RepeatingTimes;

				message.Channel.SendMessageAsync($"Loaded preset {arguments[1]}.");
				return $"loaded preset {name} for {message.Author.Username}";
			}
			case "save": // $$$ add override warning
			{
				if (found is not null) alarm.Presets.Remove(found);
				alarm.Presets.Add(new(name, alarm.RepeatingTimes, alarm.SingleTimes));

				message.Channel.SendMessageAsync($"Saved current times to preset {arguments[1]}.");
				return $"saved current times to preset {name} for {message.Author.Username}";
			}
			case "delete":
			{
				if (found is null) throw new Exception($"Preset {arguments[1]} does not exist.");
				else alarm.Presets.Remove(found);

				message.Channel.SendMessageAsync($"Deleted preset {arguments[1]}.");
				return $"deleted preset {name} for {message.Author.Username}";
			}
			case "rename":
			{
				if (arguments.Length < 3) throw new Exception("You must specify a new name.");
				else if (found is null) throw new Exception($"Preset {arguments[1]} does not exist.");
				else found.Name = arguments[2].ToLower();

				message.Channel.SendMessageAsync($"Renamed preset {arguments[1]} to {arguments[2]}.");
				return $"renamed preset {name} to {found.Name} for {message.Author.Username}";
			}
			default:
			{
				throw new ArgumentException($"Command preset {arguments[0]} not found.");
			}
		}
	}

	public static string Pause(UserAlarm alarm, IMessage message, string[] _)
	{
		if (alarm.Paused) throw new Exception("Your alarms are already paused.");
		else alarm.Paused = true;

		message.Channel.SendMessageAsync("Alarms paused.");
		return $"paused alarms for {message.Author.Username}";
	}

	public static string Resume(UserAlarm alarm, IMessage message, string[] _)
	{
		if (alarm.Paused) alarm.Paused = false;
		else throw new Exception("Your alarms are already resumed.");

		message.Channel.SendMessageAsync("Alarms resumed.");
		return $"resumed alarms for {message.Author.Username}";
	}
}