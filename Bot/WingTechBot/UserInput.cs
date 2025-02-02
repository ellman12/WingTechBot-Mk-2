namespace WingTechBot;

public static class UserInput
{
	///Prompts a text or thread channel for a message of a type.
	public static async Task<T> Prompt<T>(SocketTextChannel channel, string prompt, CancellationToken token)
	{
		await channel.SendMessageAsync(prompt);
		T value = default;

		while (!token.IsCancellationRequested)
		{
			var messageReceived = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();
			if (messageReceived == null || !ValidUserMessage(messageReceived))
			{
				await Task.Delay(100, token);
				Logger.LogLine("WAITING");
				continue;
			}

			try
			{
				value = (T)Convert.ChangeType(messageReceived.Content, typeof(T));
				break;
			}
			catch (Exception e)
			{
				await channel.SendMessageAsync(prompt);
			}
		}

		return value;
	}

	public static bool ValidUserMessage(IMessage message) => !message.Author.IsBot && !message.Author.IsWebhook;
}