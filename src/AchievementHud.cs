using Menu;
using System.Collections.Generic;
using AchievementTracker;
using UnityEngine;

namespace AchievementTracker
{
	internal class AchievementHud : Menu.Menu
	{
		//drawables
		public RoundedRect rect;

		public List<AchievementDisplay> displays;
		public static readonly ProcessManager.ProcessID AchievementMenu = new("AchievementMenu", true);

		public float hudsize;

		public AchievementHud(ProcessManager manager) : base(manager, AchievementMenu)
		{
			displays = new List<AchievementDisplay>();
			pages.Add(new Page(this, null, "achtracker", 0));
			hudsize = manager.rainWorld.screenSize.x / 4f;
			rect = new(this, pages[0], new(manager.rainWorld.screenSize.x - hudsize, 0), new(hudsize, manager.rainWorld.screenSize.y), true) { };
			pages[0].subObjects.Add(rect);
			displays.Add(new(this, RainWorld.AchievementID.PassageSurvivor));

		}
		public AchievementHud(ProcessManager manager, AchievementHud hud) : this(manager)
		{
			displays = hud.displays;
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
			rect.GrafUpdate(timeStacker);
			foreach (AchievementDisplay achievement in displays)
			{
				if (achievement.delete)
				{
					displays.Remove(achievement);
					continue;
				}
				achievement.DrawSprites();
			}
		}

		public override void Update()
		{
			base.Update();
			rect.Update();
			foreach (AchievementDisplay achievement in displays)
			{
				achievement.Update();
			}
		}
		public override void ShutDownProcess()
		{
			base.ShutDownProcess();
			if (rect != null)
			{
				for (int i = 0; i < rect.sprites.Length; i++)
				{
					rect.sprites[i]?.RemoveFromContainer();
				}
			}
			rect ??= null;
			foreach (AchievementDisplay achievement in displays)
			{
				achievement.ClearSprites();
			}
		}
	}
}
