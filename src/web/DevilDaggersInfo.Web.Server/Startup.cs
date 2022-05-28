using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Web.Server.Caches.LeaderboardHistory;
using DevilDaggersInfo.Web.Server.Caches.LeaderboardStatistics;
using DevilDaggersInfo.Web.Server.Caches.ModArchives;
using DevilDaggersInfo.Web.Server.Caches.SpawnsetHashes;
using DevilDaggersInfo.Web.Server.Caches.SpawnsetSummaries;
using DevilDaggersInfo.Web.Server.Clients.Clubber;
using DevilDaggersInfo.Web.Server.HostedServices;
using DevilDaggersInfo.Web.Server.Middleware;
using DevilDaggersInfo.Web.Server.NSwag;
using DevilDaggersInfo.Web.Server.RewriteRules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema;
using NSwag;
using System.Globalization;

namespace DevilDaggersInfo.Web.Server;

public class Startup
{
	private const string _defaultCorsPolicy = nameof(_defaultCorsPolicy);

	public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
	{
		Configuration = configuration;
		WebHostEnvironment = webHostEnvironment;
	}

	public IConfiguration Configuration { get; }
	public IWebHostEnvironment WebHostEnvironment { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddCors(options => options.AddPolicy(_defaultCorsPolicy, builder => builder.AllowAnyOrigin().AllowAnyMethod()));

		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), MySqlServerVersion.LatestSupportedServerVersion, providerOptions => providerOptions.EnableRetryOnFailure(5)));

		services.AddDatabaseDeveloperPageExceptionFilter();

		services.AddControllersWithViews();

		services.AddRazorPages();

		services.AddTransient<IToolService, ToolService>();

		services.AddTransient<ModArchiveAccessor>();
		services.AddTransient<ModArchiveProcessor>();
		services.AddTransient<ModScreenshotProcessor>();
		services.AddTransient<CustomLeaderboardValidatorService>();
		services.AddTransient<AuditLogger>();

		services.AddScoped<CustomEntryProcessor>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ProfileService>();

		services.AddSingleton<BackgroundServiceMonitor>();
		services.AddSingleton<LogContainerService>();
		services.AddSingleton<ResponseTimeMonitor>();
		services.AddSingleton<LeaderboardResponseParser>();

		services.AddSingleton<IFileSystemService, FileSystemService>();

		services.AddSingleton<LeaderboardHistoryCache>();
		services.AddSingleton<LeaderboardStatisticsCache>();
		services.AddSingleton<ModArchiveCache>();
		services.AddSingleton<SpawnsetSummaryCache>();
		services.AddSingleton<SpawnsetHashCache>();

		services.AddHttpClient<ClubberClient>();
		services.AddHttpClient<LeaderboardClient>();

		// Register this background service first, so it exists last. We want to log when the application exits.
		services.AddHostedService<DiscordLogFlushBackgroundService>();

		services.AddDataProtection()
			.PersistKeysToFileSystem(new DirectoryInfo("keys"));

		if (!WebHostEnvironment.IsDevelopment())
		{
			services.AddHostedService<DiscordUserIdFetchBackgroundService>();
			services.AddHostedService<LeaderboardHistoryBackgroundService>();
			services.AddHostedService<PlayerNameFetchBackgroundService>();
			services.AddHostedService<ResponseTimesBackgroundService>();
		}

		// Hosted service that runs once after startup.
		services.AddHostedService<StartupCacheHostedService>();

		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(jwtBearerOptions =>
			{
				jwtBearerOptions.RequireHttpsMetadata = true;
				jwtBearerOptions.SaveToken = true;
				jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtKey"])),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero,
				};
			});

		AddSwaggerDocument("Public", "This is the main API for DevilDaggers.info. Specifications may change based on the website client requirements. Use at your own risk.");
		AddSwaggerDocument("Dd", "This API is intended to be used by Devil Daggers only.");
		AddSwaggerDocument("Ddse", "This API is intended to be used by Devil Daggers Survival Editor only.");

		void AddSwaggerDocument(string apiNamespace, string description)
		{
			services.AddSwaggerDocument(config =>
			{
				config.PostProcess = document =>
				{
					document.Info.Title = $"DevilDaggers.Info API ({apiNamespace.ToUpper()})";
					document.Info.Description = description;
					document.Info.Contact = new()
					{
						Name = "Noah Stolk",
						Url = "//noahstolk.com/",
					};
				};
				config.DocumentName = apiNamespace.ToUpper();
				config.OperationProcessors.Insert(0, new ApiOperationProcessor(apiNamespace));
				config.SchemaType = SchemaType.OpenApi3;
				config.GenerateEnumMappingDescription = true;
			});
		}
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
	{
		Stopwatch sw = Stopwatch.StartNew();

		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

		app.UseMiddleware<ResponseTimeMiddleware>();

		// Do not change order of redirects.
		RewriteOptions options = new RewriteOptions()

			// V3
			.AddRedirect("^Home/Index$", "/")
			.AddRedirect("^Home$", "/")
			.AddRedirect("^Home/Leaderboard$", "leaderboard")
			.AddRedirect("^Home/Spawnsets$", "custom/spawnsets")
			.AddRedirect("^Home/Donations$", "donations")

			// V3 wiki
			.AddRedirect("^Home/Hands$", "wiki/upgrades")
			.AddRedirect("^Home/Enemies$", "wiki/enemies")
			.AddRedirect("^Home/Daggers$", "wiki/daggers")
			.AddRedirect("^Home/Spawns$", "wiki/spawns")
			.AddRedirect("^Daggers$", "wiki/daggers")
			.AddRedirect("^Enemies$", "wiki/enemies")
			.AddRedirect("^Spawns$", "wiki/spawns")
			.AddRedirect("^Upgrades$", "wiki/upgrades")

			// V4 outdated
			.AddRedirect("^Wiki/SpawnsetGuide$", "guides/creating-spawnsets")
			.AddRedirect("^Wiki/AssetGuide$", "guides/creating-mods")
			.AddRedirect("^Leaderboard/UserSettings", "leaderboard/player-settings")

			// V4
			.AddRedirect("^Leaderboard/PlayerSettings", "leaderboard/player-settings")
			.AddRedirect("^Leaderboard/WorldRecordProgression", "leaderboard/world-record-progression")
			.AddRedirect("^CustomLeaderboards$", "custom/leaderboards")
			.AddRedirect("^Mods", "custom/mods")
			.AddRedirect("^Spawnsets", "custom/spawnsets")
			.AddRedirect("^Tools/DevilDaggersAssetEditor", "tools/asset-editor")
			.AddRedirect("^Tools/DevilDaggersCustomLeaderboards", "tools/custom-leaderboards")
			.AddRedirect("^Tools/DevilDaggersSurvivalEditor", "tools/survival-editor")
			.AddRedirect("^Wiki/Guides/SurvivalEditor$", "guides/creating-spawnsets")
			.AddRedirect("^Wiki/Guides/AssetEditor$", "guides/creating-mods")
			.AddRedirect("^guides/survival-editor", "guides/creating-spawnsets")
			.AddRedirect("^guides/asset-editor", "guides/creating-mods")

			.Add(new CustomLeaderboardPageRewriteRules())
			.Add(new ModPageRewriteRules())
			.Add(new PlayerPageRewriteRules())
			.Add(new SpawnsetPageRewriteRules());

		app.UseRewriter(options);

		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseMigrationsEndPoint();
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseBlazorFrameworkFiles();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseCors(_defaultCorsPolicy);

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapRazorPages();
			endpoints.MapControllers();
			endpoints.MapFallbackToPage("/_Host");
		});

		app.UseOpenApi();
		app.UseSwaggerUi3();

		if (!env.IsDevelopment())
		{
			StringBuilder sb = new();
			sb.Append("Configuration done at ").AppendLine(TimeUtils.TicksToTimeString(sw.ElapsedTicks));

#if ROLES
		CreateRolesIfNotExist(serviceProvider);
		sb.Append("Role initiation done at ").AppendLine(TimeUtils.TicksToTimeString(sw.ElapsedTicks));
#endif

			sb.Append("> **Application is now online in the `").Append(env.EnvironmentName).AppendLine("` environment.**");

			LogContainerService lcs = serviceProvider.GetRequiredService<LogContainerService>();
			lcs.Add($"{DateTime.UtcNow:HH:mm:ss.fff}: Starting...\n{sb}");
		}

		sw.Stop();
	}

#if ROLES
	private static void CreateRolesIfNotExist(IServiceProvider serviceProvider)
	{
		using ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
		bool anyChanges = false;
		foreach (string roleName in Roles.All)
		{
			if (!dbContext.Roles.Any(r => r.Name == roleName))
			{
				dbContext.Roles.Add(new RoleEntity { Name = roleName });
				anyChanges = true;
			}
		}

		if (anyChanges)
			dbContext.SaveChanges();
	}
#endif
}
