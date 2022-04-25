using DevilDaggersInfo.AssetEditor.Electron;
using ElectronNET.API;
using Microsoft.AspNetCore;

CreateWebHostBuilder(args).Build().Run();

static IWebHostBuilder CreateWebHostBuilder(string[] args)
{
	return WebHost.CreateDefaultBuilder(args)
		.ConfigureLogging((_, logging) => logging.AddConsole())
		.UseElectron(args)
		.UseStartup<Startup>();
}
