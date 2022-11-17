namespace WingTechBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class StopCommand : Command
{
	public override void Execute()
	{
		message.Channel.SendMessageAsync($"Bye bye!");
		Program.KarmaHandler.Save();
		Task.Delay(10000);
		Environment.Exit(0);
	}

	public override string LogString => $"bye bye!";
	public override bool OwnerOnly => true;
}

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
					if (command.Key == Program.CommandHandler.Commands.First((KeyValuePair<string, Type> kvp) => kvp.Value == Program.CommandHandler.Commands[command.Key]).Key)
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

internal class DMCommand : Command
{
	private string _sendMessage = string.Empty;

	public override void Execute()
	{
		var messageWords = arguments[2..];
		foreach (var s in messageWords)
		{
			_sendMessage += $"{s} ";
		}

		requested.GetOrCreateDMChannelAsync().Result.SendMessageAsync(_sendMessage);
		message.Channel.SendMessageAsync("Sent.");
	}

	public override string LogString => $"DM'd {requested.Username}#{requested.Discriminator}: {_sendMessage}";
	public override bool GetRequested => true;
	public override bool OwnerOnly => true;
}
