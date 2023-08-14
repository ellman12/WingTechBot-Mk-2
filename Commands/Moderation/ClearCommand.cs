namespace WingTechBot.Commands.Moderation;
using System;
using System.Linq;
using WingTechBot.Handlers;

internal class ClearCommand : Command
{
	public override void Execute()
	{
		try
		{
			replied = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;
			message.Channel.SendMessageAsync($"Clearing message reactions on message from {replied.Author.Mention}.");

			foreach (var v in replied.Reactions)
			{
				if (KarmaHandler.trackableEmotes.Contains(v.Key.Name))
				{
					var index = Array.IndexOf(KarmaHandler.trackableEmotes, v.Key.Name);
					Program.KarmaHandler.KarmaDictionary[replied.Author.Id][index] -= v.Value.ReactionCount;
					Console.WriteLine($"{DateTime.Now}: revoked {v.Value.ReactionCount} {v.Key.Name}(s) from {replied.Author.Mention}.");
				}
			}

			replied.RemoveAllReactionsAsync();
		}
		catch
		{
			throw new($"Failed to clear message reactions.");
		}
	}

	public override string LogString => $"cleared reactions on a message from {replied.Author.Username}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetReply => true;
}
