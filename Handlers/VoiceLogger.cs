namespace WingTechBot.Handlers;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;

public class VoiceLogger
{
    public bool OwnerInVoice { get; set; } = false;

    public Task LogVoice(SocketUser user, SocketVoiceState priorState, SocketVoiceState currentState)
    {
        if (priorState.VoiceChannel != currentState.VoiceChannel)
        {
            if (priorState.VoiceChannel is null) // CHANNEL ENTER
            {
                Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} entered channel {currentState.VoiceChannel.Name}");

                if (user.Id == Program.Config.OwnerID) OwnerInVoice = true;
                if (!OwnerInVoice)
                {
#if OS_WINDOWS
                        new SoundPlayer(NOTIFY_SOUND_PATH).Play();
#endif
                }
            }
            else if (currentState.VoiceChannel is null) // CHANNEL LEAVE
            {
                Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} left channel {priorState.VoiceChannel.Name}");

                if (user.Id == Program.Config.OwnerID) OwnerInVoice = false;
            }
            else // CHANNEL MOVE
            {
                Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} moved from channel {priorState.VoiceChannel.Name} to channel {currentState.VoiceChannel.Name}");
            }
        }

        return Task.CompletedTask;
    }
}
