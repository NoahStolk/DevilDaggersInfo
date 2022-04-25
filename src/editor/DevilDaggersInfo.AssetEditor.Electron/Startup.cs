using ElectronNET.API;
using ElectronNET.API.Entities;

namespace DevilDaggersInfo.AssetEditor.Electron;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMvc();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseStaticFiles();

		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
		});

		if (HybridSupport.IsElectronActive)
		{
			CreateWindow();
		}
	}

	private async Task CreateWindow()
	{
		BrowserWindow browserWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
		{
			Width = 1152,
			Height = 940,
			Show = false,
		});

		await browserWindow.WebContents.Session.ClearCacheAsync();

		browserWindow.OnReadyToShow += () => browserWindow.Show();
		browserWindow.SetTitle("DevilDaggersAssetEditor");
	}
}
