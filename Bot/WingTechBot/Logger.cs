using System.Diagnostics;

namespace WingTechBot;

///<summary>Used for printing messages of variable importance to the terminal.</summary>
public static class Logger
{
	public static void LogLine(object value, LogSeverity itemImportance = LogSeverity.Info)
	{
		SetConsoleColor(itemImportance);
		Console.WriteLine($"{DateTime.Now} {value}");
		Console.ResetColor();
	}

	public static Task LogLine(LogMessage message)
	{
		LogLine(message.Message, message.Severity);
		return Task.CompletedTask;
	}

	private static void SetConsoleColor(LogSeverity itemImportance)
	{
		Console.ForegroundColor = itemImportance switch
		{
			LogSeverity.Critical or LogSeverity.Error => ConsoleColor.Red,
			LogSeverity.Warning => ConsoleColor.Yellow,
			LogSeverity.Info => ConsoleColor.White,
			LogSeverity.Verbose => ConsoleColor.Cyan,
			LogSeverity.Debug => ConsoleColor.Green,
			_ => throw new ArgumentException()
		};
	}

	public static void LogException(Exception e) => LogLine($"Exception raised in {GetCallingMethodName()}: {e.Message}\n", LogSeverity.Error);

	public static async Task LogExceptionAsMessage(Exception e, IMessageChannel channel)
	{
		LogLine($"Exception raised: {e.Message}\n", LogSeverity.Error);
		await channel.SendMessageAsync($"Exception raised: {e.Message}\n");
	}

	private static string GetCallingMethodName() => new StackTrace().GetFrame(2)!.GetMethod()!.Name; //If it's set to 1 it'd print LogLine.
}