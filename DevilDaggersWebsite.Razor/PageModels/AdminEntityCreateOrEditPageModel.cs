﻿using DevilDaggersCore.Spawnsets;
using DevilDaggersDiscordBot.Logging;
using DevilDaggersWebsite.Dto.Admin;
using DevilDaggersWebsite.Entities;
using DevilDaggersWebsite.Enumerators;
using DevilDaggersWebsite.Razor.Extensions;
using DevilDaggersWebsite.Razor.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.PageModels
{
	public class AdminEntityCreateOrEditPageModel<TEntity, TAdminDto> : AbstractAdminEntityPageModel<TEntity>
		where TEntity : class, IAdminUpdatableEntity<TAdminDto>, new()
		where TAdminDto : class, IAdminDto
	{
		private readonly IWebHostEnvironment _env;

		private TEntity? _entity;

		public AdminEntityCreateOrEditPageModel(IWebHostEnvironment env, ApplicationDbContext dbContext)
			: base(dbContext)
		{
			_env = env;

			AssetModTypesList = RazorUtils.EnumToSelectList<AssetModTypes>(true);

			CategoryList = RazorUtils.EnumToSelectList<CustomLeaderboardCategory>(true);
			CurrencyList = RazorUtils.EnumToSelectList<Currency>(false);

			AssetModList = DbContext.AssetMods.Select(am => new SelectListItem(am.Name, am.Id.ToString())).ToList();
			CustomLeaderboardList = DbContext.CustomLeaderboards.Include(cl => cl.SpawnsetFile).Select(cl => new SelectListItem(cl.SpawnsetFile.Name, cl.Id.ToString())).ToList();
			PlayerList = DbContext.Players.Select(p => new SelectListItem(p.PlayerName, p.Id.ToString())).ToList();
			SpawnsetFileList = DbContext.SpawnsetFiles.Select(sf => new SelectListItem(sf.Name, sf.Id.ToString())).ToList();
			TitleList = DbContext.Titles.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
		}

		public List<SelectListItem> AssetModTypesList { get; }

		public List<SelectListItem> CategoryList { get; }
		public List<SelectListItem> CurrencyList { get; }

		public List<SelectListItem> AssetModList { get; }
		public List<SelectListItem> CustomLeaderboardList { get; }
		public List<SelectListItem> PlayerList { get; }
		public List<SelectListItem> SpawnsetFileList { get; }
		public List<SelectListItem> TitleList { get; }

		public int? Id { get; private set; }

		[BindProperty]
		public TAdminDto AdminDto { get; set; } = null!;

		public bool IsEditing => Id.HasValue;

		public IActionResult OnGet(int? id)
		{
			Id = id;

			_entity = GetFullQuery().FirstOrDefault(m => m.Id == id) ?? (TEntity?)new();

			AdminDto = _entity.Populate();

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

			if (!ModelState.IsValid)
				return Page();

			if (AdminDto == null)
				throw new($"{nameof(AdminDto)} should not be null on POST.");

			Id = id;

			AdminCustomLeaderboard? customLeaderboard = AdminDto as AdminCustomLeaderboard;
			AdminAssetMod? assetMod = AdminDto as AdminAssetMod;

			if (customLeaderboard != null)
			{
				string? invalidDaggerTime = customLeaderboard.ValidateTimes();
				if (invalidDaggerTime != null)
				{
					ModelState.AddModelError($"AdminDto.{invalidDaggerTime}", "The dagger times are incorrect.");
					return Page();
				}

				SpawnsetFile? clSpawnsetFile = DbContext.SpawnsetFiles.FirstOrDefault(sf => sf.Id == customLeaderboard.SpawnsetFileId);
				if (clSpawnsetFile == null)
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminCustomLeaderboard.SpawnsetFileId)}", $"A spawnset with ID '{customLeaderboard.SpawnsetFileId}' does not exist.");
					return Page();
				}

				if (!Spawnset.TryParse(System.IO.File.ReadAllBytes(Path.Combine(_env.WebRootPath, "spawnsets", clSpawnsetFile.Name)), out Spawnset spawnset))
					throw new($"Could not parse survival file '{clSpawnsetFile.Name}'. Please review the file. Also review how this file ended up in the 'spawnsets' directory, as it is not possible to upload non-survival files from within the Admin pages.");

				if (customLeaderboard.Category == CustomLeaderboardCategory.TimeAttack && spawnset.GameMode != GameMode.TimeAttack
				 || customLeaderboard.Category != CustomLeaderboardCategory.TimeAttack && spawnset.GameMode == GameMode.TimeAttack)
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminCustomLeaderboard.Category)}", $"Spawnset game mode is {spawnset.GameMode} while custom leaderboard category is {customLeaderboard.Category}.");
					return Page();
				}
			}

			if (assetMod != null && (assetMod.PlayerIds == null || assetMod.PlayerIds.Count == 0))
			{
				ModelState.AddModelError($"AdminDto.{nameof(AdminAssetMod.PlayerIds)}", "Mod should have at least one author.");
				return Page();
			}

			if (IsEditing)
			{
				_entity = GetFullQuery().FirstOrDefault(m => m.Id == id);
				if (_entity == null)
					return NotFound();

				DbContext.Attach(_entity).State = EntityState.Modified;

				try
				{
					StringBuilder auditLogger = new($"`EDIT` by `{this.GetIdentity()}` for `{typeof(TEntity).Name}` `{id}`\n");
					LogCreateOrEdit(auditLogger, _entity.Populate().Log(), AdminDto.Log());

					_entity.Edit(DbContext, AdminDto);
					DbContext.SaveChanges();

					_entity.CreateManyToManyRelations(DbContext, AdminDto);
					DbContext.SaveChanges();

					await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, auditLogger.ToString());
				}
				catch (DbUpdateConcurrencyException) when (!DbSet.Any(e => e.Id == _entity.Id))
				{
					return NotFound();
				}
			}
			else
			{
				if (AdminDto is AdminPlayer player && DbContext.Players.Any(p => p.Id == player.Id))
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminPlayer.Id)}", $"Player with {nameof(AdminPlayer.Id)} '{player.Id}' already exists.");
					return Page();
				}

				if (AdminDto is AdminSpawnsetFile spawnsetFile && DbContext.SpawnsetFiles.Any(sf => sf.Name == spawnsetFile.Name))
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminSpawnsetFile.Name)}", $"SpawnsetFile with {nameof(AdminSpawnsetFile.Name)} '{spawnsetFile.Name}' already exists.");
					return Page();
				}

				if (customLeaderboard != null && DbContext.CustomLeaderboards.Any(cl => cl.SpawnsetFileId == customLeaderboard.SpawnsetFileId))
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminCustomLeaderboard.SpawnsetFileId)}", "A leaderboard for this spawnset already exists.");
					return Page();
				}

				if (assetMod != null && DbContext.AssetMods.Any(am => am.Name == assetMod.Name))
				{
					ModelState.AddModelError($"AdminDto.{nameof(AdminAssetMod.Name)}", $"AssetMod with {nameof(AssetMod.Name)} '{assetMod.Name}' already exists.");
					return Page();
				}

				_entity = new();
				_entity.Create(DbContext, AdminDto);
				DbContext.SaveChanges();

				_entity.CreateManyToManyRelations(DbContext, AdminDto);
				DbContext.SaveChanges();

				StringBuilder auditLogger = new($"`CREATE` by `{this.GetIdentity()}` for `{typeof(TEntity).Name}` `{_entity.Id}`\n");
				LogCreateOrEdit(auditLogger, null, AdminDto.Log());

				await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, auditLogger.ToString());
			}

			return RedirectToPage("./Index");
		}

		private IQueryable<TEntity> GetFullQuery()
		{
			if (typeof(TEntity) == typeof(Title))
				return DbSet.Include(t => (t as Title)!.PlayerTitles);
			if (typeof(TEntity) == typeof(AssetMod))
				return DbSet.Include(t => (t as AssetMod)!.PlayerAssetMods);
			if (typeof(TEntity) == typeof(Player))
				return DbSet.Include(t => (t as Player)!.PlayerAssetMods).Include(t => (t as Player)!.PlayerTitles);
			return DbSet.AsQueryable();
		}
	}
}
