namespace WingTechBot;

public sealed class WingSounds
{
	public static async Task Create(Config config)
	{
		var builder = WebApplication.CreateBuilder();

		builder.Services.AddCors(options =>
		{
			options.AddPolicy("FrontendOrigin", policy => policy.WithOrigins(config.ServerUrl).AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowAnyHeader());
		});

		builder.WebHost.ConfigureKestrel(options =>
		{
			options.Listen(IPAddress.Any, 5000);
		});

		builder.Services.AddControllers();
		var app = builder.Build();

		app.UseCors("FrontendOrigin");
		app.MapControllers();
		await app.StartAsync();
	}
}