using Chromely;
using DevilDaggersInfo.App.CustomLeaderboard.Chromely.Services;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using DevilDaggersInfo.Core.NativeInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevilDaggersInfo.App.CustomLeaderboard.Chromely;

public class App : ChromelyBasicApp
{
	public override void ConfigureServices(IServiceCollection services)
	{
		base.ConfigureServices(services);

		IConfiguration configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

		services.AddLogging(configure => configure.AddConsole());
		services.AddSingleton<IClientConfiguration, ClientConfiguration>();
		services.AddSingleton<IEncryptionService, EncryptionService>();
		services.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		services.AddSingleton<INativeMemoryService, NativeMemoryService>();
		services.AddSingleton<NetworkService>();
		services.AddSingleton<ReaderService>();
		services.AddSingleton<UploadService>();
		services.AddSingleton(configuration);
	}
}
