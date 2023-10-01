using BepInEx;
using Kittehface.Framework20;
using Menu;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;
using Achid = RainWorld.AchievementID;
using Procid = ProcessManager.ProcessID;
//using Kittehface.Framework20;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace AchievementTracker;

[BepInPlugin("timofey26.achievementtracker", "Achievement Tracker", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
	public static List<Achid> achievements;
	public static bool loadError;
	public static Hud hud;
	public static AchMenu menu;
	public static TrackerOptions options;
	public static readonly Procid[] processids =
	{
		Procid.Game,
		Procid.DeathScreen,
		Procid.StarveScreen,
		Procid.Statistics,
		Procid.FastTravelScreen,
		Procid.PauseMenu,
		Procid.SleepScreen,
		Procid.KarmaToMaxScreen,

	};

	public static string Filepath { get { return Custom.LegacyRootFolderDirectory() + string.Concat(new string[]
		{
			Path.DirectorySeparatorChar.ToString(),
			"UserData",
			Path.DirectorySeparatorChar.ToString(),
			"ahievementsTracker.txt"
		}).ToLowerInvariant(); } }

	public static bool colorize;
	public static bool ShowAchieved;
	public static bool ShowHidden;
	public static bool ShowUnAchieved;
	public static bool sort;

	public Plugin()
	{
		//options = new TrackerOptions(this);
	}

	public void OnEnable()
	{
		colorize = false;
		ShowAchieved = true;
		ShowUnAchieved = true;
		ShowHidden = false;
		sort = false;
		/*file folder: ((this.legacySaveFileIndex == 0) ? "sav" : ("sav_" + (this.legacySaveFileIndex + 1).ToString()));
		 or just

		Custom.LegacyRootFolderDirectory() + string.Concat(new string[]
		{
			Path.DirectorySeparatorChar.ToString(),
			"UserData",
			Path.DirectorySeparatorChar.ToString(),
			text,
			".txt"
		}).ToLowerInvariant();
		 
		 */
		On.RainWorld.OnModsInit += OnModsInit;
		On.ProcessManager.CueAchievement += ProcessManager_CueAchievement;
		On.ProcessManager.Update += ProcessManager_Update;
		On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

		On.Menu.PauseMenu.ctor += PauseMenu_ctor;
		On.Menu.PauseMenu.WarpSignal += PauseMenu_WarpSignal;
		On.Menu.Menu.PlaySound_SoundID += Menu_PlaySound_SoundID;
		On.Menu.MainMenu.ctor += MainMenu_ctor;
		On.Menu.MainMenu.Singal += MainMenu_Singal;

		On.RainWorld.AchievementAlreadyDisplayed += RainWorld_AchievementAlreadyDisplayed;
	}

	private void Menu_PlaySound_SoundID(On.Menu.Menu.orig_PlaySound_SoundID orig, Menu.Menu self, SoundID soundID)
	{
		if (self is PauseMenu && menu != null) return;
		orig(self, soundID);
	}

	private void MainMenu_Singal(On.Menu.MainMenu.orig_Singal orig, MainMenu self, MenuObject sender, string message)
	{
		orig(self, sender, message);
		if (message == "OPENMENU")
		{
			menu?.ShutDownProcess();
			self.manager.RequestMainProcessSwitch(CustomIds.loop, .5f);
		}
	}

	private void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
	{
		orig(self, manager, showRegionSpecificBkg);
		SimpleButton button = new(self, self.pages[0], self.Translate("Achievements"), "OPENMENU", new Vector2(manager.rainWorld.options.ScreenSize.x / 2f - 55f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));

		self.AddMainMenuButton(button, ToMenu, 1);
	}

	private void ToMenu()
	{
		
	}

	private void PauseMenu_WarpSignal(On.Menu.PauseMenu.orig_WarpSignal orig, PauseMenu self, MenuObject sender, string message)
	{
		orig(self, sender, message);
		if (message == "SHOWACHIEVEMENTS")
		{
			if (menu == null)
			{
				menu = new(self.manager, self);
				self.manager.sideProcesses.Add(menu);
			}
		}
	}

	private void PauseMenu_ctor(On.Menu.PauseMenu.orig_ctor orig, PauseMenu self, ProcessManager manager, RainWorldGame game)
	{
		orig(self, manager, game);
		SimpleButton button = new(self, self.pages[0], self.Translate("Achievements"), "SHOWACHIEVEMENTS", new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
		self.pages[0].subObjects.Add(button);
	}

	private void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, Procid ID)
	{
		if (ID == CustomIds.loop)
		{
			self.currentMainLoop = new AchMenu(self, null);
		}
		orig(self, ID);
		Debug.Log(ID.ToString());
		Debug.Log("tis info");
		if (processids.Contains(ID))
		{
			if (hud != null)
			{
				Hud newhud = new(self, hud);
				hud?.ShutDownProcess();
				hud = newhud;
				return;
			}
			hud?.ShutDownProcess();
			hud = new(self);
		} else
		{
			hud?.ShutDownProcess();
			hud = null;
		}
	}

	private void ProcessManager_Update(On.ProcessManager.orig_Update orig, ProcessManager self, float deltaTime)
	{
		orig(self, deltaTime);
		if (hud != null)
		{
			hud.Update();
			hud.GrafUpdate(deltaTime);
		}
		//menu?.Update();
		//menu?.GrafUpdate(deltaTime);
		if (menu != null && menu.delete)
		{
			self.sideProcesses.Remove(menu);
			menu = null;
		}
	}

	private void ProcessManager_CueAchievement(On.ProcessManager.orig_CueAchievement orig, ProcessManager self, Achid ID, float delay)
	{
		orig(self, ID, delay);
		if (!achievements.Contains(ID))
		{
			hud?.AddAchievement(ID);
			achievements.Add(ID);
		}
		achievements = achievements.Distinct().ToList();
		SaveAchievements();
	}

	private bool RainWorld_AchievementAlreadyDisplayed(On.RainWorld.orig_AchievementAlreadyDisplayed orig, RainWorld self, Achid ID)
	{
		return achievements.Contains(ID);
	}

	private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
	{
		orig(self);
		CustomIds.Initialize();
		Futile.atlasManager.LoadAtlas("atlases/allachievements");
		Futile.atlasManager.LoadImage("atlases/god");
		options = new(this, self.processManager);
		achievements = new();
		try
		{
			MachineConnector.SetRegisteredOI("timofey26.achievementtracker", options);
		}
		catch (Exception ex)
		{
			Debug.LogError("I really fucked up");
		}
		LoadAchievements();
		// Initialize assets, your mod config, and anything that uses RainWorld here
		if (!loadError)
		{
			Logger.LogDebug("Achivement tracker initialized!");
		}
	}
	private static void LoadAchievements()
	{
		achievements = new();
		try
		{
			if (!File.Exists(Filepath))
			{
				File.Create(Filepath);
			}
			string[] lines = File.ReadAllLines(Filepath);
			if (lines.Length > 0)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					try
					{
						achievements.Add((Achid)Enum.Parse(typeof(Achid), lines[i].Trim()));
					}
					catch (ArgumentException)
					{

					}
				}
			}

		}
		catch (Exception e)
		{
			switch (e)
			{
				case UnauthorizedAccessException:
					Debug.LogError("NO ACCESS TO FILES!!!");
					break;
				case DirectoryNotFoundException:
					Debug.LogError("CANNOT FIND MOD DIRECTORY");
					break;
			}
			loadError = true;
		}
	}

	public static void ClearAchievements()
	{
		if (File.Exists(Filepath))
		{
			try
			{
				File.Delete(Filepath);
			} catch (UnauthorizedAccessException)
			{
				Debug.LogError("NO ACCESS TO FILES!!!");
			} catch (DirectoryNotFoundException)
			{
				Debug.LogError("CANNOT FIND MOD DIRECTORY");
			}
		}
		LoadAchievements();
		
	}
	public static void SyncAchievements(ProcessManager self)
	{
		achievements = new();
		for (int i = 0; i < CustomIds.achievementdata.Count; i++)
		{
			Achid id = CustomIds.achievementdata.Keys.ToList()[i];
			if (self.mySteamManager.HasAchievement(id.ToString()))
			{
				achievements.Add(id);
			}
		}
		SaveAchievements();
	}

	private static void SaveAchievements()
	{
		string[] text = new string[achievements.Count];
		for (int i = 0; i < achievements.Count; i++)
		{
			text[i] = achievements[i].ToString();
		}
		File.WriteAllLines(Filepath, text);
	}
}
