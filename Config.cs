namespace WingTechBot;

public record Config
{
	public string LoginToken { get; set; }
	public ulong OwnerID { get; set; }

	public ulong ServerID { get; set; }
	public ulong? BotChannelID { get; set; }

	public ulong? ModRoleID { get; set; }
}