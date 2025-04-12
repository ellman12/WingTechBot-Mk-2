namespace WingTechBot;

///Allows users to communicate with WingTech Bot.
public sealed class Communication
{
	public WingTechBot Bot { get; }

	private readonly HttpClient httpClient = new();

	private DateTime lastPingTime = DateTime.MinValue, lastMessageSentTime = DateTime.MinValue;

	public Communication(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.MessageReceived += OnMessageReceived;
	}

	public async Task<string> SendMessageToAi(string message)
	{
		var data = new MessagePostData(message, Bot.Config.LLMBehavior);
		var json = JsonSerializer.Serialize(data);
		var content = new StringContent(json);
		var response = await httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={Bot.Config.LLMToken}", content);

		var parsed = JsonSerializer.Deserialize<MessageResponse>(await response.Content.ReadAsStringAsync());
		return parsed.Candidates.First().Content.Parts.First().Text;
	}

	private async Task OnMessageReceived(SocketMessage message)
	{
		if (message.Author.Id == Bot.Config.UserId || message.Author.IsBot)
			return;

		if (message.MentionedUsers.Any(u => u.Id == Bot.Config.UserId) || message.MentionedRoles.Any(r => r.Members.Any(u => u.Id == Bot.Config.UserId)) || message.MentionedEveryone)
		{
			string filtered = Regex.Replace(message.Content, @"<@(\d+)>", "");
			var response = await SendMessageToAi(filtered);
			await message.Channel.SendMessageAsync(response);
		}
	}

	private readonly struct MessagePostData(string message, string behavior)
	{
		[JsonPropertyName("system_instruction")]
		public SystemInstruction SystemInstruction { get; init; } = new()
		{
			Parts = [new Part {Text = behavior}]
		};

		[JsonPropertyName("contents")]
		public Content Content { get; init; } = new()
		{
			Parts = [new Part {Text = message}]
		};
	}

	private readonly struct SystemInstruction
	{
		[JsonPropertyName("parts")]
		public List<Part> Parts { get; init; }
	}

	private readonly struct MessageResponse
	{
		[JsonPropertyName("candidates")]
		public List<Candidate> Candidates { get; init; }
	}

	private readonly struct Candidate
	{
		[JsonPropertyName("content")]
		public Content Content { get; init; }
	}

	private readonly struct Content
	{
		[JsonPropertyName("parts")]
		public List<Part> Parts { get; init; }
	}

	private readonly struct Part
	{
		[JsonPropertyName("text")]
		public string Text { get; init; }
	}
}