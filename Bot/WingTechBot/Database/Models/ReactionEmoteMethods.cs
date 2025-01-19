namespace WingTechBot.Database.Models;

public sealed partial class ReactionEmote 
{
	///Attempts to convert an emoji code like :eyes: or :thumbsup: into 👀 or 👍.
	public static string ConvertEmojiName(string name)
	{
		if (Emoji.TryParse(name, out Emoji emoji))
			name = emoji.Name;
		
		return name;
	}
	
}