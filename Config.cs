namespace WingTechBot;

public record Config
{
    public string LoginToken { get; set; }
    public ulong OwnerID { get; set; }
    
    public ulong ServerID { get; set; }
    public ulong? BotChannelID { get; set; }

    public ulong? ModRoleID { get; set; }
    public ulong? JesterRoleID { get; set; }
    public ulong? NaughtyRoleID { get; set; }
    public ulong? SlowmodeRoleID { get; set; }
    public ulong? DoodooRoleID { get; set; }
}