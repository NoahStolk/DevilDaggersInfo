﻿using DevilDaggersCore.Game;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersWebsite.Razor.Utils
{
	public static class RazorUtils
	{
		public const string DiscordUrl = "https://discord.gg/NF32j8S";

		public static HtmlString NAString { get; } = new("<span style='color: #444;'>N/A</span>");

		// TODO: Figure out how to properly add "asp-append-version".
		public static HtmlString GetCssList(IWebHostEnvironment environment, string subdirectory)
			=> GetList(environment, subdirectory, (sb, href) => sb.Append("<link rel='stylesheet' href='/").Append(href).Append("' />\n"));

		// TODO: Figure out how to properly add "asp-append-version".
		public static HtmlString GetJsList(IWebHostEnvironment environment, string subdirectory)
			=> GetList(environment, subdirectory, (sb, href) => sb.Append("<script defer src='/").Append(href).Append("'></script>\n"));

		private static HtmlString GetList(IWebHostEnvironment environment, string subdirectory, Action<StringBuilder, string> appendAction)
		{
			string directory = Path.Combine(environment.WebRootPath, subdirectory);

			StringBuilder sb = new();
			foreach (string path in Directory.GetFiles(directory))
				appendAction(sb, Path.Combine(subdirectory, Path.GetFileName(path)));

			return new(sb.ToString());
		}

		public static HtmlString GetCopyrightString(int startYear)
			=> new($"&copy; DevilDaggers.info {startYear}-{DateTime.UtcNow.Year}");

		public static HtmlString GetLayoutAnchor(this Enemy enemy, bool plural = false, GameVersion? gameVersionOverride = null)
		{
			GameVersion gameVersion = enemy.GameVersion;
			string colorCode = enemy.ColorCode;
			if (gameVersionOverride.HasValue)
			{
				gameVersion = gameVersionOverride.Value;
				colorCode = GameInfo.GetEnemies(gameVersionOverride.Value).First(e => e.Name == enemy.Name).ColorCode;
			}

			return new($"<a style='color: #{colorCode};' href='/Wiki/Enemies?GameVersion={gameVersion}#{enemy.Name.Replace(" ", string.Empty)}'>{enemy.Name}{(plural ? "s" : string.Empty)}</a>");
		}

		public static HtmlString GetLayoutAnchor(this Upgrade upgrade)
			=> new($"<a style='color: #{upgrade.ColorCode};' href='/Wiki/Upgrades?GameVersion={upgrade.GameVersion}#{upgrade.Name}'>{upgrade.Name}</a>");

		// TODO: Rewrite whole method. It's messy and not very performant.
		public static HtmlString GetLayout(string str, GameVersion? gameVersion = null)
		{
			char[] beginSeparators = new char[] { '>', ' ', ',', '.', '(', '-', '/' };
			char[] endSeparators = new char[] { ' ', ',', '.', 's', ')', '\'', ';', '/' };

			List<Enemy> enemies = GameInfo.GetEnemies(gameVersion ?? GameVersion.V31);

			// Use reverse iteration because transmuted skulls come after normal skulls in the list.
			for (int i = enemies.Count - 1; i >= 0; i--)
			{
				Enemy enemy = enemies[i];
				foreach (char begin in beginSeparators)
				{
					foreach (char end in endSeparators)
					{
						string enemyString = $"{begin}{enemy.Name}{end}";

						// Enemy string should not be inside an <a> element.
						if (str.Contains(enemyString) && (str.Length < str.IndexOf(enemyString) + enemyString.Length + "</a>".Length || str.Substring(str.IndexOf(enemyString) + enemyString.Length, "</a>".Length) != "</a>"))
							str = str.Replace(enemyString, $"{begin}{enemy.GetLayoutAnchor(end == 's')}{(end == 's' ? string.Empty : end.ToString())}");
					}
				}
			}

			foreach (Upgrade upgrade in GameInfo.GetUpgrades(gameVersion ?? GameVersion.V31))
			{
				foreach (char begin in beginSeparators)
				{
					foreach (char end in endSeparators)
					{
						string upgradeString = $"{begin}{upgrade.Name}{end}";
						if (str.Contains(upgradeString))
							str = str.Replace(upgradeString, $"{begin}{upgrade.GetLayoutAnchor()}{end}");
					}
				}
			}

			return new(str);
		}

		public static string TransmuteString(this string str)
		{
			return str
				.Replace(" transmute ", " <a style='color: var(--col-red);' href='/Wiki/Enemies#transmuted-skulls'>transmute</a> ")
				.Replace(" transmutes ", " <a style='color: var(--col-red);' href='/Wiki/Enemies#transmuted-skulls'>transmutes</a> ");
		}

		public static string ToIdString(this string str)
			=> $"{str.ToLower().Replace(" ", "-")}";

		public static string S(this int value)
			=> value == 1 ? string.Empty : "s";

		public static string GetCssWidth(int width)
			=> $"width: {width}px;";

		public static List<SelectListItem> EnumToSelectList<TEnum>(bool ignoreZero)
			where TEnum : Enum
		{
			IEnumerable<TEnum> enumValues = (IEnumerable<TEnum>)Enum.GetValues(typeof(TEnum));

			if (ignoreZero)
				enumValues = enumValues.Where(e => (int)(object)e != 0);

			return enumValues
				.Select(c => new SelectListItem
				{
					Text = c.ToString(),
					Value = ((int)(object)c).ToString(),
				})
				.ToList();
		}

		public static string ToUpperFirst(this string str)
		{
			if (str.Length == 0)
				return str;

			return $"{char.ToUpper(str[0])}{str[1..]}";
		}
	}
}