namespace WingTechBot;
using System;

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
