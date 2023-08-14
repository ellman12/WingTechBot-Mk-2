namespace WingTechBot;
using System;
using System.Linq;
using Discord;

internal class TopCommand : Command
{
	public override void Execute()
	{
		var sorted = Program.KarmaHandler.KarmaDictionary
			.OrderBy(kvp => kvp.Value[1] - kvp.Value[0])
			.Select(kvp => kvp.Key)
			.ToList();

		var numToReport = Program.KarmaHandler.KarmaDictionary.Count;

		if (arguments.Length >= 2)
		{
			var success = int.TryParse(arguments[1], out numToReport);
			if (success)
			{
				numToReport = Math.Clamp(numToReport, Math.Min(1, Program.KarmaHandler.KarmaDictionary.Count), Program.KarmaHandler.KarmaDictionary.Count);
			}
			else if (arguments[1].ToLower() != "all")
			{
				message.Channel.SendMessageAsync($"Argument {arguments[1]} not recognized. Did you mean 'all'?");
			}
			else
			{
				numToReport = Program.KarmaHandler.KarmaDictionary.Count;
			}
		}

		var text = $"```Karma Leaderboard, Top {numToReport}\n";

		for (var i = 0; i < numToReport; i++)
		{
			try
			{
				var user = ((IGuild)Program.Client.GetGuild(Program.Config.ServerID))
					.GetUserAsync(sorted[i])
					.Result;
				text += $"[{i + 1}] = {Program.KarmaHandler.KarmaDictionary[sorted[i]][0] - Program.KarmaHandler.KarmaDictionary[sorted[i]][1]} karma - {user.Username}#{user.Discriminator}\n";
			}
			catch
			{
				text += $"[{i + 1}] = {Program.KarmaHandler.KarmaDictionary[sorted[i]][0] - Program.KarmaHandler.KarmaDictionary[sorted[i]][1]} karma - UNKNOWN USER\n";
			}
		}

		text += "```";

		message.Channel.SendMessageAsync(text);
	}

	public override string LogString => "reported leaderboard";
}