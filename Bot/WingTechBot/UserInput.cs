using System.Diagnostics;

namespace WingTechBot;

public static class UserInput
{
	public record ReceivedInput<T>(T Input, IMessage Message);
	///Prompts a text or thread channel for a message of a type, optionally with a condition.
	public static async Task<ReceivedInput<T>> Prompt<T>(IMessageChannel channel, string prompt, CancellationToken token, Predicate<T> condition = null)
	{
		await channel.SendMessageAsync(prompt);

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
				var value = (T)Convert.ChangeType(messageReceived.Content.ToLower(), typeof(T));

				if (condition != null && !condition(value))
				{
					await Task.Delay(100, token);
					continue;
				}

				
				return new ReceivedInput<T>(value, messageReceived);
			}
			catch
			{
				await channel.SendMessageAsync(prompt);
				continue;
			}
		}

		throw new TaskCanceledException();
	}

	public static async Task<string> StringPrompt(IMessageChannel channel, string prompt, CancellationToken token, Predicate<string> condition = null)
	{
		return (await Prompt(channel, prompt, token, condition)).Input;
	}

	public static async Task<char> CharPrompt(IMessageChannel channel, string prompt, CancellationToken token, Predicate<char> condition = null)
	{
		return (await Prompt(channel, prompt, token, condition)).Input;
	}

	public static async Task<string> MultipleChoice(IMessageChannel channel, string prompt, CancellationToken token, params string[] choices)
	{
		return await StringPrompt(channel, prompt, token, choices.Contains);
	}

	public static async Task<bool> PromptYN(IMessageChannel channel, string prompt, CancellationToken token)
	{
		string input = await StringPrompt(channel, $"{prompt} (y/n)", token, s => s is "y" or "n");
		return input == "y";
	}

	public static bool TryGetUser(IMessageChannel channel, IReadOnlyCollection<SocketGuildUser> availableUsers, string prompt, CancellationToken token, out IUser user)
	{
		string input = StringPrompt(channel, prompt, token).Result;
		user = availableUsers.FirstOrDefault(u => string.Equals(u.Username, input, StringComparison.InvariantCultureIgnoreCase));

		return user != null;
	}

	private static bool ValidUserMessage(IMessage message) => !message.Author.IsBot && !message.Author.IsWebhook;
}