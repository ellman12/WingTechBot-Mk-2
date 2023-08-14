namespace WingTechBot.Commands.Role;
using WingTechBot.Handlers;

internal class SlowmodeCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.SlowmodeRoleID, arguments, message, "slowmode", requested);

	public override string LogString => $"added slowmode role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}
