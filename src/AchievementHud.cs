using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUD;
using Menu;
using UnityEngine;

namespace AchievementTracker
{
	internal class AchievementHud : Menu.Menu
	{
		public RoundedRect rect;
		public FSprite sprite;
		public static readonly ProcessManager.ProcessID AchievementMenu = new("AchievementMenu", true);

		public float hudsize;

		public AchievementHud(ProcessManager manager) : base(manager, AchievementMenu)
		{
			pages.Add(new Page(this, null, "tracker", 0));
			hudsize = manager.rainWorld.screenSize.x / 4f;
			rect = new(this, pages[0], new(manager.rainWorld.screenSize.x - hudsize, 0), new(hudsize, manager.rainWorld.screenSize.y), true);
			pages[0].subObjects.Add(rect);
			
			sprite = new("GhostSB");
			sprite.SetPosition(rect.pos);
			sprite.scale = 3f;
			sprite.SetAnchor(new(0, 0));
			pages[0].Container.AddChild(sprite);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
			rect.GrafUpdate(timeStacker);
		}

		public override void Update()
		{
			base.Update();
			rect.Update();
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
		}
	}
}
