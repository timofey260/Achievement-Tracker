using Menu;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AchievementTracker
{
	internal class Hud : Menu.Menu
	{
		//drawables
		public RoundedRect rect;

		public List<AchievementDisplay> displays;

		public float hudsize;

		public Vector2 NewAchievementpos
		{
			get
			{
				if (displays.Count > 0)
				{
					return displays.Last().spawnpos - new Vector2(0, AchievementDisplay.achivementheight + AchievementDisplay.sizedecreasehalf);
				}
				return rect.pos + new Vector2(0, rect.size.y - AchievementDisplay.achivementheight);
			}
		}

		public Hud(ProcessManager manager) : base(manager, CustomIds.hud)
		{
			displays = new List<AchievementDisplay>();
			pages.Add(new Page(this, null, "achtracker", 0));
			hudsize = manager.rainWorld.screenSize.x / 4f;
			rect = new(this, pages[0], new(manager.rainWorld.screenSize.x - hudsize, 0), new(hudsize, manager.rainWorld.screenSize.y), true) { };
			pages[0].subObjects.Add(rect);
			//displays.Add(new(this, RainWorld.AchievementID.PassageFriend));

		}
		public Hud(ProcessManager manager, Hud hud) : this(manager)
		{
			foreach (var display in hud.displays)
			{
				displays.Add(new AchievementDisplay(this, display.achievementID) { lifespan = display.lifespan });
			}
		}

		public void AddAchievement(RainWorld.AchievementID achievementID)
		{
			displays.Add(new AchievementDisplay(this, achievementID));
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
