using WingTechBot.Handlers;

namespace WingTechBot
{
    class NaughtyCommand : Command
    {
        string duration;

        public override void Execute() => duration = CommandHandler.TempAddRole(Secrets.NAUGHTY_ROLE_ID, arguments, message, "naughty", requested);

        public override string LogString => $"added naughty role to {requested.Username} {duration}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override bool GetRequested => true;
    }

    class JesterCommand : Command
    {
        string duration;

        public override void Execute() => duration = CommandHandler.TempAddRole(Secrets.JESTER_ROLE_ID, arguments, message, "jester", requested);

        public override string LogString => $"added jester role to {requested.Username} {duration}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override bool GetRequested => true;
    }

    class SlowmodeCommand : Command
    {
        string duration;

        public override void Execute() => duration = CommandHandler.TempAddRole(Secrets.SLOWMODE_ROLE_ID, arguments, message, "slowmode", requested);

        public override string LogString => $"added slowmode role to {requested.Username} {duration}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override bool GetRequested => true;
    }

    class DoodooCommand : Command
    {
        string duration;

        public override void Execute() => duration = CommandHandler.TempAddRole(Secrets.DOODOO_ROLE_ID, arguments, message, "doodoo head", requested);

        public override string LogString => $"added doodoo head role to {requested.Username} {duration}";
        public override bool Audit => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
        public override bool GetRequested => true;
    }
}