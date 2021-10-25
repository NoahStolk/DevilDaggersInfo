﻿using DevilDaggersWebsite.Dto;
using DevilDaggersWebsite.Entities;
using DevilDaggersWebsite.Transients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Io = System.IO;

namespace DevilDaggersWebsite.Api
{
	[Route("api/mods")]
	[ApiController]
	public class ModsController : ControllerBase
	{
		private readonly IWebHostEnvironment _environment;
		private readonly ApplicationDbContext _dbContext;
		private readonly ModHelper _modHelper;

		public ModsController(IWebHostEnvironment environment, ApplicationDbContext dbContext, ModHelper modHelper)
		{
			_environment = environment;
			_dbContext = dbContext;
			_modHelper = modHelper;
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[Obsolete("Use " + nameof(GetModsForDdae) + " instead. This is still in use by DDAE 1.0.0.0.")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public List<Mod> GetMods(string? authorFilter = null, string? nameFilter = null, bool? isHostedFilter = null)
			=> _modHelper.GetMods(authorFilter, nameFilter, isHostedFilter);

		[HttpGet("ddae")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public List<Mod> GetModsForDdae(string? authorFilter = null, string? nameFilter = null, bool? isHostedFilter = null)
			=> _modHelper.GetMods(authorFilter, nameFilter, isHostedFilter);

		[HttpGet("{modName}/file")]
		[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetModFile([Required] string modName)
		{
			if (!_dbContext.AssetMods.Any(am => am.Name == modName))
				return new NotFoundObjectResult(new ProblemDetails { Title = $"Mod '{modName}' was not found." });

			string fileName = $"{modName}.zip";
			string path = Path.Combine("mods", fileName);
			if (!Io.File.Exists(Path.Combine(_environment.WebRootPath, path)))
				return new BadRequestObjectResult(new ProblemDetails { Title = $"Mod file '{fileName}' does not exist." });

			return File(Io.File.ReadAllBytes(Path.Combine(_environment.WebRootPath, path)), MediaTypeNames.Application.Zip, fileName);
		}
	}
}