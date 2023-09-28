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
using static System.Net.Mime.MediaTypeNames;
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
	public static AchievementHud hud;
	public static AchievementMenu menu;
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

	public Plugin()
	{
		options = new TrackerOptions(this);
	}

	public static Achid GiveRandomAchievement() { return RXRandom.AnyItem(AchievementDisplay.achievementdata.Keys.ToArray()); }

	public void OnEnable()
	{
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
		On.Player.Jump += Player_Jump;
		On.ProcessManager.CueAchievement += ProcessManager_CueAchievement;
		On.ProcessManager.Update += ProcessManager_Update;
		On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

		On.Menu.PauseMenu.ctor += PauseMenu_ctor;
		On.Menu.PauseMenu.WarpSignal += PauseMenu_WarpSignal;
		On.Menu.MainMenu.ctor += MainMenu_ctor;
		On.Menu.MainMenu.Singal += MainMenu_Singal;

		On.RainWorld.AchievementAlreadyDisplayed += RainWorld_AchievementAlreadyDisplayed;
	}

	private void MainMenu_Singal(On.Menu.MainMenu.orig_Singal orig, MainMenu self, MenuObject sender, string message)
	{
		orig(self, sender, message);
		if (message == "OPENMENU")
		{
			menu?.ShutDownProcess();
			self.manager.RequestMainProcessSwitch(CustomIds.loop, .5f);
			//self.ShutDownProcess();
		}
	}

	private void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
	{
		orig(self, manager, showRegionSpecificBkg);
		SimpleButton button = new(self, self.pages[0], "Achievements", "OPENMENU", new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
		self.pages[0].subObjects.Add(button);
	}

	private void PauseMenu_WarpSignal(On.Menu.PauseMenu.orig_WarpSignal orig, PauseMenu self, MenuObject sender, string message)
	{
		orig(self, sender, message);
		if (message == "SHOWACHIEVEMENTS")
		{
			menu = new(self.manager, self);
		}
	}

	private void PauseMenu_ctor(On.Menu.PauseMenu.orig_ctor orig, PauseMenu self, ProcessManager manager, RainWorldGame game)
	{
		orig(self, manager, game);
		SimpleButton button = new(self, self.pages[0], "Achievements", "SHOWACHIEVEMENTS", Vector2.zero, new Vector2(110f, 30f));
		self.pages[0].subObjects.Add(button);
		for (int i = 0; i < self.pages[0].subObjects.Count; i++)
		{
			if (self.pages[0].subObjects[i] is SimpleButton)
			{
				self.pages[0].subObjects[i].nextSelectable[1] = button;
				button.nextSelectable[3] = self.pages[0].subObjects[i];
				button.pos = new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f));
				break;
			}
		}
	}

	private void Player_Jump(On.Player.orig_Jump orig, Player self)
	{
		hud.AddAchievement(GiveRandomAchievement());
		orig(self);
	}

	private void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, Procid ID)
	{
		if (ID == CustomIds.loop)
		{
			self.currentMainLoop = new AchievementLoop(self);
		}
		orig(self, ID);
		Debug.Log(ID.ToString());
		Debug.Log("tis info");
		if (processids.Contains(ID))
		{
			if (hud != null) { hud = new(self, hud); return; }
			hud = null;
			hud = new(self);
		} else
		{
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
		menu?.Update();
		menu?.GrafUpdate(deltaTime);
	}

	private void ProcessManager_CueAchievement(On.ProcessManager.orig_CueAchievement orig, ProcessManager self, Achid ID, float delay)
	{
		orig(self, ID, delay);
		hud?.AddAchievement(ID);
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
		achievements = new();
		try
		{
			MachineConnector.SetRegisteredOI("timofey26.achievementtracker", options);
		}
		catch (Exception ex)
		{
			Debug.LogError("I really fucked up");
		}
		string path = Custom.LegacyRootFolderDirectory() + string.Concat(new string[]
		{
			Path.DirectorySeparatorChar.ToString(),
			"UserData",
			Path.DirectorySeparatorChar.ToString(),
			"ahievementsTracker.txt"
		}).ToLowerInvariant();
		try
		{
			if (File.Exists(path))
			{

			}
			else
			{
				File.Create(path);
			}
			string[] lines = File.ReadAllLines(path);
			if (lines.Length > 0)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					try
					{
						achievements.Add((Achid)Enum.Parse(typeof(Achid), lines[i].Trim()));
					} catch (ArgumentException)
					{

					}
				}
			}

		} catch (Exception e)
		{
			switch (e) {
				case UnauthorizedAccessException:
					Debug.LogError("NO ACCESS TO FILES!!!");
					break;
				case DirectoryNotFoundException:
					Debug.LogError("CANNOT FIND MOD DIRECTORY");
					break;
			}
			loadError = true;
		}
		// Initialize assets, your mod config, and anything that uses RainWorld here
		if (!loadError)
		{
			Logger.LogDebug("Achivement tracker initialized!");
		}
	}
	private void SaveAchievements()
	{
		string path = Custom.LegacyRootFolderDirectory() + string.Concat(new string[]
		{
			Path.DirectorySeparatorChar.ToString(),
			"UserData",
			Path.DirectorySeparatorChar.ToString(),
			"ahievementsTracker.txt"
		}).ToLowerInvariant();
		string[] text = new string[achievements.Count];
		for (int i = 0; i < achievements.Count; i++)
		{
			text[i] = achievements[i].ToString();
		}
		File.WriteAllLines(path, text);
	}
}
