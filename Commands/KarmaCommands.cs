namespace WingTechBot;
using System;
using System.IO;
using System.Linq;
using Discord;
using WingTechBot.Handlers;

internal class KarmaCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any karma on record.");
		}
	}

	public override string LogString => $"reported {requested}'s karma";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "karma", "k" };
}

internal class AwardCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards on record.");
		}
	}

	public override string LogString => $"reported {requested}'s awards";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "award", "awards", "a" };
}

internal class RecordCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]}) {requested} has: {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards or karma on record.");
		}
	}

	public override string LogString => $"reported {requested}'s record";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "record", "records", "r" };
}

internal class SaveCommand : Command
{
	public override void Execute()
	{
		Program.KarmaHandler.Save();
		message.Channel.SendMessageAsync($"Saving karma values.");
	}

	public override string LogString => "manually saved";
	public override bool Audit => true;
	public override bool OwnerOnly => true;
}

internal class LogCommand : Command
{
	public override void Execute()
	{
		message.Channel.SendMessageAsync("Logging Karma Dictionary to Console.");
		foreach (var entry in Program.KarmaHandler.KarmaDictionary)
		{
			Console.Write(entry.Key);
			for (var i = 0; i < entry.Value.Length; i++)
			{
				Console.Write($" {entry.Value[i]}");
			}

			Console.WriteLine();
		}
	}

	public override string LogString => $"logging karma dictionary.";
	public override bool OwnerOnly => true;
}

internal class ReverseCommand : Command
{
	private int _caseNumber;

	public override void Execute()
	{
		try
		{
			_caseNumber = int.Parse(arguments[1]);
		}
		catch
		{
			throw new ArgumentException("Case number must be specified.");
		}

		string line;

		try
		{
			line = File.ReadLines(KarmaHandler.CASE_PATH).Skip(_caseNumber).Take(1).First();
		}
		catch
		{
			throw new($"{_caseNumber} not found.");
		}

		var values = line.Split(' ');

		var id = ulong.Parse(values[1]);

		var emoteID = int.Parse(values[2]);
		Program.KarmaHandler.KarmaDictionary[id][emoteID] += int.Parse(values[3]);

		message.Channel.SendMessageAsync($"Karma and awards from case {_caseNumber} has been returned to {Program.GetUser(id).Mention}...");

		var arrLine = File.ReadAllLines(KarmaHandler.CASE_PATH);
		arrLine[_caseNumber] += " REVERSED";
		File.WriteAllLines(KarmaHandler.CASE_PATH, arrLine);
	}

	public override string LogString => $"reversed case #{_caseNumber}.";
	public override bool OwnerOnly => true;
	public override bool Audit => true;
}

internal class ConfirmCommand : Command
{
	private int _caseNumber;

	public override void Execute()
	{
		try
		{
			_caseNumber = int.Parse(arguments[1]);
		}
		catch
		{
			throw new ArgumentException("Case number must be specified.");
		}

		var arrLine = File.ReadAllLines(KarmaHandler.CASE_PATH);
		try
		{
			arrLine[_caseNumber] += " CONFIRMED";
		}
		catch
		{
			message.Channel.SendMessageAsync($"{_caseNumber} not found.");
		}

		File.WriteAllLines(KarmaHandler.CASE_PATH, arrLine);
		message.Channel.SendMessageAsync($"{_caseNumber} confirmed.");
	}

	public override string LogString => $"confirmed case #{_caseNumber}.";
	public override bool OwnerOnly => true;
	public override bool Audit => true;
}

internal class RunningCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.RunningKarma.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.RunningKarma[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} running karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any running karma right now.");
		}
	}

	public override string LogString => $"reported {requested}'s running karma";
	public override bool GetRequested => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
}

internal class SpamCommand : Command
{
	public override void Execute()
	{
		message.Channel.SendMessageAsync($"Updating running karma.");
		Program.KarmaHandler.ClearRunningKarma();
	}

	public override string LogString => $"manually updating running karma.";
	public override bool OwnerOnly => true;
	public override bool Audit => true;
}

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
