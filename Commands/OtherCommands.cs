using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingTechBot
{
    class StopCommand : Command
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

    class HelpCommand : Command
    {
        static string list = string.Empty;

        public override void Execute()
        {
            if (Program.CommandHandler.Commands.Count > 0)
            {
                if (list == string.Empty) // only run once
                {
                    list = "```Available Commands:\n";
                    foreach (var command in Program.CommandHandler.Commands)
                    {
                        if (command.Key == Program.CommandHandler.Commands.First((KeyValuePair<string, Type> kvp) => kvp.Value == Program.CommandHandler.Commands[command.Key]).Key)
                        {
                            Command c = Activator.CreateInstance(command.Value) as Command;
                            list += $" - {c.Name}";
                            if (c.Aliases.Length > 1)
                            {
                                string a = " (aliases:";
                                foreach (string s in c.Aliases) a += $" {s}";
                                list += a + ")";
                            }
                            list += "\n";
                        }
                    }
                    list += "```";
                }

                Console.WriteLine($"list: {list}");
                message.Channel.SendMessageAsync(list);
            }
            else
            {
                message.Channel.SendMessageAsync("There are no available commands.");
            }
        }

        public override string LogString => "listed commands.";
    }

    class DMCommand : Command
    {
        string sendMessage = string.Empty;

        public override void Execute()
        {
            string[] messageWords = arguments[2..];
            foreach (string s in messageWords) sendMessage += $"{s} ";

            requested.GetOrCreateDMChannelAsync().Result.SendMessageAsync(sendMessage);
            message.Channel.SendMessageAsync("Sent.");
        }

        public override string LogString => $"DM'd {requested.Username}#{requested.Discriminator}: {sendMessage}";
        public override bool GetRequested => true;
        public override bool OwnerOnly => true;
    }
}