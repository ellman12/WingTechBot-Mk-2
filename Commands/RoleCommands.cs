namespace WingTechBot;
using WingTechBot.Handlers;

internal class NaughtyCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.NaughtyRoleID, arguments, message, "naughty", requested);

	public override string LogString => $"added naughty role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new ulong[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}

internal class JesterCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.JesterRoleID, arguments, message, "jester", requested);

	public override string LogString => $"added jester role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new ulong[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}

internal class SlowmodeCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.SlowmodeRoleID, arguments, message, "slowmode", requested);

	public override string LogString => $"added slowmode role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new ulong[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}

internal class DoodooCommand : Command
{
	private string _duration;

	public override void Execute() => _duration = CommandHandler.TempAddRole(Program.Config.DoodooRoleID, arguments, message, "doodoo head", requested);

	public override string LogString => $"added doodoo head role to {requested.Username} {_duration}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new ulong[] { Program.Config.ModRoleID ?? 0 };
	public override bool GetRequested => true;
}
