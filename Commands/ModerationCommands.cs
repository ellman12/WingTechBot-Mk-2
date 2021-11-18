using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using WingTechBot.Handlers;

namespace WingTechBot
{
    class DeleteCommand : Command
    {
        public override void Execute()
        {
            try
            {
                message.Channel.SendMessageAsync($"Deleting message from {replied.Author.Mention}.");

                using (StreamWriter file = File.AppendText(Program.DELETE_PATH))
                {
                    file.WriteLine($"Message from: {replied.Author}");
                    file.WriteLine($"Deleted by: {message.Author}");
                    file.WriteLine($"Deleted on: {DateTime.Now}");
                    file.WriteLine($"Content: {replied.Content}");

                    if (replied.Attachments.Count > 0)
                    {
                        file.WriteLine($"Attachments:");
                        foreach (IAttachment attachment in replied.Attachments)
                        {
                            file.WriteLine($" - {attachment.Url}");
                        }
                    }

                    if (replied.Embeds.Count > 0)
                    {
                        file.WriteLine($"Embeds:");
                        foreach (Embed embed in replied.Embeds)
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
                throw new Exception($"Failed to delete message.");
            }

        }

        public override string LogString => $"deleted a message from {replied.Author.Username} in {replied.Channel.Name}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override string[] Aliases => new string[] { "delete", "d", "remove", "x", "erase" };
        public override bool GetReply => true;
    }

    class PinCommand : Command
    {
        string pin;

        public override void Execute()
        {
            try
            {
                replied = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;

                if (replied.IsPinned) ((SocketUserMessage)replied).UnpinAsync();
                else ((SocketUserMessage)replied).PinAsync();

                pin = replied.IsPinned ? "Pin" : "Unpin";
                message.Channel.SendMessageAsync($"{pin}ning message from {replied.Author.Mention}.");
            }
            catch
            {
                throw new Exception($"Failed to {pin} message.");
            }
        }

        public override string LogString => $"{pin}ned a message in {replied.Channel.Name}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override string[] Aliases => new string[] { "pin", "unpin", "p", "up" };
        public override bool GetReply => true;
    }

    class ClearCommand : Command
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
                        int index = Array.IndexOf(KarmaHandler.trackableEmotes, v.Key.Name);
                        Program.KarmaHandler.KarmaDictionary[replied.Author.Id][index] -= v.Value.ReactionCount;
                        Console.WriteLine($"{DateTime.Now}: revoked {v.Value.ReactionCount} {v.Key.Name}(s) from {replied.Author.Mention}.");
                    }
                }

                replied.RemoveAllReactionsAsync();
            }
            catch
            {
                throw new Exception($"Failed to clear message reactions.");
            }
        }

        public override string LogString => $"cleared reactions on a message from {replied.Author.Username}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override bool GetReply => true;
    }

    class ToggleBotCommand : Command
    {
        public override void Execute()
        {
            Program.BotOnly = !Program.BotOnly;
            message.Channel.SendMessageAsync($"Bot channel only toggle is: {Program.BotOnly}");
        }

        public override string LogString => $"botOnly set to {Program.BotOnly}";
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override string[] Aliases => new string[] { "togglebot", "tbot" };
    }
}