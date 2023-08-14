namespace WingTechBot.Commands.Role;
using WingTechBot.Handlers;

internal class JesterCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.JesterRoleID, arguments, message, "jester", requested);

	public override string LogString => $"added jester role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}
