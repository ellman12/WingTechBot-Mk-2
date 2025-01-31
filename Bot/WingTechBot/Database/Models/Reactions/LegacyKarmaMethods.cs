using System.Reflection;

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

	///<summary>Converts this <see cref="LegacyKarma"/> row into a dictionary of <see cref="ReactionEmote"/>s and their totals.</summary>
	///<remarks>This is used to facilitate querying <see cref="ReactionTracker"/> data when legacy karma data is included.</remarks>
	public Dictionary<ReactionEmote, int> ConvertEmotes()
	{
		using BotDbContext context = new();
		return GetType()
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.Name != "UserId" && p.Name != "Year")
			.ToDictionary(p => context.ReactionEmotes.First(re => p.Name.ToLower().Contains(re.Name)), p => (int)p.GetValue(this)!);
	}
}