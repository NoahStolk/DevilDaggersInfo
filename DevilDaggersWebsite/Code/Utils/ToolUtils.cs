﻿using DevilDaggersCore.Tools;
using System;
using System.Collections.Generic;

namespace DevilDaggersWebsite.Code.Utils
{
	public static class ToolUtils
	{
		public static List<Tool> Tools { get; set; } = new List<Tool>
		{
			new Tool
			{
				Name = "DevilDaggersSurvivalEditor",
				VersionNumber = "2.4.10.0",
				VersionNumberRequired = "2.4.4.0",
				ChangeLog = new List<ChangeLogEntry>
				{
					new ChangeLogEntry
					{
						VersionNumber = "2.4.10.0",
						Date = new DateTime(2019, 10, 22),
						Changes = new List<Change>
						{
							new Change("Added Changelog window."),
							new Change("Fixed the Update Recommended window not closing after clicking the download button."),
							new Change("Small changes related to maintenance of the website, as well as code refactoring and improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.4.4.0",
						Date = new DateTime(2019, 9, 1),
						Changes = new List<Change>
						{
							new Change("Removed \"Loop start\" column from the online spawnset list."),
							new Change("The Switch Enemy Type window now only displays enemy types that exist in the current spawn selection."),
							new Change("Removed minimum window size and added scrollbars that become active when the window size becomes smaller than 1366x768. This applies to the Main window and the Download Spawnset window."),
							new Change("Fixed spawn enemy text colour not always updating correctly."),
							new Change("Small layout improvements."),
							new Change("Improved logging."),
							new Change("Internal changes such as importing DDCL-specific code for memory scanning base functionality as preparation for possible custom leaderboard integration (DDCL probably won't be getting updates anymore), as well as general code refactoring and improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.4.0.0",
						Date = new DateTime(2019, 8, 5),
						Changes = new List<Change>
						{
							new Change("Added end loop preview which allows you to view spawn information for any wave. This includes Gigapedes changing into Ghostpedes every third wave. The end loop preview can also be turned off in the Settings window."),
							new Change("Added ability to increment/decrement tile heights by 0.1 while holding CTRL and using the mouse wheel."),
							new Change("Added functionality to auto-detect the location of the survival file within the Settings window."),
							new Change("Recreated layout for Settings window."),
							new Change("Optimised performance when deleting spawns."),
							new Change("Added \"clear search\" buttons to Download Spawnset window."),
							new Change("Added \"leaderboard\" column to the spawnset list in the Download Spawnset window."),
							new Change("Improved Download Spawnset window layout."),
							new Change("Added tooltips that display a spawnset's description if it has one."),
							new Change("Fixed log file not being written to."),
							new Change("Other small layout changes, bug fixes, and improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.3.1.0",
						Date = new DateTime(2019, 7, 30),
						Changes = new List<Change>
						{
							new Change("Fixed crash when deleting multiple spawns.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.3.0.0",
						Date = new DateTime(2019, 7, 30),
						Changes = new List<Change>
						{
							new Change("Heavily optimised performance for the spawns section."),
							new Change("Added author sorting to the Download Spawnset window."),
							new Change("Fixed the application not asking whether you want to save the current spawnset when closing the application."),
							new Change("Fixed Modify Spawn Delay window being able to change spawn delay values into negative values."),
							new Change("The application now displays the Update Recommended window on start up when an update is available."),
							new Change("Layout improvements for the Download Spawnset window.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.2.0.0",
						Date = new DateTime(2019, 7, 7),
						Changes = new List<Change>
						{
							new Change("The application now keeps track of whether or not you have any unsaved changes and will ask you whether or not you want to to save it before proceeding to overwrite it by opening an existing spawnset or creating a new one."),
							new Change("The application window title now displays the current spawnset's name if it has one."),
							new Change("Added Save As menu item."),
							new Change("Added shortcut keys:")
							{
								SubChanges = new List<Change>
								{
								   new Change("CTRL+S - Save"),
								   new Change("CTRL+C - Copy currently selected spawn(s)"),
								   new Change("CTRL+V - Paste spawn(s) currently on the clipboard"),
								   new Change("Delete - Delete currently selected spawn(s)")
								}
							},
							new Change("The Download Spawnset window now remembers the spawnset sorting after it is closed."),
							new Change("The application now asks you to confirm to overwrite the existing arena with a preset, as this cannot be undone easily."),
							new Change("The maximum amount of spawns you can have per spawnset is now set to 10,000."),
							new Change("Improved messages when saving or replacing spawnsets."),
							new Change("Fixed the survival file restore writing the original file bytes on top of the file instead of overwriting it entirely."),
							new Change("Fixed the application not displaying an \"unsaved changes\" warning message when opening the currently active survival file."),
							new Change("Fixed the end loop not being displayed correctly when there are no EMPTY spawns in the spawnset."),
							new Change("Performance optimisations, layout improvements, and other bug fixes.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.1.1.0",
						Date = new DateTime(2019, 7, 5),
						Changes = new List<Change>
						{
							new Change("Hotfix for restoring the default survival file.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.1.0.0",
						Date = new DateTime(2019, 7, 5),
						Changes = new List<Change>
						{
							new Change("Added spawnset sorting to the Download Spawnset window."),
							new Change("Added Select All and Deselect All buttons to the arena editor."),
							new Change("Added copy/paste functionality to the spawns editor."),
							new Change("Added timeout for web requests so the application doesn't keep waiting when the website is offline."),
							new Change("Fixed tile elements not updating colours after rounding or randomising."),
							new Change("The spawns editor scrollbar now scrolls to the end when adding new spawns."),
							new Change("The Download Spawnset window now remembers the author and spawnset search values after it is closed."),
							new Change("The default survival file is now embedded into the executable so the actual file is not needed anymore. This removes the issue where the application crashes whenever the file would not be present."),
							new Change("Optimisations and layout improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "2.0.0.0",
						Date = new DateTime(2019, 6, 29),
						Changes = new List<Change>
						{
							new Change("General")
							{
								SubChanges = new List<Change>
								{
									new Change("Made the main application window resizable (includes fullscreen)."),
									new Change("Added loading screen."),
									new Change("Made user input more lenient, less errors will be thrown and the application will just deal with input values even if they don't make much sense.")
								}
							},
							new Change("Fixes")
							{
								SubChanges = new List<Change>
								{
									new Change("Fixed issue where the arena was always incorrectly rotated and mirrored."),
									new Change("Fixed issue where restoring the original V3 spawnset would remove the survival file from the application's 'Content' folder and as a result the option no longer works until the file is put back (either by re-downloading the application or by doing it manually)."),
									new Change("Fixed issue where the application would not start up again after using it (due to the issue above)."),
									new Change("Fixed issue where you could create spawns with a negative delay value.")
								}
							},
							new Change("Spawns")
							{
								SubChanges = new List<Change>
								{
									new Change("Added ability to modify (add, subtract, multiply, divide) delays for selected spawns. This can be used to easily speed up or slow down parts of a spawnset, or a spawnset in its entirety."),
									new Change("Added ability to switch enemy types for selected spawns."),
									new Change("Added ability to add or insert the same spawn multiple times at once using the Amount value.")
								}
							},
							new Change("Arena")
							{
								SubChanges = new List<Change>
								{
									new Change("Set the max tile height to 54 rather than 63, since any tiles with a height value greater than 54 will be in complete darkness and won't be visible (regardless of the spawnset brightness setting, or the in-game gamma setting)."),
									new Change("The arena editor now shows void tile heights as \"Void\" rather than their actual (meaningless) value."),
									new Change("Added height selector which lets you pick a height and use it in the arena editor."),
									new Change("Removed the old height map as this is now redundant."),
									new Change("Added multiple tile selection to the arena editor."),
									new Change("Added continuous tile modification and selection to the arena editor."),
									new Change("Added rectangular tile modification and selection to the arena editor."),
									new Change("Added ability to round heights for selected tiles."),
									new Change("Added ability to randomise heights for selected tiles."),
									new Change("Added ability to rotate and flip the arena."),
									new Change("Made the tiles brighter for better visibility."),
									new Change("Optimised the shrink preview slider for better performance."),
									new Change("Implemented custom pixel shading for the arena editor to take advantage of high-performant GPU rendering to render lighting, selection borders and selection highlighting.")
								}
							},
							new Change("Arena presets")
							{
								SubChanges = new List<Change>
								{
									new Change("Renamed Pyramid preset to Qbert."),
									new Change("Renamed Cage preset to Cage Rectangular."),
									new Change("Renamed Random preset to Random Noise."),
									new Change("Added new arena presets:")
									{
										SubChanges = new List<Change>
										{
											new Change("Cage Ellipse"),
											new Change("Ellipse"),
											new Change("Hill"),
											new Change("Pyramid"),
											new Change("Random Gaps"),
											new Change("Random Islands"),
											new Change("Random Pillars")
										}
									},
									new Change("Removed Default Flat preset, as it can now be created using the new Ellipse preset, or using the Round Heights button on the Default arena."),
									new Change("Added wall thickness parameter to Cage Rectangular preset."),
									new Change("Added offset parameters to Qbert preset."),
									new Change("Added option for arena presets whether to overwrite the previous arena entirely or to generate new tiles on top of it."),
								}
							},
							new Change("Menu")
							{
								SubChanges = new List<Change>
								{
									new Change("Replaced the Open From DevilDaggers.info menu item with a new Download Spawnset window which contains the spawnsets list."),
									new Change("Added menu item to open the current survival file.")
								}
							},
							new Change("Spawnsets from DevilDaggers.info")
							{
								SubChanges = new List<Change>
								{
									new Change("Added search and filter options to the spawnsets list."),
									new Change("The spawnsets list now shows more information (such as when the loop starts) about the spawnsets.")
								}
							},
							new Change("Miscellaneous")
							{
								SubChanges = new List<Change>
								{
									new Change("Added various warnings:")
									{
										SubChanges = new List<Change>
										{
											new Change("The application warns you when the spawnset you're creating might cause Devil Daggers to become unstable, for instance when the end loop is very short, or when you're spawning the player in the void. This also includes the new discovery of the {25, 27} tile, which causes Devil Daggers to glitch whenever its height is put to a value greater than 0.4973333."),
											new Change("The application warns you when the path to the survival file in the user settings is incorrect, or when the file could not be parsed.")
										}
									},
									new Change("Added more settings:")
									{
										SubChanges = new List<Change>
										{
											new Change("Prevent the player from spawning in the void by making sure the spawn tile always has a height."),
											new Change("Prevent tile {25, 27} from going outside of its safe range.")
										}
									},
									new Change("The application now uses logging, so whenever it crashes you can open the log to see what went wrong."),
									new Change("The application is now dependent on DevilDaggersCore, which is a .NET Standard class library used to share code between various Devil Daggers related applications.")
								}
							},
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.1.5.0",
						Date = new DateTime(2018, 11, 4),
						Changes = new List<Change>
						{
							new Change("Added functionality to automatically check for new versions of the program.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.1.4.0",
						Date = new DateTime(2018, 8, 5),
						Changes = new List<Change>
						{
							new Change("Downloading spawnsets and retrieving the spawnset list is now done asynchronously so it doesn't block the application."),
							new Change("Added functionality to reload the spawnset list if there was no internet connection or if the site was unresponsive."),
							new Change("Various fixes and small improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.1.2.0",
						Date = new DateTime(2018, 7, 27),
						Changes = new List<Change>
						{
							new Change("Added functionality to download spawnsets directly from DevilDaggers.info within the menu.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.0.2.0",
						Date = new DateTime(2018, 7, 26),
						Changes = new List<Change>
						{
							new Change("Enforced en-US globalisation.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.0.1.0",
						Date = new DateTime(2018, 6, 25),
						Changes = new List<Change>
						{
							new Change("Fixed not being able to read some spawnsets made using a hex editor when reading an undefined enemy type.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "1.0.0.0",
						Date = new DateTime(2018, 6, 16),
						Changes = new List<Change>
						{
							new Change("Initial release.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.9.0.0",
						Date = new DateTime(2018, 5, 14),
						Changes = new List<Change>
						{
							new Change("Beta release.")
						}
					}
				}
			},
			new Tool
			{
				Name = "DDCL",
				VersionNumber = "0.4.3.0",
				VersionNumberRequired = "0.4.0.1",
				ChangeLog = new List<ChangeLogEntry>
				{
					new ChangeLogEntry
					{
						VersionNumber = "0.4.3.0",
						Date = new DateTime(2019, 8, 8),
						Changes = new List<Change>
						{
							new Change("Improved way of detecting survival file cheats; there is no need to record the entire run anymore."),
							new Change("Fixed log file not being written to."),
							new Change("Implemented \"Reverse\" category leaderboards.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.4.0.1",
						Date = new DateTime(2019, 6, 5),
						Changes = new List<Change>
						{
							new Change("Compatibility update due to some internal bug fixes which aren't related to DDCL directly.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.4.0.0",
						Date = new DateTime(2019, 5, 27),
						Changes = new List<Change>
						{
							new Change("Leaderboards are now secured with the Advanced Encryption Standard (AES).")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.3.3.0",
						Date = new DateTime(2019, 5, 24),
						Changes = new List<Change>
						{
							new Change("Fixed inconsistent spawnset hashing. The hashing system is no longer dependent on files. This fixes the problem where some spawnsets wouldn't work if they were downloaded directly from the website rather than imported via the editor.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.3.0.0",
						Date = new DateTime(2019, 5, 20),
						Changes = new List<Change>
						{
							new Change("The program now tells you when there is an update available and warns you when the current version is no longer accepted by the server.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.2.5.0",
						Date = new DateTime(2019, 5, 19),
						Changes = new List<Change>
						{
							new Change("Added logging."),
							new Change("Some improvements in the layout and better feedback for when runs don't upload."),
							new Change("Crash fixes and internal clean up. The \"out of bounds\" error shouldn't occur anymore when starting DDCL before starting Devil Daggers."),
							new Change("Console is no longer resizable so it doesn't mess with the layout."),
							new Change("Added a retry count for when the upload fails. Usually it retries 3 times and stops after that, waiting for you to restart a run.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.2.1.0",
						Date = new DateTime(2019, 5, 18),
						Changes = new List<Change>
						{
							new Change("Lots of internal clean up, improvements, and fixes."),
							new Change("Program now outputs what values it submits to the server."),
							new Change("Program only retrieves the spawnset hash during the first second of the run so people cannot cheat by changing the survival file during the run. If you start the program later than 1 second after the run starts, the hash will not be calculated and your submission will be marked as invalid and not upload."),
							new Change("The server now has a minimal version it will accept submissions from.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.1.10.0",
						Date = new DateTime(2019, 5, 18),
						Changes = new List<Change>
						{
							new Change("Small fixes.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.1.9.0",
						Date = new DateTime(2019, 5, 18),
						Changes = new List<Change>
						{
							new Change("Application now uses .NET Framework 4.6.1 rather than 4.7.2."),
							new Change("Fixed bug where level up values don't reset when you restart a run."),
							new Change("Prevented replays from uploading so people won't submit runs to the wrong leaderboard by intentionally replacing the survival file during the replay.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.1.5.0",
						Date = new DateTime(2019, 5, 17),
						Changes = new List<Change>
						{
							new Change("Made application Windows-only because scanning memory for other operating systems will work differently anyway."),
							new Change("Enforced en-US culture to fix broken submissions on PCs that use commas as decimal separators."),
							new Change("Fixed usernames being limited to 4 characters."),
							new Change("Prevented submissions with 0.0000 time from uploading by setting a time constraint of a minimum of 2.5 seconds.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.1.0.0",
						Date = new DateTime(2019, 5, 15),
						Changes = new List<Change>
						{
							new Change("Initial release.")
						}
					}
				}
			},
			new Tool
			{
				Name = "DevilDaggersAssetEditor",
				VersionNumber = "0.3.3.0",
				VersionNumberRequired = "0.1.0.0",
				ChangeLog = new List<ChangeLogEntry>
				{
					// Implemented shader, model, and texture compression. Features such as compressing and using mod files for "dd" and "core" are now available and functional.
					new ChangeLogEntry
					{
						VersionNumber = "0.3.3.0",
						Date = new DateTime(2019, 9, 23),
						Changes = new List<Change>
						{
							new Change("Created progress bar for lengthy tasks such as compressing and extracting."),
							new Change("Many bug fixes."),
							new Change("Improved audio descriptions.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.3.0.0",
						Date = new DateTime(2019, 9, 21),
						Changes = new List<Change>
						{
							new Change("Implemented particle extraction, compression, and mod files. The particle files are in binary, so there is not much to mod until I figure out what the bytes mean, but you can switch particles and replace them with others."),
							new Change("Implemented functionality to specify whether or not to use relative paths in mod files."),
							new Change("Added user settings to specify Devil Daggers root folder, mods root folder, and assets root folder."),
							new Change("Improve initial directories for file and folder dialogs."),
							new Change("Small bug and crash fixes."),
							new Change("The mod file format has been changed, and mod files created using version 0.2.0.0 will no longer open. You'll need to convert these by navigating to Compatibility > Convert 0.2.0.0 mod file format and select your mod file.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.2.0.0",
						Date = new DateTime(2019, 9, 19),
						Changes = new List<Change>
						{
							new Change("Loudness is now exported as a .ini file rather than a .wav file."),
							new Change("Added functionality to export the current loudness values."),
							new Change("Added ability to save and open \"mod\" files (\".audio\" extension for audio mods) which are files containing the asset paths and loudness values. This removes the need to import/export loudness values when closing the application and is way more convenient than having to extract and compress huge files every time as well. Note that the \"mod\" files only contain local paths, so sharing them will not work."),
							new Change("Fixed not being able to type decimal values in the loudness fields."),
							new Change("Added support to automatically check for new versions of the program."),
							new Change("Added icon."),
							new Change("Added About window and other menu items."),
							new Change("Added logging."),
							new Change("Added description for audio assets (WIP)."),
							new Change("Many GUI and code improvements.")
						}
					},
					new ChangeLogEntry
					{
						VersionNumber = "0.1.0.0",
						Date = new DateTime(2019, 9, 11),
						Changes = new List<Change>
						{
							new Change("Initial release.")
						}
					}
				}
			}
		};
	}
}