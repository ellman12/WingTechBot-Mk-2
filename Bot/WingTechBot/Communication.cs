namespace WingTechBot;

///Allows users to communicate with WingTech Bot.
public sealed class Communication
{
	public WingTechBot Bot { get; }

	private readonly HttpClient httpClient = new();

	public Communication(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.MessageReceived += OnMessageReceived;
	}

	public async Task<string> SendMessage(string message)
	{
		string json = $$"""{"contents": [{"parts": [{"text": "{{message}}"}]}]}""";
		var content = new StringContent(json);
		var response = await httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={Bot.Config.LLMToken}", content);

		var parsed = JsonSerializer.Deserialize<MessageResponse>(await response.Content.ReadAsStringAsync());
		return parsed.Candidates.First().Content.Parts.First().Text;
	}

	private async Task OnMessageReceived(SocketMessage message)
	{
		if (message.MentionedUsers.Any(u => u.Id == Bot.Config.UserId) || message.MentionedRoles.Any(r => r.Members.Any(u => u.Id == Bot.Config.UserId)) || message.MentionedEveryone)
		{
			await message.Channel.SendMessageAsync("Whomst has summoned the almighty one?");
		}
	}

	private sealed record MessageResponse
	{
		[JsonPropertyName("candidates")]
		public List<Candidate> Candidates { get; init; }
	}

	private sealed record Candidate
	{
		[JsonPropertyName("content")]
		public Content Content { get; init; }
	}

	private sealed record Content
	{
		[JsonPropertyName("parts")]
		public List<Part> Parts { get; init; }
	}

	private sealed record Part
	{
		[JsonPropertyName("text")]
		public string Text { get; init; }
	}
}