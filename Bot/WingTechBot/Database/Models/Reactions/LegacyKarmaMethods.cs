namespace WingTechBot.Database.Models.Reactions;

public sealed partial class LegacyKarma
{
	///Imports a file containing legacy karma data with the filename format of karma_xxxx.txt
	public static async Task ImportFile(string filePath, int year)
	{
		string[] lines = await File.ReadAllLinesAsync(filePath);

		var users = lines
			.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			.Select(split => (
				userId: UInt64.Parse(split[0]),
				values: split.Skip(1).Select(Int32.Parse).ToArray()
			))
			.Select(tuple =>
			{
				(ulong userId, int[] values) = tuple;
				return new LegacyKarma(userId, year, values[0], values[1], values[2], values[3], values[4]);
			})
			.ToArray();

		await using BotDbContext context = new();
		await context.LegacyKarma.AddRangeAsync(users);
		await context.SaveChangesAsync();
	}
}
