using DevilDaggersInfo.Web.Server;
using DevilDaggersInfo.Web.Server.HostedServices.DdInfoDiscordBot;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseSentry(o => o.TracesSampleRate = 1.0);

		webBuilder.UseStaticWebAssets();
		webBuilder.UseStartup<Startup>();
	})
	.ConfigureServices(services => services.AddHostedService<DiscordBotService>());

IHost app = builder.Build();
app.Run();
