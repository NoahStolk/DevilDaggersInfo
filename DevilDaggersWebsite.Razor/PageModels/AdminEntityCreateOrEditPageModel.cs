﻿using DevilDaggersWebsite.Dto.Admin;
using DevilDaggersWebsite.Entities;
using DevilDaggersWebsite.Enumerators;
using DevilDaggersWebsite.Razor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersWebsite.Razor.PageModels
{
	public class AdminEntityCreateOrEditPageModel<TEntity, TAdminDto> : AbstractAdminEntityPageModel<TEntity>
		where TEntity : class, IAdminUpdatableEntity<TAdminDto>, new()
		where TAdminDto : class
	{
		private TEntity? _entity;

		public AdminEntityCreateOrEditPageModel(ApplicationDbContext dbContext)
			: base(dbContext)
		{
			AssetModFileContentsList = RazorUtils.EnumToSelectList<AssetModFileContents>();
			AssetModTypesList = RazorUtils.EnumToSelectList<AssetModTypes>();

			CategoryList = RazorUtils.EnumToSelectList<CustomLeaderboardCategory>();
			CurrencyList = RazorUtils.EnumToSelectList<Currency>();

			AssetModList = DbContext.AssetMods.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();
			PlayerList = DbContext.Players.Select(p => new SelectListItem(p.PlayerName, p.Id.ToString())).ToList();
			SpawnsetFileList = DbContext.SpawnsetFiles.Select(sf => new SelectListItem(sf.Name, sf.Id.ToString())).ToList();
			TitleList = DbContext.Titles.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();
		}

		public List<SelectListItem> AssetModFileContentsList { get; }
		public List<SelectListItem> AssetModTypesList { get; }

		public List<SelectListItem> CategoryList { get; }
		public List<SelectListItem> CurrencyList { get; }

		public List<SelectListItem> AssetModList { get; }
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

			IQueryable<TEntity> query = DbSet.AsQueryable();
			if (typeof(TEntity) == typeof(Title))
				query = DbSet.Include(t => (t as Title)!.PlayerTitles);
			else if (typeof(TEntity) == typeof(AssetMod))
				query = DbSet.Include(t => (t as AssetMod)!.PlayerAssetMods);
			else if (typeof(TEntity) == typeof(Player))
				query = DbSet.Include(t => (t as Player)!.PlayerAssetMods).Include(t => (t as Player)!.PlayerTitles);

			_entity = query.FirstOrDefault(m => m.Id == id) ?? (TEntity?)new();

			AdminDto = _entity.Populate();

			return Page();
		}

		public IActionResult OnPost(int? id)
		{
			IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

			if (!ModelState.IsValid)
				return Page();

			if (AdminDto == null)
				throw new($"{nameof(AdminDto)} should not be null on POST.");

			Id = id;

			if (IsEditing)
			{
				_entity = DbSet.FirstOrDefault(m => m.Id == id);
				if (_entity == null)
					return NotFound();

				DbContext.Attach(_entity).State = EntityState.Modified;

				try
				{
					_entity.Edit(DbContext, AdminDto);
					DbContext.SaveChanges();

					_entity.CreateManyToManyRelations(DbContext, AdminDto);
					DbContext.SaveChanges();
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
					ModelState.AddModelError("AdminDto.Id", "Player already exists.");
					return Page();
				}

				if (AdminDto is AdminCustomLeaderboard customLeaderboard && DbContext.CustomLeaderboards.Any(cl => cl.SpawnsetFileId == customLeaderboard.SpawnsetFileId))
				{
					ModelState.AddModelError("AdminDto.SpawnsetFileId", "A leaderboard for this spawnset already exists.");
					return Page();
				}

				_entity = new();
				_entity.Create(DbContext, AdminDto);
				DbContext.SaveChanges();

				_entity.CreateManyToManyRelations(DbContext, AdminDto);
				DbContext.SaveChanges();
			}

			return RedirectToPage("./Index");
		}
	}
}
