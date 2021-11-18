using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingTechBot.Handlers
{
    public class GameHandler
    {
        public Type[] Games { get; private set; }
        public List<Game> ActiveGames { get; private set; } = new();

        public GameHandler()
        {
            var @assembly = typeof(Game).Assembly;
            Games = @assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Game))).ToArray();
        }

        public Task GameTask(SocketMessage message)
        {
            if (string.IsNullOrEmpty(message.Content)) return Task.CompletedTask;
            if (message.Channel is IGuildChannel && message.Channel.Name != "bot") return Task.CompletedTask;

            bool endGame = message.Content.Trim().ToLower() == "~endgame";

            if (!endGame && message.Content.Trim().ToLower()[0] == '~')
            {
                if (message.Content.Trim().Length > 1 && message.Content.Trim().ToLower()[1] == '~')
                {
                    foreach (Game game in ActiveGames)
                    {
                        if (game.GamemasterID == message.Author.Id || game.PlayerIDs.Contains(message.Author.Id))
                        {
                            game.ReceiveCommand(message);
                            break;
                        }
                    }
                }

                return Task.CompletedTask;
            }

            foreach (Game game in ActiveGames)
            {
                if (game.GamemasterID == message.Author.Id || game.PlayerIDs.Contains(message.Author.Id))
                {
                    if (endGame)
                    {
                        message.Channel.SendMessageAsync("Ending game.");
                        game.Shutdown();
                        EndGame(game);
                    }
                    else game.ReceiveMessage(message);

                    break;
                }
            }

            return Task.CompletedTask;
        }

        public bool PlayerAvailable(ulong playerId) => ActiveGames.FirstOrDefault((Game g) => g.PlayerIDs.Contains(playerId)) == null;

        public void EndGame(Game game) => ActiveGames.Remove(game);
    }
}
