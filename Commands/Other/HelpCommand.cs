namespace WingTechBot.Commands.Other;
using System;
using System.Linq;

internal class HelpCommand : Command
{
	private static string _list = string.Empty;

	public override void Execute()
	{
		if (Program.CommandHandler.Commands.Count > 0)
		{
			if (_list == string.Empty) // only run once
			{
				_list = "```Available Commands:\n";
				foreach (var command in Program.CommandHandler.Commands)
				{
					if (command.Key == Program.CommandHandler.Commands.First((kvp) => kvp.Value == Program.CommandHandler.Commands[command.Key]).Key)
					{
						var c = Activator.CreateInstance(command.Value) as Command;
						_list += $" - {c.Name}";
						if (c.Aliases.Length > 1)
						{
							var a = " (aliases:";
							foreach (var s in c.Aliases)
							{
								a += $" {s}";
							}

							_list += a + ")";
						}

						_list += "\n";
					}
				}

				_list += "```";
			}

			Console.WriteLine($"list: {_list}");
			message.Channel.SendMessageAsync(_list);
		}
		else
		{
			message.Channel.SendMessageAsync("There are no available commands.");
		}
	}

	public override string LogString => "listed commands.";
}
