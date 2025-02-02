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

	private static bool ValidUserMessage(IMessage message) => !message.Author.IsBot && !message.Author.IsWebhook;
}