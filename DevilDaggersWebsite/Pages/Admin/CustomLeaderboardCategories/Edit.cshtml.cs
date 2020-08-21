﻿using DevilDaggersWebsite.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Pages.Admin.CustomLeaderboardCategories
{
	public class EditModel : PageModel
	{
		private readonly ApplicationDbContext context;

		public EditModel(ApplicationDbContext context)
		{
			this.context = context;
		}

		[BindProperty]
		public CustomLeaderboardCategory CustomLeaderboardCategory { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
				return NotFound();

			CustomLeaderboardCategory = await context.CustomLeaderboardCategories.FirstOrDefaultAsync(m => m.Id == id);

			if (CustomLeaderboardCategory == null)
				return NotFound();
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			context.Attach(CustomLeaderboardCategory).State = EntityState.Modified;

			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CustomLeaderboardCategoryExists(CustomLeaderboardCategory.Id))
					return NotFound();
				else
					throw;
			}

			return RedirectToPage("./Index");
		}

		private bool CustomLeaderboardCategoryExists(int id) => context.CustomLeaderboardCategories.Any(e => e.Id == id);
	}
}