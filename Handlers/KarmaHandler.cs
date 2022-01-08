namespace WingTechBot.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

public class KarmaHandler
{
	public Dictionary<ulong, int[]> KarmaDictionary { get; private set; } = new();
	public Dictionary<ulong, int[]> RunningKarma { get; private set; } = new();

	public static readonly string[] trackableEmotes = new string[] { "upvote", "downvote", "silver", "gold", "platinum" };

	public const string CASE_PATH = @"save\cases.txt";
	public const string SAVE_PATH = @"save\karma.txt";

	private static readonly string[] _upvoteScolds = new string[]
	{
			"god imagine upvoting yourself",
			"eww, a self-upvote",
			"upvoting yourself? cringe",
			"eww don't upvote yourself, this isn't reddit",
			"i'm going to verbally harrass you if you keep upvoting yourself",
			"smh my head this man just self-upvoted",
			"gross self-upvote",
			"redditor",
			"you know upvoting yourself doesn't increase your karma, right?",
			"i'm telling ben you upvoted yourself",
			"upvoting yourself? not cool",
			"peepee poopoo don't upvote yourself",
			"only nerds upvote themselves",
	};

	public const int RUNNING_KARMA_LIMIT = 25;

	public static readonly DateTimeOffset START_TIME = new DateTime(2020, 11, 25);

	public async Task CheckRunningKarma()
	{
		while (true)
		{
			await Task.Delay(900_000);
			Console.WriteLine("Checking Running Karma.");
			await ClearRunningKarma();
		}
	}

	public Task ClearRunningKarma()
	{
		foreach (var kvp in RunningKarma)
		{
			for (int i = 0; i < kvp.Value.Length; i++)
			{
				if (kvp.Value[i] is >= RUNNING_KARMA_LIMIT or <= (-5))
				{
					CreateCase(kvp, i);
					kvp.Value[i] = 0;
				}
				else kvp.Value[i] /= 2;
			}
		}

		return Task.CompletedTask;
	}

	private void CreateCase(KeyValuePair<ulong, int[]> kvp, int index)
	{
		int caseNumber = File.ReadLines(CASE_PATH).Count();
		string caseString = $"{caseNumber} {kvp.Key} {index} {kvp.Value[index]} ";
		Console.WriteLine($"Started new case {caseString}");

		KarmaDictionary[kvp.Key][index] -= kvp.Value[index];

		Program.BotChannel.SendMessageAsync($"Possible karma manipulation detected on user {Program.GetUser(kvp.Key).Mention}. {kvp.Value[index]} {trackableEmotes[index]} are being temporarily withheld. Case {caseNumber} opened. If this was an error, {Program.GetUser(Program.Config.OwnerID).Mention} will fix it shortly.");

		using StreamWriter file = File.AppendText(CASE_PATH);

		file.WriteLine(caseString);
	}

	public Task Save()
	{
		FileInfo fi = new(SAVE_PATH);
		using StreamWriter file = new(fi.Open(FileMode.Create));

		foreach (var entry in KarmaDictionary)
		{
			file.Write(entry.Key);
			for (int i = 0; i < entry.Value.Length; i++)
			{
				file.Write($" {entry.Value[i]}");
			}

			file.Write(" "); // need to pad!
			file.WriteLine();
		}

		return Task.CompletedTask;
	}

	public void Load()
	{
		FileInfo fi = new(SAVE_PATH);
		using StreamReader file = new(fi.Open(FileMode.OpenOrCreate));
		while (!file.EndOfStream)
		{
			string s = file.ReadLine();

			if (string.IsNullOrWhiteSpace(s)) continue;

			int i = 0;

			ulong id = ulong.Parse(NextNumber(s, ref i));

			int[] counts = new int[trackableEmotes.Length];

			for (int j = 0; j < counts.Length; j++)
			{
				i++;
				counts[j] = int.Parse(NextNumber(s, ref i));
			}

			KarmaDictionary.Add(id, counts);
			Console.WriteLine($"Loaded user: {id}.");
		}
	}

	public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheMessage, ISocketMessageChannel channel, SocketReaction reaction)
	{
		IMessage message = await cacheMessage.GetOrDownloadAsync();

		if (message is null || message.Timestamp < START_TIME) return;

		IGuildUser user = ((IGuild)(message.Channel as SocketGuildChannel).Guild).GetUserAsync(reaction.UserId).Result;

		if (trackableEmotes.Contains(reaction.Emote.Name))
		{
			if (!KarmaDictionary.ContainsKey(message.Author.Id)) KarmaDictionary.Add(message.Author.Id, new int[trackableEmotes.Length]);
			if (!RunningKarma.ContainsKey(message.Author.Id)) RunningKarma.Add(message.Author.Id, new int[trackableEmotes.Length]);

			if (message.Author.Id != reaction.UserId)
			{
				int id = Array.IndexOf(trackableEmotes, reaction.Emote.Name);
				KarmaDictionary[message.Author.Id][id]++;
				RunningKarma[message.Author.Id][id]++;
				Console.WriteLine($"{DateTime.Now}: incremented {message.Author}'s {trackableEmotes[id]}s");

				if (message.Author.Id == Program.BotID && reaction.Emote.Name == "downvote") // downvote self
				{
					await message.AddReactionAsync(Emote.Parse("<:downvote:672248822474211334>"));
					Console.WriteLine($"{DateTime.Now}: Downvoted self.");
				}
			}
			else
			{
				if (reaction.Emote.Name == "upvote")
				{
					if (!Program.BotOnly || channel.Id == Program.Config.BotChannelID) await message.Channel.SendMessageAsync($"{_upvoteScolds[Program.Random.Next(_upvoteScolds.Length)]} {message.Author.Mention}");
				}

				Console.WriteLine($"{DateTime.Now}: ignored {message.Author} self-vote");
			}
		}

		if (user.RoleIds.Contains(Program.Config.JesterRoleID ?? 0))
		{
			await cacheMessage.DownloadAsync().Result.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
			return;
		}
	}

	public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cacheMessage, ISocketMessageChannel _, SocketReaction reaction)
	{
		IMessage message = await cacheMessage.GetOrDownloadAsync();

		if (message is null || DateTime.Now < START_TIME) return;

		if (trackableEmotes.Contains(reaction.Emote.Name))
		{
			if (!KarmaDictionary.ContainsKey(message.Author.Id)) KarmaDictionary.Add(message.Author.Id, new int[trackableEmotes.Length]);
			if (!RunningKarma.ContainsKey(message.Author.Id)) RunningKarma.Add(message.Author.Id, new int[trackableEmotes.Length]);

			if (message.Author.Id != reaction.UserId)
			{
				int id = Array.IndexOf(trackableEmotes, reaction.Emote.Name);
				KarmaDictionary[message.Author.Id][id]--;
				RunningKarma[message.Author.Id][id]--;
				Console.WriteLine($"{DateTime.Now}: decremented {message.Author}'s {trackableEmotes[id]}s");
			}
		}
	}

	private static string NextNumber(string s, ref int i)
	{
		int start = i;
		bool startFound = false;

		for (; i < s.Length; i++)
		{
			if (!char.IsNumber(s[i]))
			{
				if (startFound) return s[start..i];
			}
			else if (!startFound)
			{
				startFound = true;
				start = i;
			}
		}

		if (char.IsNumber(s[^1])) return s[start..];

		Console.WriteLine("Invalid number found.");
		return "0";
	}
}
