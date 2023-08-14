namespace WingTechBot.Commands.Other;
using System;
using System.Threading.Tasks;

internal class StopCommand : Command
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
