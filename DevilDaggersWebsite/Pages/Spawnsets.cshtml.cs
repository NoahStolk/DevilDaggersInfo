﻿using CoreBase;
using CoreBase.Services;
using DevilDaggersWebsite.Models.Spawnset;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersWebsite.Pages
{
	public class SpawnsetsModel : PageModel
	{
		private readonly ICommonObjects _commonObjects;

		public PaginatedList<SpawnsetFile> SpawnsetFiles;

		public string NameSort { get; set; }
		public string AuthorSort { get; set; }
		public string SortOrder { get; set; }
		public int PageSize { get; set; } = 15;
		public int PageIndex { get; private set; }

		public SpawnsetsModel(ICommonObjects commonObjects)
		{
			_commonObjects = commonObjects;
		}

		public void OnGet(string sortOrder, int? pageIndex)
		{
			SortOrder = sortOrder;

			NameSort = sortOrder == "Name" ? "Name_asc" : "Name";
			AuthorSort = sortOrder == "Author_asc" ? "Author" : "Author_asc";

			List<string> spawnsetPaths = Directory.GetFiles(Path.Combine(_commonObjects.Env.WebRootPath, "spawnsets/")).ToList();

			List<SpawnsetFile> spawnsetFiles = new List<SpawnsetFile>();
			foreach (string spawnsetPath in spawnsetPaths)
			{
				string fileName = Path.GetFileName(spawnsetPath);

				spawnsetFiles.Add(new SpawnsetFile(fileName));
			}

			switch (sortOrder)
			{
				default:
				case "Name_asc":
					spawnsetFiles = spawnsetFiles.OrderBy(s => s.Name).ThenByDescending(s => s.Author).ToList();
					break;
				case "Name":
					spawnsetFiles = spawnsetFiles.OrderByDescending(s => s.Name).ThenBy(s => s.Author).ToList();
					break;
				case "Author_asc":
					spawnsetFiles = spawnsetFiles.OrderBy(s => s.Author).ThenByDescending(s => s.Name).ToList();
					break;
				case "Author":
					spawnsetFiles = spawnsetFiles.OrderByDescending(s => s.Author).ThenBy(s => s.Name).ToList();
					break;
			}

			PageIndex = pageIndex ?? 1;
			PageIndex = Math.Max(PageIndex, 1);
			PageIndex = Math.Min(PageIndex, (int)Math.Ceiling(spawnsetFiles.Count / (double)PageSize));

			SpawnsetFiles = PaginatedList<SpawnsetFile>.Create(spawnsetFiles, PageIndex, PageSize);
		}
	}
}