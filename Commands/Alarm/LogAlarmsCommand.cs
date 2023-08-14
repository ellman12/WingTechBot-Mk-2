namespace WingTechBot.Commands;
using System.IO;
using Newtonsoft.Json;

internal class LogAlarmsCommand : Command
{
	public override void Execute()
	{
		File.WriteAllText("alarm_dump.json", JsonConvert.SerializeObject(Program.AlarmHandler, Formatting.Indented));
		message.Channel.SendMessageAsync("dumped all alarms to alarm_dump.json");
	}

	public override bool OwnerOnly => true;

	public override string LogString => "logging all alarms";
}
