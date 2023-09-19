using BepInEx;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
//using Kittehface.Framework20;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace AchievementTracker;

[BepInPlugin("timofey26.achievementtracker", "Achievement Tracker", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
	public static List<RainWorld.AchievementID> achievements;
	public static bool loadError;
	public static AchievementHud hud;

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
		On.ProcessManager.CueAchievement += ProcessManager_CueAchievement;
		On.ProcessManager.Update += ProcessManager_Update;
		On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

		On.RainWorld.AchievementAlreadyDisplayed += RainWorld_AchievementAlreadyDisplayed;
	}

	private void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
	{
		orig(self, ID);
		Debug.Log(ID.ToString());
		Debug.Log("tis info");
		if (ID != ProcessManager.ProcessID.Initialization && ID != ProcessManager.ProcessID.IntroRoll && ID != ProcessManager.ProcessID.MainMenu)
		{
			hud = new(self);
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
	}

	private void ProcessManager_CueAchievement(On.ProcessManager.orig_CueAchievement orig, ProcessManager self, RainWorld.AchievementID ID, float delay)
	{
		orig(self, ID, delay);
	}

	private bool RainWorld_AchievementAlreadyDisplayed(On.RainWorld.orig_AchievementAlreadyDisplayed orig, RainWorld self, RainWorld.AchievementID ID)
	{
		return achievements.Contains(ID);
	}

	private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
	{
		orig(self);
		Futile.atlasManager.LoadAtlas("atlases/allachievements");
		achievements = new();
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
						achievements.Add((RainWorld.AchievementID)Enum.Parse(typeof(RainWorld.AchievementID), lines[i].Trim()));
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
