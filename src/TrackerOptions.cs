using BepInEx;
using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AchievementTracker
{
	internal class TrackerOptions : OptionInterface
	{
		public OpHoldButton EraseAchievements;
		public OpHoldButton SyncAchievements;
		public readonly ProcessManager manager;
		public TrackerOptions(BaseUnityPlugin plugin, ProcessManager manager)
		{
			this.manager = manager;
		}

		public override void Initialize()
		{
			base.Initialize();
			OpTab tab1 = new(this, "options");
			EraseAchievements = new(new Vector2(400, 250), 100, "ERASE\nACHIEVEMENTS", 200) { colorEdge = Color.red, description = "Are you really really sure?" };
			EraseAchievements.OnClick += EraseAchievements_OnClick;
			SyncAchievements = new(new Vector2(0, 250), 100, "Sync\nachievements", 200) { description = "Syncs with steam" };
			SyncAchievements.OnClick += SyncAchievements_OnClick;
			tab1.AddItems(EraseAchievements);
			tab1.AddItems(SyncAchievements);
			Tabs = new OpTab[] { tab1 };
		}

		private void SyncAchievements_OnClick(UIfocusable trigger)
		{
			Plugin.SyncAchievements(manager);
		}

		private void EraseAchievements_OnClick(UIfocusable trigger)
		{
			Plugin.ClearAchievements();
		}
	}
}
