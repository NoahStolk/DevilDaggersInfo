﻿using DevilDaggersWebsite.Code.Utils;
using DevilDaggersWebsite.Core.Entities;
using DevilDaggersWebsite.Core.Enumerators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Pages.Admin.Donations
{
	public class CreateModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public CreateModel(ApplicationDbContext context)
		{
			_context = context;

			CurrencyList = RazorUtils.EnumToSelectList<Currency>();
		}

		public List<SelectListItem> CurrencyList { get; }

		[BindProperty]
		public Donation Donation { get; set; }

		public IActionResult OnGet() => Page();

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			_context.Donations.Add(Donation);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}