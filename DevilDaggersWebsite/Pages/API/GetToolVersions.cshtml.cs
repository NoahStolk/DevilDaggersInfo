﻿using CoreBase.Services;
using DevilDaggersWebsite.Models.API;
using DevilDaggersWebsite.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DevilDaggersWebsite.Pages.API
{
	[ApiFunction(Description = "Returns the list of tools including their version number.", ReturnType = MediaTypeNames.Application.Json)]
	public class GetToolVersionsModel : ApiPageModel
	{
		private readonly ICommonObjects _commonObjects;

		public GetToolVersionsModel(ICommonObjects commonObjects)
		{
			_commonObjects = commonObjects;
		}

		public FileResult OnGet()
		{
			return JsonFile(ToolUtils.Tools);
		}
	}
}