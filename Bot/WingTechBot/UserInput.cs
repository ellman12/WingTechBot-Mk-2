namespace WingTechBot;

public static class UserInput
{
	///Prompts a text or thread channel for a message of a type, optionally with a condition.
	public static async Task<T> Prompt<T>(SocketTextChannel channel, string prompt, CancellationToken token, Predicate<T> condition = null)
	{
		await channel.SendMessageAsync(prompt);
		T value = default;

		while (!token.IsCancellationRequested)
		{
			var messageReceived = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();
			if (messageReceived == null || !ValidUserMessage(messageReceived))
			{
				await Task.Delay(100, token);
				continue;
			}

			try
			{
				value = (T)Convert.ChangeType(messageReceived.Content.ToLower(), typeof(T));

				if (condition != null && !condition(value))
				{
					await Task.Delay(100, token);
					continue;
				}

				break;
			}
			catch (Exception e)
			{
				await channel.SendMessageAsync(prompt);
			}
		}

		return value;
	}

	public static async Task<string> Prompt(SocketTextChannel channel, string prompt, CancellationToken token, Predicate<string> condition = null)
	{
		return await Prompt<string>(channel, prompt, token, condition);
	}

	public static async Task<char> CharPrompt(SocketTextChannel channel, string prompt, CancellationToken token, Predicate<char> condition = null)
	{
		return await Prompt(channel, prompt, token, condition);
	}

	public static async Task<string> MultipleChoice(SocketTextChannel channel, string prompt, CancellationToken token, params string[] choices)
	{
		return await Prompt(channel, prompt, token, choices.Contains);
	}

	public static async Task<bool> PromptYN(SocketTextChannel channel, string prompt, CancellationToken token)
	{
		string input = await Prompt(channel, $"{prompt} (y/n)", token, s => s is "y" or "n");
		return input == "y";
	}

	public static bool TryGetUser(SocketTextChannel channel, IReadOnlyCollection<SocketGuildUser> availableUsers, string prompt, CancellationToken token, out IUser user)
	{
		string input = Prompt(channel, prompt, token).Result;
		user = availableUsers.FirstOrDefault(u => String.Equals(u.Username, input, StringComparison.InvariantCultureIgnoreCase));

		return user != null;
	}

	private static bool ValidUserMessage(IMessage message) => !message.Author.IsBot && !message.Author.IsWebhook;
}