using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using UnityEngine;

namespace AchievementTracker
{
	internal class AchMenu : Menu.Menu
	{
		public readonly Menu.Menu connectedMenu;
		public FSprite blackSprite;
		public MenuLabel label;
		public RoundedRect roundedRect;
		public AchMenu(ProcessManager manager, Menu.Menu menu) : base(manager, CustomIds.menu)
		{
			connectedMenu = menu;
			pages.Add(new Page(this, null, "main", 0));
			blackSprite = new("pixel", true)
			{
				color = MenuRGB(MenuColors.Black),
				scaleX = 1400f,
				scaleY = 800f,
				x = manager.rainWorld.options.ScreenSize.x / 2f,
				y = manager.rainWorld.options.ScreenSize.y / 2f,
				alpha = menu is AchievementLoop ? 0f : .5f
			};
			int amount = AchievementDisplay.achievementdata.Count;
			int collected = Plugin.achievements.Count;
			label = new(this, pages[0], string.Format("Total of {0} out of {1} achievements", new object[] { collected, amount }), new(0, 0), new(600, 100), false);
			roundedRect = new(this, pages[0], new Vector2(0, 0), new(500, 900), true);
			pages[0].subObjects.Add(label);
			pages[0].subObjects.Add(roundedRect);
			pages[0].Container.AddChild(blackSprite);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
		}

		public override void Update()
		{
			if (connectedMenu is PauseMenu menu)
			{
				if (menu.wantToContinue)
				{
					ShutDownProcess();
				}
			} else if (connectedMenu == null)
			{
				ShutDownProcess();
			}
			base.Update();
		}
		public override void ShutDownProcess()
		{
			blackSprite?.RemoveFromContainer();
			base.ShutDownProcess();
		}
	}
}
