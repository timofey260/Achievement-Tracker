using Menu;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using RWCustom;

namespace AchievementTracker
{
	internal class Hud : Menu.Menu
	{
		//drawables
		public RoundedRect rect;

		public List<AchievementDisplay> displays;

		public float hudsize;
		public int alpha;

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
			alpha = 0;
			pages.Add(new Page(this, null, "achtracker", 0));
			hudsize = manager.rainWorld.screenSize.x / 4f;
			rect = new(this, pages[0], new(manager.rainWorld.screenSize.x - hudsize, 0), new(hudsize, manager.rainWorld.screenSize.y), true);
			pages[0].subObjects.Add(rect);
			//displays.Add(new(this, RainWorld.AchievementID.PassageFriend));

		}
		public Hud(ProcessManager manager, Hud hud) : this(manager)
		{
			foreach (var display in hud.displays)
			{
				AchievementDisplay Adisplay = new(this, pages[0], display.achievementID) { lifespan = display.lifespan };
				displays.Add(Adisplay);
				pages[0].subObjects.Add(Adisplay);
			}
		}

		public void AddAchievement(RainWorld.AchievementID achievementID)
		{
			AchievementDisplay Adisplay = new(this, pages[0], achievementID);
			displays.Add(Adisplay);
			pages[0].subObjects.Add(Adisplay);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
            foreach (var sprite in rect.sprites)
            {
				sprite.alpha = Custom.LerpQuadEaseOut(0f, 1f, alpha / 100f);
            }
        }

		public override void Update()
		{
			base.Update();
			if (displays.Count > 0)
			{
				alpha = Math.Min(alpha + 2, 100);
			} else
			{
				alpha = Math.Max(alpha - 2, 0);
			}
			foreach (AchievementDisplay achievement in displays)
			{
				if (achievement.delete)
				{
					achievement.ClearSprites();
					pages[0].RemoveSubObject(achievement);
				}
			}
			displays.RemoveAll(match => match.delete);
		}
		public override void ShutDownProcess()
		{
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
				achievement?.ClearSprites();
			}
			base.ShutDownProcess();
		}
	}
}
