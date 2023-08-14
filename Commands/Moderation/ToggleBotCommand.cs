namespace WingTechBot.Commands.Moderation;
internal class ToggleBotCommand : Command
{
    public override void Execute()
    {
        Program.BotOnly = !Program.BotOnly;
        message.Channel.SendMessageAsync($"Bot channel only toggle is: {Program.BotOnly}");
    }

    public override string LogString => $"botOnly set to {Program.BotOnly}";
    public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
    public override string[] Aliases => new[] { "togglebot", "tbot" };
}
