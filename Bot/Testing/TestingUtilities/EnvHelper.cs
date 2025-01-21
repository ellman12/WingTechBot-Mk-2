namespace TestingUtilities;

///Helper methods for working with the .env file.
public static class EnvHelper
{
	public static string EnvPath { get; } = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName, ".env");
	
	///Reads in and sets the values from the .env file.
	public static void ReadEnvVariables()
	{
		if (!File.Exists(EnvPath))
			return;
		
		string[] lines = File.ReadAllLines(EnvPath);
		foreach (var line in lines)
		{
			if (String.IsNullOrWhiteSpace(line))
				continue;
			
			string[] split = line.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Environment.SetEnvironmentVariable(split[0], split[1]);
		}
	}
}