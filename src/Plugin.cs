using BepInEx;
using System.Security.Permissions;
using Kittehface.Framework20;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace AchievementTracker;

[BepInPlugin("timofey26.achievementtracker", "Achievement Tracker", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
	bool init;

	public void OnEnable()
	{
		// Add hooks here
		On.RainWorld.OnModsInit += OnModsInit;
		On.Player.Jump += Player_Jump;
		On.Player.ThrownSpear += Player_ThrownSpear;
		On.RainWorld.AchievementAlreadyDisplayed += RainWorld_AchievementAlreadyDisplayed;
	}

	private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
	{
		self.room.game.rainWorld.processManager.mySteamManager.ClearAchievement(RainWorld.AchievementID.HunterPayload.ToString());
	}

	private bool RainWorld_AchievementAlreadyDisplayed(On.RainWorld.orig_AchievementAlreadyDisplayed orig, RainWorld self, RainWorld.AchievementID ID)
	{
		return false;
	}

	private void Player_Jump(On.Player.orig_Jump orig, Player self)
	{
		self.room.game.rainWorld.PingAchievement(RainWorld.AchievementID.HunterPayload);
		orig(self);
	}

	private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
	{
		orig(self);

		if (init) return;

		init = true;

		// Initialize assets, your mod config, and anything that uses RainWorld here
		Logger.LogDebug("Hello world!");
	}
}
