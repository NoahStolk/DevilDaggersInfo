﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevilDaggersWebsite.Code.Database
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<AssetMod> AssetMods { get; set; }
		public DbSet<CustomEntry> CustomEntries { get; set; }
		public DbSet<CustomLeaderboard> CustomLeaderboards { get; set; }
		public DbSet<CustomLeaderboardCategory> CustomLeaderboardCategories { get; set; }
		public DbSet<Donation> Donations { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<SpawnsetFile> SpawnsetFiles { get; set; }
		public DbSet<Title> Titles { get; set; }

		public DbSet<PlayerAssetMod> PlayerAssetMods { get; set; }
		public DbSet<PlayerTitle> PlayerTitles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PlayerAssetMod>()
				.HasKey(pam => new { pam.PlayerId, pam.AssetModId });

			modelBuilder.Entity<PlayerAssetMod>()
				.HasOne(pam => pam.Player)
				.WithMany(p => p.PlayerAssetMods)
				.HasForeignKey(pam => pam.PlayerId);

			modelBuilder.Entity<PlayerAssetMod>()
				.HasOne(pam => pam.AssetMod)
				.WithMany(am => am.PlayerAssetMods)
				.HasForeignKey(pam => pam.AssetModId);

			modelBuilder.Entity<PlayerTitle>()
				.HasKey(pt => new { pt.PlayerId, pt.TitleId });

			modelBuilder.Entity<PlayerTitle>()
				.HasOne(pt => pt.Player)
				.WithMany(p => p.PlayerTitles)
				.HasForeignKey(pt => pt.PlayerId);

			modelBuilder.Entity<PlayerTitle>()
				.HasOne(pt => pt.Title)
				.WithMany(t => t.PlayerTitles)
				.HasForeignKey(pt => pt.TitleId);

			base.OnModelCreating(modelBuilder);
		}
	}
}