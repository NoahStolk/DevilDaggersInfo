// #define TEST_EXCEPTION_HANDLER
using DevilDaggersWebsite.Code.Database;
using DevilDaggersWebsite.Code.Tasks;
using DevilDaggersWebsite.Code.Tasks.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace DevilDaggersWebsite
{
	public class Startup
	{
		private const string defaultPolicy = nameof(defaultPolicy);

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy(defaultPolicy, builder => { builder.AllowAnyOrigin(); });
			});

			services.AddMvc();

			services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			// TODO: Add all tasks using reflection?
			// services.AddSingleton<IScheduledTask, RetrieveEntireLeaderboardTask>();
			services.AddSingleton<IScheduledTask, CreateLeaderboardHistoryFileTask>();

			services.AddScoped<IUrlHelper>(factory => new UrlHelper(factory.GetService<IActionContextAccessor>().ActionContext));

			services.AddScheduler((sender, args) =>
			{
				Console.Write(args.Exception?.Message);
				args.SetObserved();
			});

			services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.Converters.Add(new StringEnumConverter());
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			// Do not change order of redirects.
			RewriteOptions options = new RewriteOptions()
				.AddRedirect("^Home/Index$", "Index")
				.AddRedirect("^Home/Leaderboard$", "Leaderboard")
				.AddRedirect("^Home/Spawnset/(.*)$", "Spawnset?spawnset=$1")
				.AddRedirect("^Home/Spawnsets$", "Spawnsets")
				.AddRedirect("^Home/SpawnsetsInfo$", "Spawnsets")
				.AddRedirect("^Home/Spawnset$", "Spawnset")
				.AddRedirect("^Home/Hands$", "Wiki/Upgrades")
				.AddRedirect("^Home/Enemies$", "Wiki/Enemies")
				.AddRedirect("^Home/Daggers$", "Wiki/Daggers")
				.AddRedirect("^Home/Donations$", "Donations")
				.AddRedirect("^Home$", "Index")
				.AddRedirect("^Upgrades$", "Wiki/Upgrades")
				.AddRedirect("^Enemies$", "Wiki/Enemies")
				.AddRedirect("^Daggers$", "Wiki/Daggers")
				.AddRedirect("^GetSpawnsets$", "Api/GetSpawnsets")
				.AddRedirect("^GetToolVersions$", "Api/GetToolVersions")
				.AddRedirect("^LeaderboardJson$", "Api/LeaderboardJson")
				.AddRedirect("^Spawns$", "Wiki/Spawns")
				.AddRedirect("^Home/Spawns$", "Wiki/Spawns")
				.AddRedirect("^Wiki/SpawnsetGuide$", "Wiki/Guides/SurvivalEditor")
				.AddRedirect("^Wiki/AssetGuide$", "Wiki/Guides/AssetEditor")
				.AddRedirect("^DownloadSpawnset", "Api/DownloadSpawnset")
				.AddRedirect("^DownloadSpawnset?file=(.*)", "Api/DownloadSpawnset?file=$1");
			app.UseRewriter(options);

			if (env.IsDevelopment())
			{
#if TEST_EXCEPTION_HANDLER
				app.UseExceptionHandler("/Error");
#else
				app.UseDeveloperExceptionPage();
#endif
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseStatusCodePagesWithReExecute("/Error/{0}");
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseCors(defaultPolicy);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
			});
		}
	}
}