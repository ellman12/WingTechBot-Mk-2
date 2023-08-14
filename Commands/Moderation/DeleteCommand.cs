namespace WingTechBot.Commands.Moderation;
using System;
using System.IO;
using System.Linq;
using Discord;

internal class DeleteCommand : Command
{
	public override void Execute()
	{
		try
		{
			message.Channel.SendMessageAsync($"Deleting message from {replied.Author.Mention}.");

			using (var file = File.AppendText(Program.DELETE_PATH))
			{
				file.WriteLine($"Message from: {replied.Author}");
				file.WriteLine($"Deleted by: {message.Author}");
				file.WriteLine($"Deleted on: {DateTime.Now}");
				file.WriteLine($"Content: {replied.Content}");

				if (replied.Attachments.Count > 0)
				{
					file.WriteLine($"Attachments:");
					foreach (var attachment in replied.Attachments)
					{
						file.WriteLine($" - {attachment.Url}");
					}
				}

				if (replied.Embeds.Count > 0)
				{
					file.WriteLine($"Embeds:");
					foreach (var embed in replied.Embeds.Cast<Embed>())
					{
						file.WriteLine($" - {embed.Url}");
					}
				}

				file.WriteLine("");
			}

			message.Channel.DeleteMessageAsync(replied.Id);
		}
		catch
		{
			throw new($"Failed to delete message.");
		}
	}

	public override string LogString => $"deleted a message from {replied.Author.Username} in {replied.Channel.Name}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
	public override string[] Aliases => new[] { "delete", "d", "remove", "x", "erase" };
	public override bool GetReply => true;
}
