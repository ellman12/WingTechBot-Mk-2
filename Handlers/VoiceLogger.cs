using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace WingTechBot.Handlers
{
    public class VoiceLogger
    {
        public bool OwnerInVoice { get; set; } = false;

        public Task LogVoice(SocketUser user, SocketVoiceState priorState, SocketVoiceState currentState)
        {
            if (priorState.VoiceChannel != currentState.VoiceChannel)
            {
                if (priorState.VoiceChannel == null) // CHANNEL ENTER
                {
                    Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} entered channel {currentState.VoiceChannel.Name}");

                    if (user.Id == Secrets.OWNER_USER_ID) OwnerInVoice = true;
                    if (!OwnerInVoice)
                    {
#if OS_WINDOWS
                        new SoundPlayer(NOTIFY_SOUND_PATH).Play();
#endif
                    }
                }
                else if (currentState.VoiceChannel == null) // CHANNEL LEAVE
                {
                    Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} left channel {priorState.VoiceChannel.Name}");

                    if (user.Id == Secrets.OWNER_USER_ID) OwnerInVoice = false;
                }
                else // CHANNEL MOVE
                {
                    Console.WriteLine($"{DateTime.Now}: {user.Username}#{user.Discriminator} moved from channel {priorState.VoiceChannel.Name} to channel {currentState.VoiceChannel.Name}");
                }
            }

            return Task.CompletedTask;
        }
    }
}
