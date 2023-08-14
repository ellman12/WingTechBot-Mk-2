namespace WingTechBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using WingTechBot.Alarm;

internal class AlarmCommand : Command
{
	public static Dictionary<string, Func<UserAlarm, IMessage, string[], string>> SubCommands => new()
	{
		["log"] = (a, message, _) => AlarmSubCommands.Log(a, message),
		["skip"] = (a, message, _) => AlarmSubCommands.Skip(a, message),
		["preset"] = AlarmSubCommands.Preset,
		["pause"] = (a, message, _) => AlarmSubCommands.Pause(a, message),
		["resume"] = (a, message, _) => AlarmSubCommands.Resume(a, message),
		["clear"] = AlarmSubCommands.Clear,
		//["set"] = AlarmSubCommands.Set,
		["template"] = (_, message, _) => AlarmSubCommands.Template(message),
		["help"] = (a, message, _) => AlarmSubCommands.Help(a, message),
		["add"] = AlarmSubCommands.Add,
		["remove"] = AlarmSubCommands.Remove,
	};

	private static readonly string[] _allowNull = new[] { "add", "set", "template", "help" };

	private string _logString;
	private UserAlarm _alarm;

	public override void Execute()
	{
		_alarm = Program.AlarmHandler.GetAlarm(message.Author.Id);
		var command = arguments[1].ToLower();

		if (SubCommands.ContainsKey(command))
		{
			if (_alarm is not null || _allowNull.Contains(command))
			{
				_logString = SubCommands[command].Invoke(_alarm, message, arguments[2..]);
			}
			else
			{
				throw new($"You do not have any alarms saved.");
			}
		}
		else
		{
			throw new($"Alarm subcommand {arguments[1]} does not exist.");
		}
	}

	public override string LogString => _logString;
}
