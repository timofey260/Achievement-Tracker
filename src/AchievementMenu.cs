using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using UnityEngine;

namespace AchievementTracker
{
	internal class AchievementMenu : Menu.Menu
	{
		public readonly Menu.Menu connectedMenu;
		public FSprite blackSprite;
		public AchievementMenu(ProcessManager manager, Menu.Menu menu) : base(manager, CustomIds.menu)
		{
			connectedMenu = menu;
			pages.Add(new Page(this, null, "main", 0));
			blackSprite = new("pixel", true);
			blackSprite.color = MenuRGB(MenuColors.Black);
			blackSprite.scaleX = 1400f;
			blackSprite.scaleY = 800f;
			blackSprite.x = manager.rainWorld.options.ScreenSize.x / 2f;
			blackSprite.y = manager.rainWorld.options.ScreenSize.y / 2f;
			blackSprite.alpha = menu is AchievementLoop ? 0f : .5f;
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
