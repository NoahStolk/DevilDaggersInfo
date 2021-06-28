﻿using DevilDaggersWebsite.Authorization;
using DevilDaggersWebsite.Dto.Titles;
using DevilDaggersWebsite.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersWebsite.Api
{
	[Route("api/titles")]
	[ApiController]
	public class TitlesController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public TitlesController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet]
		[Authorize(Policies.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<List<GetTitleDto>> GetTitles()
		{
			List<Title> titles = _dbContext.Titles
				.AsNoTracking()
				.Include(t => t.PlayerTitles)
				.ToList();

			return titles.ConvertAll(t => new GetTitleDto
			{
				Id = t.Id,
				Name = t.Name,
				PlayerIds = t.PlayerTitles.ConvertAll(pt => pt.PlayerId),
			});
		}

		[HttpPost]
		[Authorize(Policies.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
		public ActionResult AddTitle(AddTitleDto addTitle)
		{
			Title title = new()
			{
				Name = addTitle.Name,
			};
			_dbContext.Titles.Add(title);
			_dbContext.SaveChanges();

			UpdatePlayerTitles(addTitle.PlayerIds ?? new(), title.Id);
			_dbContext.SaveChanges();

			return Ok();
		}

		[HttpPut]
		[Authorize(Policies.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditTitle(EditTitleDto editTitle)
		{
			Title? title = _dbContext.Titles.FirstOrDefault(t => t.Id == editTitle.Id);
			if (title == null)
				return NotFound();

			title.Name = editTitle.Name;
			UpdatePlayerTitles(editTitle.PlayerIds ?? new(), title.Id);

			_dbContext.SaveChanges();

			return Ok();
		}

		[HttpDelete]
		[Authorize(Policies.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteTitle(int id)
		{
			Title? title = _dbContext.Titles
				.Include(t => t.PlayerTitles)
				.FirstOrDefault(t => t.Id == id);
			if (title == null)
				return NotFound();

			_dbContext.Titles.Remove(title);
			_dbContext.SaveChanges();

			return Ok();
		}

		private void UpdatePlayerTitles(List<int> playerIds, int titleId)
		{
			foreach (PlayerTitle newEntity in playerIds.ConvertAll(pi => new PlayerTitle { TitleId = titleId, PlayerId = pi }))
			{
				if (!_dbContext.PlayerTitles.Any(pam => pam.TitleId == newEntity.TitleId && pam.PlayerId == newEntity.PlayerId))
					_dbContext.PlayerTitles.Add(newEntity);
			}

			foreach (PlayerTitle entityToRemove in _dbContext.PlayerTitles.Where(pam => pam.TitleId == titleId && !playerIds.Contains(pam.PlayerId)))
				_dbContext.PlayerTitles.Remove(entityToRemove);
		}
	}
}
