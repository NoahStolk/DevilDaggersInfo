using DevilDaggersInfo.App.CustomLeaderboard.NativeImpl.Windows;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using DevilDaggersInfo.Core.NativeInterface;
using DevilDaggersInfo.Razor.Core.CustomLeaderboard;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

IConfiguration configuration = new ConfigurationBuilder()
	//.AddJsonFile("appsettings.json")
	.Build();

builder.Services.AddSingleton<IClientConfiguration, ClientConfiguration>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
builder.Services.AddSingleton<INativeMemoryService, NativeMemoryService>();
builder.Services.AddSingleton<NetworkService>();
builder.Services.AddSingleton<ReaderService>();
builder.Services.AddSingleton<UploadService>();
builder.Services.AddSingleton(configuration);

await builder.Build().RunAsync();
