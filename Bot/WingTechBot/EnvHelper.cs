namespace WingTechBot;

///Helper methods for working with the .env file.
public static class EnvHelper
{
	public static readonly string DefaultPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName, ".env");
	
	///Reads in and sets the values from the .env file.
	public static void ReadEnvVariables(string path = "")
	{
		if (String.IsNullOrWhiteSpace(path))
			path = DefaultPath;

		if (!path.EndsWith(".env"))
			path = Path.Combine(path, ".env");

		if (!File.Exists(path))
			return;
		
		string[] lines = File.ReadAllLines(path);
		foreach (var line in lines)
		{
			if (String.IsNullOrWhiteSpace(line))
				continue;
			
			string[] split = line.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Environment.SetEnvironmentVariable(split[0], split[1]);
		}
	}
}