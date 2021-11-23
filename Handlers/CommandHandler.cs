using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingTechBot.Handlers
{
    public class CommandHandler
    {
        public Dictionary<string, Type> Commands { get; } = new();

        public CommandHandler()
        {
            var @assembly = typeof(CommandHandler).Assembly;

            Type[] commandArray = @assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Command))).ToArray();

            foreach (var t in commandArray)
            {
                foreach (string name in (Activator.CreateInstance(t) as Command).Aliases)
                {
                    try
                    {
                        Commands.Add(name, t);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"{name} from {t} errored: {e.Message}", e);
                    }
                }
            }
        }

        public Task CommandTask(SocketMessage message)
        {
            if (!message.Content.StartsWith("~") || message.Author.IsBot || (message.Channel.Name != "bot" && Program.BotOnly))
            {
                if (!message.Content.StartsWith("~") && !message.Author.IsBot && message.Author.Id != Secrets.OWNER_USER_ID && message.Channel is IDMChannel)
                {
                    Program.GetUser(Secrets.OWNER_USER_ID).GetOrCreateDMChannelAsync().Result.SendMessageAsync($"from {message.Author.Username}#{message.Author.Discriminator} with ID ({message.Author.Id}) @ {message.Timestamp.LocalDateTime}: {message.Content}");
                }

                return Task.CompletedTask;
            }

            string command;
            string[] arguments;

            arguments = message.Content[1..].Split(' ');
            command = arguments[0].ToLower();

            if (!Commands.TryGetValue(command, out Type foundCommand))
            {
                message.Channel.SendMessageAsync($"The command \"{command}\" could not be found.");
                return Task.CompletedTask;
            }

            Command createCommand = Activator.CreateInstance(foundCommand) as Command;

            try
            {
                createCommand.Init(message, arguments);
                createCommand.Execute();
                Console.WriteLine($"{DateTime.Now}: {createCommand.LogString}");
                if (createCommand.Audit) Program.AddToAuditLog(message.Author, createCommand.LogString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                message.Channel.SendMessageAsync(e.Message);
            }

            return Task.CompletedTask;
        }

        public static string TempAddRole(ulong roleID, string[] arguments, SocketMessage message, string roleName, IUser requested)
        {
            try
            {
                SocketGuild server = (message.Channel as SocketGuildChannel).Guild;
                IGuildUser user = ((IGuild)server).GetUserAsync(requested.Id).Result;

                if (user.RoleIds.Contains(Secrets.MOD_ROLE_ID) || user.Id == Secrets.OWNER_USER_ID) throw new Exception($"Role {roleName} cannot be applied to a mod.");
                user.AddRoleAsync(server.GetRole(roleID));

                int minutes = arguments.Length < 2 ? -1 : int.Parse(arguments[2]);
                string duration = minutes >= 1 ? $"for {minutes} minute(s)" : "permanently";

                Program.AddToAuditLog(message.Author, $"added role {roleName} to {user.Username} {duration}");

                if (minutes >= 1)
                {
                    Task t = new(async () =>
                    {
                        await Task.Delay(minutes * 60_000);
                        await user.RemoveRoleAsync(server.GetRole(roleID));
                        Console.WriteLine($"{DateTime.Now}: removing {roleName} to {user.Username}");
                    });

                    t.Start();
                }

                Console.WriteLine($"{DateTime.Now}: giving {roleName} to {user.Username} {duration}.");
                message.Channel.SendMessageAsync($"Giving {roleName} to {user.Username} {duration}.");

                return duration;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to demote. {e.Message}");
            }
        }
    }
}
