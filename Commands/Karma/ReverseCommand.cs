namespace WingTechBot;
using System;
using System.IO;
using System.Linq;
using WingTechBot.Handlers;

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
