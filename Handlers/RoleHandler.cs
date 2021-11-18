using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace WingTechBot.Handlers
{
    public static class RoleHandler
    {
        public static readonly Dictionary<string, ulong> reactionRoles = new()
        {
            { "🎮", 704845642727686194 },
            { "🕵️", 835162140880797697 },
            { "💩", 835166184340455474 },
            { "🧊", 835164547122724904 },
            { "🧑‍🎓", 835162634654318592 },
        };

        public static void Handle(SocketReaction reaction, bool add)
        {
            SocketGuild server = (reaction.Channel as SocketGuildChannel).Guild;
            IRole role = server.GetRole(reactionRoles[reaction.Emote.Name]);
            IGuildUser user = reaction.User.Value as IGuildUser;

            if (add)
            {
                user.AddRoleAsync(role);
                Console.WriteLine($"{DateTime.Now}: added role {role} to {user.Username}#{user.Discriminator}.");
            }
            else
            {
                user.RemoveRoleAsync(role);
                Console.WriteLine($"{DateTime.Now}: removed role {role} to {user.Username}#{user.Discriminator}.");
            }
        }
    }
}
