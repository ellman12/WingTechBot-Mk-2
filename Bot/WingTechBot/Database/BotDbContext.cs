namespace WingTechBot.Database;

///Manages interfacing with WingTech Bot's database.
public sealed class BotDbContext : DbContext
{
    //Tables
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<ReactionEmote> ReactionEmotes { get; set; }
    
    ///Configures the database.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string host = Environment.GetEnvironmentVariable("DATABASE_HOST");
        string dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
        string user = Environment.GetEnvironmentVariable("DATABASE_USER");
        string password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        
        optionsBuilder.UseNpgsql($"Host={host}; Database={dbName}; Username={user}; Password={password}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //https://stackoverflow.com/a/53721519
        var assemblyWithConfigurations = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
    }
}
