namespace WingTechBot;
using System;
using System.IO;
using WingTechBot.Handlers;

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
